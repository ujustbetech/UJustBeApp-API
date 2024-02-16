using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Business.Service.Models.ProductService;
using Business.Service.Repositories.ProductService;
using UJBHelper.Common;

namespace Business.Service.Manager.ProductServices
{
    public class Update : IDisposable
    {
        private Put_Request request;
        private IAddProductService _addProductService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Update(Put_Request request, IAddProductService addProductService)
        {
            this.request = request;
            _addProductService = addProductService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            if (Verify_Product())
            {
                Update_Product_Images();
            }
        }

        private void Update_Product_Images()
        {
            try
            {
                _addProductService.Update_Product_Images(request);

                _messages.Add(new Message_Info
                {
                    Message = "Product/Service Images Updated",
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
                if (_addProductService.Check_If_Product_Exists(request.productId))
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
            request = null;

            _addProductService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}
