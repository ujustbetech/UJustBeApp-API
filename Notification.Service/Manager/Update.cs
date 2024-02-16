using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Notification.Service.Models;
using Notification.Service.Repositories;
using UJBHelper.Common;

namespace Notification.Service.Manager
{
    public class Update : IDisposable
    {
        private Put_Request request;
        private INotificationService _notificationService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Update(Put_Request request, INotificationService notificationService)
        {
            this.request = request;
            _notificationService = notificationService;
            _messages = new List<Message_Info>();
        }

        internal void Process()
        {
            Update_Notification_Read_Flag();
        }

        private void Update_Notification_Read_Flag()
        {
            try
            {
                _notificationService.Update_Notification_Read_Flag(request.notificationIds);

                _messages.Add(new Message_Info
                {
                    Message = "Read Flag Updated Successfully",
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
            _notificationService = null;
            _messages = null;
        }
    }
}
