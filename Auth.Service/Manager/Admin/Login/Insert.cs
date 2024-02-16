using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Auth.Service.Models.Login;
using Auth.Service.Respositories.Login;
using UJBHelper.Common;

namespace Auth.Service.Manager.Admin.Login
{
    public class Insert : IDisposable
    {
        private Post_Request request;

        public Get_Request _response = null;

        public HttpStatusCode _statusCode = HttpStatusCode.OK;

        private ILoginService _loginService;

        public List<Message_Info> _messages = null;

        public Insert(Post_Request request, ILoginService loginService)
        {
            _messages = new List<Message_Info>();
            this.request = request;

            _loginService = loginService;
        }

        public void Process()
        {
            try
            {
                Verify_User();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Exception Occured", Type = Message_Type.ERROR.ToString()}) ;

                _statusCode = HttpStatusCode.InternalServerError;
            }
        }

        private void Verify_User()
        {
            try
            {
                var userid = _loginService.Verify_Admin_User(request.Username, request.Password);
                if (!string.IsNullOrWhiteSpace(userid))
                {
                    var user = _loginService.Get_Post_Login_Admin_Details(userid);

                    _response = new Get_Request();

                    _response._id = user._id;

                    //_response.Language = user.language;

                    _response.Role = user.Role;

                    //_response.Is_Otp_Verified = user.otpVerification.OTP_Verified;

                    _messages.Add(new Message_Info { Message = "Admin User Verified Successfully", Type = Message_Type.SUCCESS.ToString() });

                    _statusCode = HttpStatusCode.OK;
                }
                else
                {
                    _messages.Add(new Message_Info { Message = "Invalid Username or Password", Type = Message_Type.ERROR.ToString() });

                    _statusCode = HttpStatusCode.NotFound;
                }
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

            _messages = null;

            _response = null;

            _statusCode = HttpStatusCode.OK;
        }
    }
}
