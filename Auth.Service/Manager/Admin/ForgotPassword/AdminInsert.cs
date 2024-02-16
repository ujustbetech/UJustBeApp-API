using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UJBHelper.Common;
using Auth.Service.Models.Admin.ForgotPassword;
using Auth.Service.Respositories.Login;
using Microsoft.Extensions.Configuration;
using UJBHelper.DataModel;
using System.Collections.Generic;
using System.Linq;

namespace Auth.Service.Manager.Admin.ForgotPassword
{
    public class AdminInsert:IDisposable
    {
        private Post_Request request;

        private Get_Admin_Request user_details;

        public HttpStatusCode _statusCode = HttpStatusCode.OK;

        private IForgotPasswordService _forgotPasswordService;

        public List<Message_Info> _message = null;

        public string new_password = null;

        private string user_id = null;

        private IConfiguration _iconfiguration;
        private Notification notify_template;
        public AdminInsert(Post_Request request, IForgotPasswordService forgotPasswordService, IConfiguration iConfig)
        {
            _message = new List<Message_Info>();

            this.request = request;

            _forgotPasswordService = forgotPasswordService;

            _iconfiguration = iConfig;
        }

        public void Process()
        {
            try
            {
                if (Verify_User())
                {
                    Generate_New_Password();
                    Send_Via_Email_And_Phone();
                    Change_Password();                    
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

        private void Send_Via_Email_And_Phone()
        {
            //Send_Password_Via_Email();
            //Send_Password_Via_SMS();
            var nq = new Notification_Queue(_iconfiguration["ConnectionString"], _iconfiguration["Database"]);
            nq.Add_To_Queue(user_id, "", "", "", "new", "Forgot Password", "", "Email", "Admin", "");

            Notification notify_template = new Notification();
             notify_template =  nq.Get_Notification_Template("Forgot Password");
          
            bool isaalowed = notify_template.Data.Where(x => x.Receiver == "Admin").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
            if (isaalowed)
            {
            //    nq.Add_To_Queue(user_id, "", "", "", "new", "Forgot Password", "", "SMS", "Admin", "");
            }
            //if (Check_If_Phone_Verified())
            //{

            //}
            //else
            //{
            //    _message.Add(new Message_Info { Message = "Mobile No. not verified!", Type = Message_Type.INFO.ToString() });
            //}
        }

        private void Change_Password()
        {
            try
            {
                _forgotPasswordService.Create_New_Admin_Password(user_id, new_password);
                _message.Add(new Message_Info { Message = "Password Reset and Sent Successfully", Type = Message_Type.SUCCESS.ToString() });
                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
                _message.Add(new Message_Info { Message = "Couldn't Reset Password", Type = Message_Type.ERROR.ToString() });
                _statusCode = HttpStatusCode.BadRequest;
            }
        }

        private bool Verify_User()
        {
            try
            {
                var userid = _forgotPasswordService.Verify_Admin_User(request.Username);
                if (string.IsNullOrWhiteSpace(userid))
                {
                    return false;
                }
                user_id = userid;
                user_details = _forgotPasswordService.Get_Admin_User_Details(user_id);
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




        
       

       

      

       

       

       

       
   

