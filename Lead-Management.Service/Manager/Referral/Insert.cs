using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Lead_Management.Service.Models.Referral;
using Lead_Management.Service.Repositories.Referral;
using Microsoft.Extensions.Configuration;
using UJBHelper.Common;
using System.Linq;
using UJBHelper.DataModel;
using System.Threading.Tasks;

namespace Lead_Management.Service.Manager.Referral
{
    public class Insert : IDisposable
    {
        private Post_Request request;
        private IReferralService _referralService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        public string _referralId = null;
        private IConfiguration _iconfiguration;


        public Insert(Post_Request request, IReferralService referralService, IConfiguration iconfiguration
)
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
                    Create_New_Referral();

                    Update_System_Default();
                    var sendNotification = SendNotification(request.referredById, _referralId, request.businessId);
                    //  Add_To_Notification_Queue();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        public Task SendNotification(string UserId, string leadid, string businessId)
        {
            MessageBody MB = new MessageBody();
            var nq = new Notification_Sender();
            nq.SendNotification("Lead Created", MB, UserId, leadid, businessId);
            return Task.CompletedTask;

        }
        private void Add_To_Notification_Queue()
        {
            var nq = new Notification_Queue(_iconfiguration["ConnectionString"], _iconfiguration["Database"]);
            nq.Add_To_Queue("", _referralId, "", "", "new", "Lead Created", "", "Email", "UJBAdmin", "");
            Notification notify_template = new Notification();
            notify_template = nq.Get_Notification_Template("Lead Created");

            bool isaalowed = notify_template.Data.Where(x => x.Receiver == "UJBAdmin").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
            if (isaalowed)
            {
                nq.Add_To_Queue("", _referralId, "", "", "new", "Lead Created", "", "SMS", "UJBAdmin", "");
            }

            nq.Add_To_Queue("", _referralId, "", "", "new", "Lead Created", "", "Email", "Referrer", "");
            bool isaalowedreferral = notify_template.Data.Where(x => x.Receiver == "Referrer").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
            if (isaalowedreferral)
            {
                nq.Add_To_Queue("", _referralId, "", "", "new", "Lead Created", "", "SMS", "Referrer", "");
            }
            //nq.Add_To_Queue("", _referralId, "", "", "new", "Lead Acceptance", "", "Push", "Referrer", "");

            if (!request.forSelf)
            {
                // nq.Add_To_Queue("", _referralId, "", "", "new", "Lead Created", "", "Email", "Referred", "");
                bool isaalowedreferred = notify_template.Data.Where(x => x.Receiver == "Referred").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                if (isaalowedreferred)
                {
                    nq.Add_To_Queue("", _referralId, "", "", "new", "Lead Created", "", "SMS", "Referred", "");
                }
            }

            nq.Add_To_Queue("", _referralId, "", "", "new", "Lead Created", "", "Email", "Business", "");

            bool isaalowedbussiness = notify_template.Data.Where(x => x.Receiver == "Business").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
            if (isaalowedbussiness)
            {
                nq.Add_To_Queue("", _referralId, "", "", "new", "Lead Created", "", "SMS", "Business", "");
            }
            nq.Add_To_Queue("", _referralId, "", "", "new", "Lead Created", "", "Push", "Business", "");
        }

        public void Update_System_Default()
        {
            try
            {
                _referralService.Update_System_Default("ReferralCode");

                _messages.Add(new Message_Info
                {
                    Message = "Referral Created Successfully",
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

        private void Create_New_Referral()
        {
            try
            {
                _referralId = _referralService.Create_New_Referral(request);

                _messages.Add(new Message_Info
                {
                    Message = "Referral Created Successfully",
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

        private Boolean Is_Active_Users()
        {
            try
            {
                if (_referralService.Is_Active_Users(request.referredById, request.businessId))
                {
                    return true;
                }
                else
                {
                    _messages.Add(new Message_Info
                    {
                        Message = "Iactive User",
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
        public void Dispose()
        {
            _messages = null;
            _referralService = null;
            _referralId = null;
            _statusCode = HttpStatusCode.OK;
        }
    }
}
