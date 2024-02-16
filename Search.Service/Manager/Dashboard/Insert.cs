using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Search.Service.Models.Dashboard;
using Search.Service.Repositories.Dashboard;
using UJBHelper.Common;

namespace Search.Service.Manager.Dashboard
{
    public class Insert : IDisposable
    {
        private Post_Request request;
        public Get_Request _response;
        private IDashboardService _dashboardService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        private List<string> latLongLists = null;

        public Insert(Post_Request request, IDashboardService dashboardService)
        {
            this.request = request;
            _dashboardService = dashboardService;
            _messages = new List<Message_Info>();
            latLongLists = new List<string>();
        }

        public void Process()
        {
            if (Check_If_User_IsActive())
            {
                Get_Business();
                _response.Is_Active = true;
            }
            else
            {
                _response.Is_Active = false;
            }
           
        }

        private void Get_Business()
        {
            try
            {
                _response = _dashboardService.Get_Business_By_Search(request);

                if (_response.businessList.Count() != 0)
                {
                    _messages.Add(new Message_Info { Message = "Business List", Type = Message_Type.SUCCESS.ToString() });

                    _statusCode = HttpStatusCode.OK;
                }
                else
                {
                    _messages.Add(new Message_Info { Message = "No Business Found", Type = Message_Type.INFO.ToString() });

                    _statusCode = HttpStatusCode.OK;
                }

            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Exception Occured", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.InternalServerError;
            }
        }

        private bool Check_If_User_IsActive()
        {
            try
            {
                if (_dashboardService.Check_If_User_IsActive(request.userId))
                {
                   
                    return true;
                }
                else
                {
                    _messages.Add(new Message_Info
                    {
                        Message = "Partner is Inactive",
                        Type = Message_Type.SUCCESS.ToString()
                    });
                    return false;

                }


                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
                _messages.Add(new Message_Info
                {
                    Message = "No Listed Partner Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }


        public void Dispose()
        {
            request = null;
            _dashboardService = null;
            _statusCode = HttpStatusCode.OK;
            _messages = null;
        }
    }
}
