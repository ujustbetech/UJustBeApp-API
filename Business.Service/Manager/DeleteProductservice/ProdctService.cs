using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Business.Service.Models.DeleteProductService;
using Business.Service.Repositories.DeleteProductService;
using UJBHelper.Common;

namespace Business.Service.Manager.DeleteProductservice
{
    public class DeleteProductService : IDisposable
    {
        private string ProdServiceId;
        // private string type;

        private IDeleteProductService _deleteProductService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        public DeleteProductService(string ProdServiceId, IDeleteProductService deleteProductService)
        {
            this.ProdServiceId = ProdServiceId;
            //  this.type = type;
            _deleteProductService = deleteProductService;
            _messages = new List<Message_Info>();
        }
        public void Process()
        {
            if (Check_If_Product_Exists())
            {
                if (Check_If_Referral_Exists())
                {
                    Delete_Product_Service();
                }
            }

        }



        private void Delete_Product_Service()
        {
            try
            {
                _deleteProductService.Delete_Products_service(ProdServiceId);

                _messages.Add(new Message_Info { Message = "Product Service Deleted successfully", Type = Message_Type.SUCCESS.ToString() });

                _statusCode = HttpStatusCode.OK;

            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Exception Occured", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.InternalServerError;
                throw;
            }
        }

        private bool Check_If_Product_Exists()
        {
            try
            {
                if (_deleteProductService.Check_If_Product_Exists(ProdServiceId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No product Found",
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
                    Message = "No product Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }

        private bool Check_If_Referral_Exists()
        {
            try
            {
                if (_deleteProductService.Check_If_Referral_Exists(ProdServiceId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "This product or service can not be deleted ",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotAcceptable;

                return false;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
                _messages.Add(new Message_Info
                {
                    Message = "This product or service can not be deleted ",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotAcceptable;

                return false;
            }
        }

        public void Dispose()
        {
            ProdServiceId = null;

            // type = null;

            _deleteProductService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }

    }



}
