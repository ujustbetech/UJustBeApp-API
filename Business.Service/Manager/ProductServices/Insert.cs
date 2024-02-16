using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Business.Service.Models.ProductService;
using Business.Service.Repositories.ProductService;
using UJBHelper.Common;
using System.Threading.Tasks;

namespace Business.Service.Manager.ProductServices
{
    public class Insert : IDisposable
    {
        private Post_Request request;
        private IAddProductService _addProductService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Insert(Post_Request request, IAddProductService addProductService)
        {
            this.request = request;
            _addProductService = addProductService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {

            if (string.IsNullOrWhiteSpace(request.productId))
            {
                Insert_New_Product();
            }
            else
            {
                if (Verify_Product())
                {
                    if (request.isActive)
                    {
                        Update_Product_Details();
                    }
                    else
                    {
                        if (Check_If_Referral_Exists())
                        {
                            Update_Product_Details();
                        }
                    }
                }

            }
        }
        public Task SendNotification(string details, string bussinessId)
        {
            MessageBody MB = new MessageBody();
            MB.details = details;

            var nq = new Notification_Sender();

            nq.SendNotification("Product Details Updated", MB, "", "", bussinessId);
            return Task.CompletedTask;

        }
        private bool Check_If_Referral_Exists()
        {
            try
            {
                if (_addProductService.Check_If_Referral_Exists(request.productId))
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

        private void Update_Product_Details()
        {
            try
            {
                string fileds = _addProductService.Update_Product_Details(request);

                if (!string.IsNullOrWhiteSpace(fileds))
                {
                    string details = fileds;
                    var sendnotifiation = SendNotification(details, request.businessId);
                }

                _messages.Add(new Message_Info
                {
                    Message = "Product/Service Details Updated",
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

        private void Insert_New_Product()
        {
            try
            {
                _addProductService.Insert_New_Product(request);

                _messages.Add(new Message_Info
                {
                    Message = "New Product/Service Created",
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

        public void Dispose()
        {
            request = null;

            _addProductService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}
