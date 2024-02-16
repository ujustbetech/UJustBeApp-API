using Auth.Service.Models.Registeration.Register;
using Auth.Service.Respositories.Registeration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UJBHelper.Common;

namespace Auth.Service.Manager.Admin.Register
{
    public class Insert : IDisposable
    {
        private Post_Request request;

        public Get_Request _response;

        public HttpStatusCode _statusCode = HttpStatusCode.OK;

        private IRegisterService _registerService;

        public List<Message_Info> _messages = null;

        private string _new_otp = "";

        public Insert(Post_Request post_Request, IRegisterService registerService)
        {
            _messages = new List<Message_Info>();

            request = post_Request;

            _registerService = registerService;
        }

        public void Process()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    Generate_Otp();

                    Insert_New_User();

                    //Send_Otp_Via_Sms();
                }
                else
                {
                    Update_Existing_User();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Error While Registering User", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.InternalServerError;

                throw;
            }
        }

        private void Send_Otp_Via_Sms()
        {
            try
            {
                var message = $"{_new_otp} is your OTP for UJUSTB Registeration. use this OTP to continue";
                var mobilenumber = request.countryCode.Substring(1) + request.mobileNumber;
                Email_Sms_Sender.Send_Sms(message, mobilenumber);

                _response.otp = _new_otp;

                _messages.Add(new Message_Info { Message = "OTP sent successfully via SMS", Type = Message_Type.SUCCESS.ToString() });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Couldn't Send OTP Via SMS", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.BadRequest;
            }
        }

        private void Generate_Otp()
        {
            _new_otp = Otp_Generator.Generate_Otp();
        }

        private void Update_Existing_User()
        {
            try
            {
                _response = _registerService.Update_Admin_User(request);

                _messages.Add(new Message_Info { Message = "Admin User Updated Successfully", Type = Message_Type.SUCCESS.ToString() });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        private void Insert_New_User()
        {
            try
            {
                _response = _registerService.Insert_Admin_User(request, _new_otp);

                _messages.Add(new Message_Info { Message = "Admin User Created Successfully", Type = Message_Type.SUCCESS.ToString() });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        public void Dispose()
        {
            request = null;

            _response = null;

            _registerService = null;

            _messages = null;

            _new_otp = null;
        }
    }
}