using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Auth.Service.Models.ChangePassword;
using Auth.Service.Respositories.Login;
using Microsoft.Extensions.Configuration;
using UJBHelper.Common;
using UJBHelper.DataModel;

namespace Auth.Service.Manager.ChangePassword
{
    public class Insert : IDisposable
    {
        private Post_Request request;

        private Get_Request user_details;

        public HttpStatusCode _statusCode = HttpStatusCode.OK;

        private IChangePasswordService _changePasswordService;

        public List<Message_Info> _message = null;

        private string new_password = null;

        private string user_id = null;

        private IConfiguration _iconfiguration;


        public Insert(Post_Request request, IChangePasswordService ChangePasswordService, IConfiguration iconfiguration
)
        {
            _message = new List<Message_Info>();

            new_password = request.New_Password;

            user_id = request.User_Id;

            this.request = request;

            _changePasswordService = ChangePasswordService;

            _iconfiguration = iconfiguration;

        }

        public void Process()
        {
            try
            {
                if (Verify_User())
                {
                    if (Verify_Password())
                    {
                        Change_Password();
                        var sendNotification = SendNotification(request.User_Id);
                        //   Send_Via_Email_And_Phone();
                    }
                    else
                    {
                        _message.Add(new Message_Info { Message = "New And Old Password Cannot Be Same", Type = Message_Type.INFO.ToString() });

                        _statusCode = HttpStatusCode.BadRequest;
                    }
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

        private bool Verify_Password()
        {
            try
            {
                var oldPass = SecurePasswordHasherHelper.Decrypt(_changePasswordService.Get_Password(request.User_Id));

              //  var oldPass = _changePasswordService.Get_Password(request.User_Id);
                if(oldPass == request.New_Password)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
                return false;
            }
        }

        private void Generate_New_Password()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var pass = new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            new_password = pass;
        }


        public  Task SendNotification(string UserId)
        {
            MessageBody MB = new MessageBody();
            MB.new_password = new_password;
            var nq = new Notification_Sender();
             nq.SendNotification("Change Password", MB, UserId, "", "");
            return Task.CompletedTask;

        }
        private void Send_Via_Email_And_Phone()
        {
            //Send_Password_Via_Email();
            //send email
            var nq = new Notification_Queue(_iconfiguration["ConnectionString"], _iconfiguration["Database"]);
            nq.Add_To_Queue(user_id, "", "", "", "new", "Change Password", "", "Email", "User", "");

            if (Check_If_Phone_Verified())
            {
                Notification notify_template = new Notification();
                notify_template = nq.Get_Notification_Template("Change Password");

                bool isaalowed = notify_template.Data.Where(x => x.Receiver == "User").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                if (isaalowed)
                {
                    nq.Add_To_Queue(user_id, "", "", "", "new", "Change Password", "", "SMS", "User", "");
                }
                //Send_Password_Via_SMS();
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
            try
            {
                string hashnewpassowrd = SecurePasswordHasherHelper.Generate_HashPasssword(new_password);
               // _forgotPasswordService.Create_New_Password(user_id, hashnewpassowrd);
                _changePasswordService.Create_New_Password(user_id, hashnewpassowrd);

                _message.Add(new Message_Info { Message = "Password Changed Successfully", Type = Message_Type.SUCCESS.ToString() });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _message.Add(new Message_Info { Message = "Couldn't Change Password", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.BadRequest;
            }
        }

        private bool Verify_User()
        {
            try
            {
              //  var userid = _changePasswordService.Verify_User(request.User_Id);
                if (!_changePasswordService.Verify_User(request.User_Id))
                {
                    return false;
                }
               
                user_id = request.User_Id;
                user_details = _changePasswordService.Get_User_Details(user_id);
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
