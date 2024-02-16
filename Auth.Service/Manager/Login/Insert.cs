using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Auth.Service.Models.Login;
using Auth.Service.Respositories.Login;
using UJBHelper.Common;

namespace Auth.Service.Manager.Login
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
                var userid = _loginService.VerifyUser(request.Username, request.Password);
                if (!string.IsNullOrWhiteSpace(userid))
                {
                    if (userid!="300")
                    {
                        var user = _loginService.Get_Post_Login_Details(userid);

                        _response = new Get_Request();

                        _response._id = user._id;

                        _response.Language = user.language;

                        _response.Role = user.Role;

                        _response.mobile_number = user.mobileNumber;

                        _response.country_code = user.countryCode;

                        _response.Is_Otp_Verified = user.otpVerification.OTP_Verified;

                    if(user.Role == "Listed Partner")
                    {
                        _response.businessId = _loginService.Get_Business_Id(user._id);
                    }
                    _messages.Add(new Message_Info { Message = "User Verified Successfully", Type = Message_Type.SUCCESS.ToString() });

                        _statusCode = HttpStatusCode.OK;
                    }
                    else
                    {
                        _messages.Add(new Message_Info { Message = "Your account is deactivated.Please contact administrator.", Type = Message_Type.INFO.ToString() });

                        _statusCode = HttpStatusCode.MovedPermanently;
                    }
                }
                else
                {
                    _messages.Add(new Message_Info { Message = "Invalid Username or Password", Type = Message_Type.ERROR.ToString() });

                    _statusCode = HttpStatusCode.Unauthorized;
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
