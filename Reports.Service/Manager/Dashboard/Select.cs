using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Reports.Service.Models.Dashboard;
using Reports.Service.Repositories.Dashboard;
using UJBHelper.Common;

namespace Reports.Service.Manager.Dashboard
{
    public class Select : IDisposable
    {
        public Post_Request _response = null;
        private string userId;
        private string type;
        private IDashboardService _dashboardService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Select(string userId, string type, IDashboardService dashboardService)
        {
            this.userId = userId;
            this.type = type;
            _dashboardService = dashboardService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            if (Verify_User())
            {
                Get_Stats();
            }
        }

        private bool Verify_User()
        {
            try
            {
                if (_dashboardService.Check_If_User_Exists(userId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No User Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
                _messages.Add(new Message_Info
                {
                    Message = "No User Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }

        private void Get_Stats()
        {
            try
            {
                switch (type)
                {
                    case "Partner":
                        _response = _dashboardService.Get_Partner_Stats(userId);
                        _messages.Add(new Message_Info { Message = "Partner Stats.", Type = Message_Type.SUCCESS.ToString() });

                        _statusCode = HttpStatusCode.OK;
                        break;
                    case "Listed Partner":
                        _response = _dashboardService.Get_Client_Partner_Stats(userId);
                        _messages.Add(new Message_Info { Message = "Listed Partner Stats.", Type = Message_Type.SUCCESS.ToString() });

                        _statusCode = HttpStatusCode.OK;
                        break;
                    default:
                        _messages.Add(new Message_Info { Message = "Invalid Type Specified", Type = Message_Type.INFO.ToString() });

                        _statusCode = HttpStatusCode.NotFound;
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Exception Occured", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.InternalServerError;
            }
        }

        public void Dispose()
        {
            userId = null;
            type = null;
            _dashboardService = null;
            _response = null;
            _messages = null;
        }
    }
}
