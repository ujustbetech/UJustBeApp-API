using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Notification.Service.Models.FCMToken;
using Notification.Service.Repositories;
using UJBHelper.Common;

namespace Notification.Service.Manager.FCMToken
{
    public class Update : IDisposable
    {
        private Put_Request request;
        private IFCMTokenService _fcmTokenService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Update(Put_Request request, IFCMTokenService fcmTokenService)
        {
            this.request = request;
            _fcmTokenService = fcmTokenService;
            _messages = new List<Message_Info>();
        }

        internal void Process()
        {
            Update_FCM_Token();
        }

        private void Update_FCM_Token()
        {
            try
            {
                _fcmTokenService.Update_FCM_Token(request);

                _messages.Add(new Message_Info
                {
                    Message = "Token Updated Successfully",
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
            }
        }

        public void Dispose()
        {
            request = null;
            _fcmTokenService = null;
            _messages = null;
        }
    }
}
