using Microsoft.Extensions.Configuration;
using Susbscription.Service.Models.FeePayment;
using Susbscription.Service.Repositories.FeePayment;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UJBHelper.Common;
using UJBHelper.DataModel;
using System.Linq;
using System.Threading.Tasks;


namespace Susbscription.Service.Manager.FeePayment
{
    public class Insert:IDisposable
    {
        private Post_Request request;
        private IFeePaymentService _addFeePaymentService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        private IConfiguration _iconfiguration;
        private Notification notify_template;

        public Insert(Post_Request request, IFeePaymentService addFeePaymentService, IConfiguration iconfiguration
)
        {
            this.request = request;
            _addFeePaymentService = addFeePaymentService;
            _messages = new List<Message_Info>();
            _iconfiguration = iconfiguration;
            notify_template = new Notification();

        }

        public void Process()
        {
            try
            {
                Insert_New_Fee_Payment();
                Send_Via_Email_And_Phone();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        private Task  Send_Via_Email_And_Phone()
        {
            
            MessageBody MB = new MessageBody();
            var nq = new Notification_Sender();
            if (request.feeType == "5d5a450d339dce0154441aab")
            {
                var SendRegistrationNotification =   SendNotification("Registeration Fee", request.userId);
              //  var   nq.SendNotification("Registeration Fee", MB, request.userId, "", "");
            }
            else if (request.feeType == "5d5a4534339dce0154441aac")
            {
                var SendRegistrationNotification =  SendNotification("Membership Fee", request.userId);
               // nq.SendNotification("Membership Fee", MB, request.userId, "", "");
            }
            else if (request.feeType == "5d5a4541339dce0154441aad")
            {
                var SendRegistrationNotification =  SendNotification("Meeting Fee", request.userId);
             //   nq.SendNotification("Meeting Fee", MB, request.userId, "", "");
              
            }
            return Task.CompletedTask;
        }

        public Task SendNotification(string  Event,  string UserId)
        {
            MessageBody MB = new MessageBody();
            
            var nq = new Notification_Sender();
            nq.SendNotification(Event, MB, UserId, "", "");
            return Task.CompletedTask;

        }


        private void Send_Via_Email_And_Phone_old()
        {
            var nq = new Notification_Queue(_iconfiguration["ConnectionString"], _iconfiguration["Database"]);

            if (request.feeType == "5d5a450d339dce0154441aab")
            {
                nq.Add_To_Queue(request.userId, "", "", "", "new", "Registeration Fee", "", "Email", "User", "");
                notify_template = nq.Get_Notification_Template("Registeration Fee");
                bool isallowed = notify_template.Data.Where(x => x.Receiver == "User").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                if (isallowed)
                {
                    nq.Add_To_Queue(request.userId, "", "", "", "new", "Registeration Fee", "", "SMS", "User", "");
                }
            }
            else if(request.feeType== "5d5a4534339dce0154441aac")
            {
                nq.Add_To_Queue(request.userId, "", "", "", "new", "Membership Fee", "", "Email", "User", "");
                notify_template = nq.Get_Notification_Template("Membership Fee");
                bool isallowed = notify_template.Data.Where(x => x.Receiver == "User").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                if (isallowed)
                {
                    nq.Add_To_Queue(request.userId, "", "", "", "new", "Membership Fee", "", "SMS", "User", "");
                }
            }
            else if(request.feeType== "5d5a4541339dce0154441aad")
            {
                nq.Add_To_Queue(request.userId, "", "", "", "new", "Meeting Fee", "", "Email", "User", "");
                notify_template = nq.Get_Notification_Template("Meeting Fee");
                bool isallowed = notify_template.Data.Where(x => x.Receiver == "User").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                if (isallowed)
                {
                    nq.Add_To_Queue(request.userId, "", "", "", "new", "Meeting Fee", "", "SMS", "User", "");
                }
            }   
        }

        private void Insert_New_Fee_Payment()
        {
            try
            {
                _addFeePaymentService.Add_FeePayment_Details(request);

                _messages.Add(new Message_Info
                {
                    Message = "New Fee Payment Created",
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

            _addFeePaymentService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}
