using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Partner.Service.Models.Partners.UpdateLocation;
using Partner.Service.Repositories.Partner;
using UJBHelper.Common;

namespace Partner.Service.Manager.Partner.UpdateLocation
{
    public class Insert : IDisposable
    {
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        private Post_Request request;
        private IUpdatePartnerProfile _updatePartnerLocation;

        public Insert(Post_Request request, IUpdatePartnerProfile updatePartnerLocation)
        {
            this.request = request;
            _updatePartnerLocation = updatePartnerLocation;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            if (Verify_User())
            {
                if (Verify_UserIsActive())
                {
                    Update_Partner_Location();
                }
            }
        }

        private bool Verify_User()
        {
            try
            {
                if (_updatePartnerLocation.Check_If_User_Exist(request.UserId))
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
                if (_updatePartnerLocation.Check_If_User_IsActive(request.UserId))
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

        private void Update_Partner_Location()
        {
            try
            {
                _updatePartnerLocation.UpdatePartnerLocation(request);

                _messages.Add(new Message_Info { Message = "Partner locations updated successfully", Type = Message_Type.SUCCESS.ToString() });

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

            _updatePartnerLocation = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}





