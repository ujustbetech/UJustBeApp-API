using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Business.Service.Models.ProductService;
using Business.Service.Repositories.ProductService;
using UJBHelper.Common;

namespace Business.Service.Manager.ProductServices
{
    public class Select : IDisposable
    {
        private string prodServiceId;
        private IAddProductService _addProductService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        public Get_Request _response = null;

        public Select(string prodServiceId, IAddProductService addProductService)
        {
            this.prodServiceId = prodServiceId;
            _addProductService = addProductService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            if (Verify_Product())
            {
                Get_Product_Details();
            }
        }

        private void Get_Product_Details()
        {
            try
            {
                _response = _addProductService.Get_Product_Service_Details(prodServiceId);

                _messages.Add(new Message_Info
                {
                    Message = "Product/Service Details",
                    Type = Message_Type.SUCCESS.ToString()
                });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info
                {
                    Message = "Exception Occured",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.InternalServerError;
                throw;
            }
        }

        private bool Verify_Product()
        {
            try
            {
                if (_addProductService.Check_If_Product_Exists(prodServiceId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No Product/Service Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
                _messages.Add(new Message_Info
                {
                    Message = "No Product/Service Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }

        public void Dispose()
        {
            prodServiceId = null;

            _addProductService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}
