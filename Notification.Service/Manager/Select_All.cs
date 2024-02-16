using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Notification.Service.Models;
using Notification.Service.Repositories;
using UJBHelper.Common;

namespace Notification.Service.Manager
{
    public class Select_All : IDisposable
    {
        private INotificationService _notificationService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        public Get_Request _response = null;
        private Post_Request request;
        

        public Select_All(Post_Request request, INotificationService notificationService)
        {
            this.request = request;
            _notificationService = notificationService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            Get_All_Notification();
        }

        private void Get_All_Notification()
        {
            try
            {
                _response = _notificationService.Get_All_Notifications(request);

                if (_response.notifications.Count == 0)
                {
                    _messages.Add(new Message_Info
                    {
                        Message = "No Notifications Found",
                        Type = Message_Type.INFO.ToString()
                    });

                    _statusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    _messages.Add(new Message_Info
                    {
                        Message = "Notifications List",
                        Type = Message_Type.SUCCESS.ToString()
                    });

                    _statusCode = HttpStatusCode.OK;
                }
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
            _response = null;
        }
    }
}
