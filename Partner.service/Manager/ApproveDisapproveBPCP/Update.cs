using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Partner.Service.Models.ApproveDisapproveBPCP;
using Partner.Service.Repositories.ApproveDisapproveBPCP;
using UJBHelper.Common;
using UJBHelper.DataModel;
using System.Linq;
using System.Threading.Tasks;

namespace Partner.Service.Manager.ApproveDisapproveBPCP
{
    public class Update : IDisposable
    {
        private Post_Request request;
        private IApproveDisapproveBPCPService _appDisappBPCPService;
        public List<Message_Info> _messages = null;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        private string mentorCode = null;
        private IConfiguration _iconfiguration;
        private Notification notify_template;

        public Update(Post_Request request, IApproveDisapproveBPCPService appDisappBPCPService, IConfiguration iconfiguration)
        {
            this.request = request;
            _appDisappBPCPService = appDisappBPCPService;
            _iconfiguration = iconfiguration;
            notify_template = new Notification();

            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            try
            {
                Generate_Mentor_Code();

                ApproveDisapproveBPCP();

                Add_To_Notification_Queue();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Exception Occured", Type = Message_Type.INFO.ToString() });

                _statusCode = HttpStatusCode.InternalServerError;
            }
        }        

        private async  void Add_To_Notification_Queue()
        {
          
            if (request.Flag)
            {
                var Send_Notification = SendNotification("Approve Partner", request.UserId);

                //nq.SendNotification("Approve Partner", MB, request.UserId, "", "");
            }
            else
            {

                var Send_Notification = SendNotification("Reject Partner", request.UserId);

                //  nq.SendNotification("Reject Partner", MB, request.UserId, "", "");
            }
        }


        public  Task SendNotification(string Event, string UserId)
        {
            MessageBody MB = new MessageBody();
           
            var nq = new Notification_Sender();
            nq.SendNotification(Event, MB, UserId, "", "");
            return Task.CompletedTask;

        }
        private void Add_To_Notification_Queue_old()
        {
            var nq = new Notification_Queue(_iconfiguration["ConnectionString"], _iconfiguration["Database"]);
            if (request.Flag)
            {
                nq.Add_To_Queue(request.UserId, "", "", "", "new", "Approve Partner", "", "Email", "User", "");
                notify_template = nq.Get_Notification_Template("Approve Partner");
                bool isallowed = notify_template.Data.Where(x => x.Receiver == "User").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                if (isallowed)
                {
                    nq.Add_To_Queue(request.UserId, "", "", "", "new", "Approve Partner", "", "SMS", "User", "");
                }
            }
            else
            {
                nq.Add_To_Queue(request.UserId, "", "", "", "new", "Reject Partner", "", "Email", "User", "");
                notify_template = nq.Get_Notification_Template("Reject Partner");
                bool isallowed = notify_template.Data.Where(x => x.Receiver == "User").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                if (isallowed)
                {
                    nq.Add_To_Queue(request.UserId, "", "", "", "new", "Reject Partner", "", "SMS", "User", "");
                }
            }
        }

        private void Generate_Mentor_Code()
        {
            mentorCode = "UJB" + Otp_Generator.Generate_Otp();
        }

        private void ApproveDisapproveBPCP()
        {
            try
            {
                _appDisappBPCPService.Approve_Disapprove_BPCP(request, mentorCode);

                _messages.Add(new Message_Info
                {
                    Message = "Approval Details Updated",
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
            mentorCode = null;
            request = null;
            _appDisappBPCPService = null;
            _messages = null;
        }

    }
}
