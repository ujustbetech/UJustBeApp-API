using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Partner.Service.Models.Partners.UpdateAddress;
using Partner.Service.Repositories.Partner;
using UJBHelper.Common;


namespace Partner.Service.Manager.Partner.UpdateAddress
{
    public class Insert : IDisposable
    {
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        private Post_Request request;
        private IUpdatePartnerProfile _updatePartnerAddress;

        public Insert(Post_Request request, IUpdatePartnerProfile updatePartnerAddress)
        {
            this.request = request;
            _updatePartnerAddress = updatePartnerAddress;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            if (Verify_User())
            {
                if (Verify_UserIsActive())
                {
                    Update_Partner_Address();
                }
            }
        }

        private bool Verify_User()
        {
            try
            {
                if (_updatePartnerAddress.Check_If_User_Exist(request.UserId))
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
                if (_updatePartnerAddress.Check_If_User_IsActive(request.UserId))
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

        private void Update_Partner_Address()
        {
            try
            {
                _updatePartnerAddress.UpdatePartnerAddress(request);

                _messages.Add(new Message_Info { Message = "Partner address updated successfully", Type = Message_Type.SUCCESS.ToString() });

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

            _updatePartnerAddress = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}