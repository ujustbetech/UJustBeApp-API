using LegalAgreement.Service.Models.AgreementStatus;
using LegalAgreement.Service.Repositories.AgreementStatus;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using UJBHelper.Common;
using UJBHelper.DataModel;

namespace LegalAgreement.Service.Manager.AgreementStatus
{
    public class ListedPartnerUpdate : IDisposable
    {
        private Put_Request request;
        private IAgreementStatusService _agreementStatusService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        private Notification notify_template;
        // private Email_Details referralDetails = null;
        private IConfiguration _iconfiguration;


        public ListedPartnerUpdate(Put_Request request, IAgreementStatusService agreementStatusService, IConfiguration iconfiguration)
        {
            this.request = request;
            _agreementStatusService = agreementStatusService;
            _messages = new List<Message_Info>();
            _iconfiguration = iconfiguration;

        }

        public void Process()
        {
            Update_Agreement_Status();

            Send_ListedPartner_Agreement_Via_Email();

        }

        private void Send_ListedPartner_Agreement_Via_Email()
        {
            try
            {
                _agreementStatusService.Send_ListedPartner_Agreement_Via_Email(request.userId, request.statusId);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        private void Update_Agreement_Status()
        {
            try
            {
                _agreementStatusService.Update_Listed_Partner_Agreement_Status(request);
                _messages.Add(new Message_Info
                {
                    Message = "Agreement Status Updated",
                    Type = Message_Type.SUCCESS.ToString()
                });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info
                {
                    Message = "Exception Occured (Error Updating Status)",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.InternalServerError;
                throw;
            }
        }

        public void Dispose()
        {
            request = null;
            _messages = null;
            _agreementStatusService = null;
            _statusCode = HttpStatusCode.OK;
        }
    }
}