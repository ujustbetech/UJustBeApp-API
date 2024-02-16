using Auth.Service.Models.Registeration.MobileCheck;
using Auth.Service.Respositories.Registeration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UJBHelper.Common;

namespace Auth.Service.Manager.Registeration.MobileCheck
{
    public class Insert : IDisposable
    {
        private Post_Request request;

        public HttpStatusCode _statusCode = HttpStatusCode.OK;

        private IEmailCheckService _emailCheckService;

        public List<Message_Info> _messages = null;

        public Insert(Post_Request post_Request, IEmailCheckService emailCheckService)
        {
            _messages = new List<Message_Info>();

            request = post_Request;

            _emailCheckService = emailCheckService;
        }

        public void Process()
        {
            Check_If_Mobile_Exists();
        }

        private void Check_If_Mobile_Exists()
        {
            try
            {
                if (_emailCheckService.Check_If_Mobile_Exists(request.MobileNo))
                {
                    _messages.Add(new Message_Info
                    {
                        Message = string.Format("Mobile No. <{0}> Exists", request.MobileNo),
                        Type = Message_Type.SUCCESS.ToString()
                    });

                    _statusCode = HttpStatusCode.OK;
                }
                else
                {
                    _messages.Add(new Message_Info
                    {
                        Message = string.Format("Mobile No. <{0}> Does not Exists", request.MobileNo),
                        Type = Message_Type.ERROR.ToString()
                    });

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
            _messages = null;

            _emailCheckService = null;

            _statusCode = HttpStatusCode.OK;

            request = null;
        }
    }
}
