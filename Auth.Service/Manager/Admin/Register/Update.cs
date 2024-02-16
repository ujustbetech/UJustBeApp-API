using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Auth.Service.Models.Registeration.Register;
using Auth.Service.Respositories.Registeration;
using UJBHelper.Common;

namespace Auth.Service.Manager.Admin.Register
{
    public class Update : IDisposable
    {
        private Put_Request request;
        private IRegisterService _registerService;
        public string new_password = null;
        public List<Message_Info> _messages = null;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        private string user_id;

        public Update(Put_Request request, IRegisterService registerService)
        {
            this.request = request;
            _registerService = registerService;
            _messages = new List<Message_Info>();

        }

        public void Process()
        {
            try
            {
                if (Verify_User())
                {
                    Change_Password();
                }
                else
                {
                    _messages.Add(new Message_Info { Message = "Admin Not Found", Type = Message_Type.INFO.ToString() });

                    _statusCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Exception Occured", Type = Message_Type.INFO.ToString() });

                _statusCode = HttpStatusCode.InternalServerError;
            }
        }

        private bool Verify_User()
        {
            try
            {
                var userid = _registerService.Verify_Admin_User(request.userId,request.oldPassword);
                if (string.IsNullOrWhiteSpace(userid))
                {
                    return false;
                }
                user_id = request.userId;
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                return false;
            }
        }

        private void Change_Password()
        {
            try
            {
                _registerService.Create_New_Password(user_id, request.newPassword);

                _messages.Add(new Message_Info { Message = "Password Changed Successfully", Type = Message_Type.SUCCESS.ToString() });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Couldn't Change Password", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.BadRequest;
            }
        }


        public void Dispose()
        {
            request = null;

            _registerService = null;

            _messages = null;

            new_password = null;
        }
    }
}
