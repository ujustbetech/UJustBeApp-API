using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Auth.Service.Models.Admin.User;
using Auth.Service.Respositories.Registeration;
using UJBHelper.Common;

namespace Auth.Service.Manager.Admin.User
{
    public class Select_All : IDisposable
    {
        public List<Get_Request> _response;
        private IUserInfoService _userInfoService;
        public List<Message_Info> _messages = null;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        private string _query = null;


        public Select_All(string query, IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
            _messages = new List<Message_Info>();
            _query = query;
        }

        public void Process()
        {
            Get_Admin_User_List();
        }

        private void Get_Admin_User_List()
        {
            try
            {
                _response = _userInfoService.Get_Admin_User_List(_query);

                if (_response.Count == 0)
                {
                    _messages.Add(new Message_Info { Message = "No Admin User.", Type = Message_Type.INFO.ToString() });
                    _statusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    _messages.Add(new Message_Info { Message = "Admin User List.", Type = Message_Type.SUCCESS.ToString() });
                    _statusCode = HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Exception Occured.", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.InternalServerError;
            }
        }

        public void Dispose()
        {
            _response = null;
            _userInfoService = null;
            _messages = null;
            _query = null;
            _statusCode = HttpStatusCode.OK;
        }
    }
}
