using Auth.Service.Models.Registeration.User;
using Auth.Service.Respositories.Registeration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UJBHelper.Common;

namespace Auth.Service.Manager.Registeration.User
{
    public class Select : IDisposable
    {
        public Get_Request _response;

        private string _userId;

        public HttpStatusCode _statusCode = HttpStatusCode.OK;

        private IUserInfoService _userInfoService;

        public List<Message_Info> _messages = null;

        public Select(string userId, IUserInfoService UserInfoService)
        {
            _userId = userId;

            _messages = new List<Message_Info>();

            _userInfoService = UserInfoService;
        }

        public void Process()
        {
            if (Check_If_User_Exists())
            {
                Get_User_Details();
            }
        }

        private bool Check_If_User_Exists()
        {
            try
            {
                if (_userInfoService.Check_If_User_Exists(_userId))
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

        private void Get_User_Details()
        {
            try
            {
                _response = _userInfoService.Get_User_Details(_userId);

                _messages.Add(new Message_Info
                {
                    Message = "USER details found Successfully",
                    Type = Message_Type.SUCCESS.ToString()
                });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _messages.Add(new Message_Info
                {
                    Message = "Could not get USER details",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.InternalServerError;

                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                throw;
            }

        }

        public void Dispose()
        {
            _messages = null;

            _userInfoService = null;

            _statusCode = HttpStatusCode.OK;

            _response = null;

            _userId = null;
        }
    }
}
