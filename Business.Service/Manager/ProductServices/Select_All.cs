using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Business.Service.Models.ProductService;
using Business.Service.Repositories.ProductService;
using UJBHelper.Common;

namespace Business.Service.Manager.ProductServices
{
    public class Select_All : IDisposable
    {
        private string userId;
        private string type;
        public List<Get_Request> _response = null;
        private IAddProductService _addProductService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Select_All(string userId, string type, IAddProductService addProductService)
        {
            this.userId = userId;
            this.type = type;
            _addProductService = addProductService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            if (Verify_User())
            {
                Get_All_Product_Service();
            }
        }

        private void Get_All_Product_Service()
        {
            try
            {
                switch (type)
                {
                    case "Product":
                        _response = _addProductService.Get_Products_By_User(userId);
                        if (_response.Count == 0)
                        {
                            _messages.Add(new Message_Info { Message = "No Products Found", Type = Message_Type.INFO.ToString() });

                            _statusCode = HttpStatusCode.NotFound;
                        }
                        else
                        {
                            _messages.Add(new Message_Info { Message = "Products List Received", Type = Message_Type.SUCCESS.ToString() });

                            _statusCode = HttpStatusCode.OK;
                        }
                        break;
                    case "Service":
                        _response = _addProductService.Get_Services_By_User(userId);
                        if (_response.Count == 0)
                        {
                            _messages.Add(new Message_Info { Message = "No Services Found", Type = Message_Type.INFO.ToString() });

                            _statusCode = HttpStatusCode.NotFound;
                        }
                        else
                        {
                            _messages.Add(new Message_Info { Message = "Services List Received", Type = Message_Type.SUCCESS.ToString() });

                            _statusCode = HttpStatusCode.OK;
                        }
                        break;
                    case "Both":
                        _response = _addProductService.Get_Products_Services_By_User(userId);
                        if (_response.Count == 0)
                        {
                            _messages.Add(new Message_Info { Message = "No Product / Services Found", Type = Message_Type.INFO.ToString() });

                            _statusCode = HttpStatusCode.NotFound;
                        }
                        else
                        {
                            _messages.Add(new Message_Info { Message = "Products / Services List Received", Type = Message_Type.SUCCESS.ToString() });

                            _statusCode = HttpStatusCode.OK;
                        }
                        break;
                    default:
                        _messages.Add(new Message_Info { Message = "Invalid Type Specified", Type = Message_Type.ERROR.ToString() });

                        _statusCode = HttpStatusCode.InternalServerError;
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Exception Occured", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.InternalServerError;
                throw;
            }
        }

        private bool Verify_User()
        {
            try
            {
                if (_addProductService.Check_If_User_Exists(userId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No User Found",
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
                    Message = "No User Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }

        public void Dispose()
        {
            userId = null;

            type = null;

            _addProductService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}
