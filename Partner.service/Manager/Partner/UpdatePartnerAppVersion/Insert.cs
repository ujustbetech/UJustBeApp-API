using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Partner.Service.Models.Partners.UpdatePartnerAppVersion;
using Partner.Service.Repositories.Partner;
using UJBHelper.Common;


namespace Partner.Service.Manager.Partner.UpdatePartnerAppVersion
{
    public class Insert : IDisposable
    {
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        private Post_Request request;
        private IUpdatePartnerProfile _updatePartnerAppVersion;

        public Insert(Post_Request request, IUpdatePartnerProfile updatePartnerAppVersion)
        {
            this.request = request;
            _updatePartnerAppVersion = updatePartnerAppVersion;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            if (Verify_User())
            {
                if (Check_If_User_AppVersion_Exist())
                {
                    Update_Partner_AppVersion();
                }
            }
        }
        private bool Verify_User()
        {
            try
            {
                if (_updatePartnerAppVersion.Check_If_User_Exist(request.userId))
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

        private bool Check_If_User_AppVersion_Exist()
        {
            try
            {
                if (!_updatePartnerAppVersion.Check_If_User_AppVersion_Exist(request.userId,request.versionCode))
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
                if (_updatePartnerAppVersion.Check_If_User_IsActive(request.userId))
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

        private void Update_Partner_AppVersion()
        {
            try
            {
                _updatePartnerAppVersion.Update_Partner_AppVersion(request);

                _messages.Add(new Message_Info { Message = "Partner App version updated successfully", Type = Message_Type.SUCCESS.ToString() });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Exception Occured", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.InternalServerError;

                throw;
            }
        }

        public void Dispose()
        {
            request = null;

            _updatePartnerAppVersion = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}