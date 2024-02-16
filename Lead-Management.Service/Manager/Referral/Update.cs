using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Lead_Management.Service.Models.Referral;
using Lead_Management.Service.Repositories.Referral;
using Microsoft.Extensions.Configuration;
using UJBHelper.Common;
using UJBHelper.DataModel;
using System.Linq;
using System.Threading.Tasks;

namespace Lead_Management.Service.Manager.Referral
{
    public class Update : IDisposable
    {
        private Put_Request request;
        private IReferralService _referralService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        private IConfiguration _iconfiguration;


        private Email_Details referralDetails = null;

        public Update(Put_Request request, IReferralService referralService, IConfiguration iconfiguration)
        {
            this.request = request;
            _referralService = referralService;
            _messages = new List<Message_Info>();
            _iconfiguration = iconfiguration;

        }

        public void Process()
        {
            try
            {
                if (Is_Active_Users())
                {
                    var nq = new Notification_Queue(_iconfiguration["ConnectionString"], _iconfiguration["Database"]);

                    Update_Referral_Status();
                    var sendnotification = SendNotification(request.userId, request.dealId);
                }

            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        private Boolean Is_Active_Users()
        {
            try
            {
                if (_referralService.Is_Active_Users(request.dealId))
                {
                    return true;
                }
                else
                {
                    _messages.Add(new Message_Info
                    {
                        Message = "Inactive User",
                        Type = Message_Type.ERROR.ToString()
                    });

                    _statusCode = HttpStatusCode.Unauthorized;
                }
                return false;


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
        public Task SendNotification(string UserId, string LeadId)
        {
            MessageBody MB = new MessageBody();
            var nq = new Notification_Sender();

            if (request.referralStatus == 1)
            {

                nq.SendNotification("Lead Acceptance", MB, UserId, LeadId, "");
                //  


            }
            else
            {
                nq.SendNotification("Lead Rejection", MB, UserId, LeadId, "");
                //   return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }
        //public asyncvoidSendNotification_async(string UserId, string LeadId)
        //{
        //    MessageBody MB = new MessageBody();
        //    var nq = new Notification_Sender();

        //    if (request.referralStatus == 1)
        //    {

        //         nq.SendNotification("Lead Acceptance", MB, UserId, LeadId,"");
        //    //    return Task.CompletedTask;


        //    }
        //    else
        //    {
        //          nq.SendNotification("Lead Rejection", MB, UserId, LeadId, "");
        //     //   return Task.CompletedTask;
        //    }
        //}

        private void Update_Referral_Status()
        {
            try
            {
                _referralService.Update_Referral_Rejection_Status(request);

                _messages.Add(new Message_Info
                {
                    Message = "Referral Status Updated",
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
            _referralService = null;
            _messages = null;
            referralDetails = null;
        }
    }

    public class Email_Details
    {
        public string ujbAdminEmailId { get; set; }

        public string referredByName { get; set; }
        public string referredByemailId { get; set; }

        public string referredToName { get; set; }
        public string referredToemailId { get; set; }

        public string clientPartnerName { get; set; }
        public string clientPartneremailId { get; set; }

        public string productServiceName { get; set; }
        public string dealStatus { get; set; }
    }
}
