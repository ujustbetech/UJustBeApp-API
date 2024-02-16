using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Search.Service.Models.Dashboard;
using Search.Service.Repositories.Dashboard;
using UJBHelper.Common;

namespace Search.Service.Manager.Dashboard
{
    public class Select : IDisposable
    {
        public List<Get_Suggestion> _response = null;
        private string query;
        private string UserId;
        private IDashboardService _dashboardService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Select(string query,string UserId, IDashboardService dashboardService)
        {
            this.query = query;
            this.UserId = UserId;
            _dashboardService = dashboardService;
            _messages = new List<Message_Info>();
        }


        internal void Process()
        {
            try
            {
                _response = _dashboardService.Get_Business_Suggestion(query, UserId);

                if (_response.Count > 0)
                {
                    _messages.Add(new Message_Info { Message = "Business list", Type = Message_Type.SUCCESS.ToString() });

                    _statusCode = HttpStatusCode.OK;
                }
                else
                {
                    _messages.Add(new Message_Info { Message = "No Business Found", Type = Message_Type.INFO.ToString() });

                    _statusCode = HttpStatusCode.NotFound;
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
            _dashboardService = null;
            query = null;
            _messages = null;
            _statusCode = HttpStatusCode.OK;
            _response = null;
        }

    }
}
