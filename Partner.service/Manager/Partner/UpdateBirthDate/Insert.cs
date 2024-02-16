using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Partner.Service.Models.Partners.UpdateBirthDate;
using Partner.Service.Repositories.Partner;
using UJBHelper.Common;


namespace Partner.Service.Manager.Partner.UpdateBirthDate
{
    public class Insert : IDisposable
    {
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        private Post_Request request;
        private IUpdatePartnerProfile _updatePartnerProfileService;
        private IConfiguration _iconfiguration;


        public Insert(Post_Request request, IUpdatePartnerProfile updatePartnerProfileService, IConfiguration iconfiguration
)
        {
            this.request = request;
            _updatePartnerProfileService = updatePartnerProfileService;
            _messages = new List<Message_Info>();
            _iconfiguration = iconfiguration;

        }


        public void Process()
        {
            if (Verify_User())
            {
                if (Verify_UserIsActive())
                {
                    //_updatePartnerProfileService.UpdateBirthDate(request);
                }
            }
        }

        private bool Verify_User()
        {
            try
            {
                if (_updatePartnerProfileService.Check_If_User_Exist(request.userId))
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
                if (_updatePartnerProfileService.Check_If_User_IsActive(request.userId))
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


        public void Dispose()
        {
            request = null;

            _updatePartnerProfileService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }

    }
}