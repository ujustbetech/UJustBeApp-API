using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Auth.Service.Models.ForgotPassword;
using Auth.Service.Respositories.Login;
using Microsoft.Extensions.Configuration;
using UJBHelper.Common;
using UJBHelper.DataModel;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Service.Manager.ForgotPassword
{
    public class Insert : IDisposable
    {
        private Post_Request request;

        private Get_Request user_details;

        public HttpStatusCode _statusCode = HttpStatusCode.OK;

        private IForgotPasswordService _forgotPasswordService;

        public List<Message_Info> _message = null;

        public string new_password = null;

        private string user_id = null;

        private IConfiguration _iconfiguration;


        public Insert(Post_Request request, IForgotPasswordService forgotPasswordService, IConfiguration iconfiguration)
        {
            _message = new List<Message_Info>();

            this.request = request;

            _forgotPasswordService = forgotPasswordService;

            _iconfiguration = iconfiguration;

        }

        public void Process()
        {
            try
            {
                if (Verify_User())
                {
                    Generate_New_Password();

                    Change_Password();
                    var sendNotification = SendNotification(user_id);
                    //Send_Via_Email_And_Phone();

                    _message.Add(new Message_Info { Message = "Please check your mail for new password", Type = Message_Type.INFO.ToString() });

                    _statusCode = HttpStatusCode.OK;
                }
                else
                {
                    _message.Add(new Message_Info { Message = "User Not Found", Type = Message_Type.INFO.ToString() });

                    _statusCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _message.Add(new Message_Info { Message = "Exception Occured", Type = Message_Type.INFO.ToString() });

                _statusCode = HttpStatusCode.InternalServerError;
            }
        }

        private void Generate_New_Password()
        {
            new_password = Password_Generator.Generate_New_Password();
        }


        public  Task SendNotification(string UserId)
        {
            MessageBody MB = new MessageBody();
            MB.new_password = new_password;
            var nq = new Notification_Sender();
             nq.SendNotification("Forgot Password", MB, UserId, "", "");
            return Task.CompletedTask;

        }

        private void Send_Via_Email_And_Phone()
        {
            //Send_Password_Via_Email();
            //send email
            var nq = new Notification_Queue(_iconfiguration["ConnectionString"], _iconfiguration["Database"]);
            nq.Add_To_Queue(user_id, "", "", "", "new", "Forgot Password", "", "Email", "User", "");

            if (Check_If_Phone_Verified())
            {
                Notification notify_template = new Notification();
                notify_template = nq.Get_Notification_Template("Forgot Password");

                bool isaalowed = notify_template.Data.Where(x => x.Receiver == "User").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                if (isaalowed)
                {
                    nq.Add_To_Queue(user_id, "", "", "", "new", "Forgot Password", "", "SMS", "User", "");
                }
            }
            else
            {
                _message.Add(new Message_Info { Message = "Mobile No. not verified!", Type = Message_Type.INFO.ToString() });
            }
        }

        private bool Check_If_Phone_Verified()
        {
            return user_details.Is_Otp_Verified;
        }

        private void Change_Password()
        {
            //  _forgotPasswordService.Create_New_Password(user_id, new_password);
            try
            {

                string hashnewpassowrd = SecurePasswordHasherHelper.Generate_HashPasssword(new_password);
                _forgotPasswordService.Create_New_Password(user_id, hashnewpassowrd);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        private bool Verify_User()
        {
            try
            {
                var userid = _forgotPasswordService.Verify_User(request.Username);
                if (string.IsNullOrWhiteSpace(userid))
                {
                    return false;
                }
                user_id = userid;
                user_details = _forgotPasswordService.Get_User_Details(user_id);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                throw;
            }
        }

        public void Dispose()
        {
            request = null;

            _message = null;

            _statusCode = HttpStatusCode.OK;
        }
    }
}
