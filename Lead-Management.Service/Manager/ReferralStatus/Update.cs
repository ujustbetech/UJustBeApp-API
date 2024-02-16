using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Lead_Management.Service.Manager.Referral;
using Lead_Management.Service.Models.ReferralStatus;
using Lead_Management.Service.Repositories.ReferralStatus;
using Microsoft.Extensions.Configuration;
using UJBHelper.Common;
using UJBHelper.DataModel;
using System.Linq;
using System.Threading.Tasks;
namespace Lead_Management.Service.Manager.ReferralStatus
{
    public class Update : IDisposable
    {
        private Put_Request request;
        private IReferralStatusService _referralStatusService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        private Email_Details referralDetails = null;
        private IConfiguration _iconfiguration;


        public Update(Put_Request request, IReferralStatusService referralStatusService, IConfiguration iconfiguration)
        {
            this.request = request;
            _referralStatusService = referralStatusService;
            _messages = new List<Message_Info>();
            _iconfiguration = iconfiguration;

        }

        public void Process()
        {
            if (Is_Active_Users() || request.updatedBy == "Admin")
            {
                Update_Referral_Status();

                Get_Referral_Details();
                var sendNotification = SendNotification(request.leadId, request.statusId);
            }
            // Add_To_Queue();
        }

        private Boolean Is_Active_Users()
        {
            try
            {
                if (_referralStatusService.Is_Active_Users(request.leadId))
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
        public async Task SendNotification(string LeadId, int statusId)
        {
            MessageBody MB = new MessageBody();
            var nq = new Notification_Sender();
            MB.statusId = statusId;
            nq.SendNotification("Lead Status Changed", MB, "", LeadId, "");
            //  return Task.CompletedTask;
        }
        private void Get_Referral_Details()
        {
            try
            {
                referralDetails = _referralStatusService.Get_Referrer_Email_Id(request.leadId);

            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info
                {
                    Message = "Exception Occured (Cannot get Referral Details to send mail)",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.InternalServerError;
                throw;
            }

        }

        private void Add_To_Queue()
        {
            try
            {

                var current_status = "";


                switch (request.statusId)
                {
                    case 1:
                        current_status = "Not Connected";
                        break;
                    case 2:
                        current_status = "Called But No Response";
                        break;
                    case 3:
                        current_status = "Deal Lost";
                        break;
                    case 4:
                        current_status = "Discussion In Progress";
                        break;
                    case 5:
                        current_status = "Deal Won";
                        break;
                    case 6:
                        current_status = "Received Part Payment & Transferred to UJustBe";
                        break;
                    case 7:
                        current_status = "Work In Progress";
                        break;
                    case 8:
                        current_status = "Work Completed";
                        break;
                    case 9:
                        current_status = "Received Full & Final Payment";
                        break;
                    case 10:
                        current_status = "Agreed Percentage Transferred to UJB";
                        break;
                }

                var nq = new Notification_Queue(_iconfiguration["ConnectionString"], _iconfiguration["Database"]);
                Notification notify_template = new Notification();
                notify_template = nq.Get_Notification_Template("Lead Status Changed");
                nq.Add_To_Queue("", request.leadId, "", "", "new", "Lead Status Changed", "", "Email", "UJBAdmin", "");
                bool isallowed = notify_template.Data.Where(x => x.Receiver == "UJBAdmin").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                if (isallowed)
                {

                    nq.Add_To_Queue("", request.leadId, "", "", "new", "Lead Status Changed", "", "SMS", "UJBAdmin", "");
                }
                nq.Add_To_Queue("", request.leadId, "", "", "new", "Lead Status Changed", "", "Email", "Referrer", "");
                bool isallowedReferrer = notify_template.Data.Where(x => x.Receiver == "Referrer").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                if (isallowedReferrer)
                {
                    nq.Add_To_Queue("", request.leadId, "", "", "new", "Lead Status Changed", "", "SMS", "Referrer", "");
                }
                nq.Add_To_Queue("", request.leadId, "", "", "new", "Lead Status Changed", "", "Push", "Referrer", "");
                //nq.Add_To_Queue("", request.leadId, "", "", "new", "Lead Status Changed", "", "Email", "Referred", "");
                nq.Add_To_Queue("", request.leadId, "", "", "new", "Lead Status Changed", "", "Email", "Business", "");
                bool isallowedBusiness = notify_template.Data.Where(x => x.Receiver == "Business").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                if (isallowedBusiness)
                {
                    nq.Add_To_Queue("", request.leadId, "", "", "new", "Lead Status Changed", "", "SMS", "Business", "");

                }
                nq.Add_To_Queue("", request.leadId, "", "", "new", "Lead Status Changed", "", "Push", "Business", "");
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        private void Update_Referral_Status()
        {
            try
            {
                _referralStatusService.Update_Referral_Status(request);
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
            _referralStatusService = null;
            _statusCode = HttpStatusCode.OK;
            referralDetails = null;
        }
    }
}
