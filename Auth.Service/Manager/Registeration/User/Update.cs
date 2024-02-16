using Auth.Service.Models.Registeration.User;
using Auth.Service.Respositories.Registeration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UJBHelper.Common;

namespace Auth.Service.Manager.Registeration.User
{
    public class Update:IDisposable
    {
        private Put_Request request;

        public HttpStatusCode _statusCode = HttpStatusCode.OK;

        private IUserInfoService _userInfoService;       

        public List<Message_Info> _messages = null;

        public Update(Put_Request request, IUserInfoService userInfoService)
        {
            _messages = new List<Message_Info>();

            _userInfoService = userInfoService;

            this.request = request;
        }

        public void Process()
        {
            try
            {
                if (Verify_UserIsActive())
                {
                    if (Verify_User())
                    {
                        Update_UserOther_Details();

                    }
                    else
                    {
                        Insert_UserOther_Details();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }
        

        private bool Verify_User()
        {
            try
            {
                if (_userInfoService.Check_If_User_Other_Exists(request.UserId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No Users Found",
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
                    Message = "No Users Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }

        private bool Verify_UserIsActive()
        {
            try
            {
                if (_userInfoService.Check_If_User_IsActive(request.UserId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "User Is InActive",
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
                    Message = "User Is InActive",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }

        private void Update_UserOther_Details()
        {
            _userInfoService.Update_UserOtherDetails(request);

            _messages.Add(new Message_Info { Message = "User Other Details Updated Successfully", Type = Message_Type.SUCCESS.ToString() });

            _statusCode = HttpStatusCode.OK;
        }

        private void Insert_UserOther_Details()
        {
            _userInfoService.Insert_UserOther_Details(request);

            _messages.Add(new Message_Info { Message = "User Other Details Created Successfully", Type = Message_Type.SUCCESS.ToString() });

            _statusCode = HttpStatusCode.OK;
        }

        public void Dispose()
        {
            _messages = null;

            _statusCode = HttpStatusCode.OK;

            request = null;

            _userInfoService = null;
        }
    }
}
