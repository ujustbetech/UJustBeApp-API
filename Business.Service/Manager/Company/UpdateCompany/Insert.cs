using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Business.Service.Models.Company.UpdateCompany;
using Business.Service.Repositories.Company;
using UJBHelper.Common;

namespace Business.Service.Manager.Company.UpdateCompany
{
    public class Insert : IDisposable
    {
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        private Post_Request request;
        private IUpdateCompanyService _updateCompanyService;

        public Insert(Post_Request request, IUpdateCompanyService updateCompanyService)
        {
            this.request = request;
            _updateCompanyService = updateCompanyService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            if (Verify_Business())
            {
                Update_Company_Details();
            }
        }

        private bool Verify_Business()
        {
            try
            {
                if (_updateCompanyService.Check_If_Business_Exists(request.businessId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No Business Found",
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
                    Message = "No Business Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }

        private void Update_Company_Details()
        {
            try
            {
                switch (request.type)
                {
                    case "Name":
                        _updateCompanyService.UpdateCompanyName(request);
                        break;
                    case "NameOfPartner":
                        _updateCompanyService.UpdateNameOfPartner(request);
                        break;
                    case "Email":
                        _updateCompanyService.UpdateBusinessEmail(request);
                        break;
                    case "Description":
                        bool bussinesupdate = _updateCompanyService.UpdateBusinessDescription(request);
                        if (bussinesupdate)
                        {
                            string details = "\"Bussiness Description\"";
                            var sendnotifiation = SendNotification(details, request.businessId);
                        }
                        break;
                    case "URL":
                        _updateCompanyService.UpdateBusinessUrl(request);
                        break;
                    case "GST":
                        _updateCompanyService.UpdateBusinessGst(request);
                        break;

                }

                _messages.Add(new Message_Info { Message = $"Business {request.type} updated successfully", Type = Message_Type.SUCCESS.ToString() });

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

        public Task SendNotification(string details, string bussinessId)
        {
            MessageBody MB = new MessageBody();
            MB.details = details;

            var nq = new Notification_Sender();

            nq.SendNotification("Product Details Updated", MB, "", "", bussinessId);
            return Task.CompletedTask;

        }

        public void Dispose()
        {
            request = null;

            _updateCompanyService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}
