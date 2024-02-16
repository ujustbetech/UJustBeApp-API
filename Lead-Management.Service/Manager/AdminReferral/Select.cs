using Lead_Management.Service.Models.AdminReferral;
using Lead_Management.Service.Repositories.Referral;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UJBHelper.Common;

namespace Lead_Management.Service.Manager.AdminReferral
{
    public class Select : IDisposable
    {
        private string referralId;
        private IAdminReferralService _referralService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        public Get_Request _response = null;
        private string userId;

        public Select(string userId, IAdminReferralService referralService)
        {
            this.userId = userId;
            _referralService = referralService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            if (Verify_User())
            {
                Get_Referral_Details();
            }
        }

        private void Get_Referral_Details()
        {
            try
            {
                _response = _referralService.Get_Referral_Details(userId);
                _messages.Add(new Message_Info
                {
                    Message = "Referral Details",
                    Type = Message_Type.SUCCESS.ToString()
                });

                _statusCode = HttpStatusCode.OK;

            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info
                {
                    Message = "Unable to FETCH Referral Details",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

            }
        }

        private bool Verify_User()
        {
            try
            {
                if (_referralService.Check_If_User_Exists(userId))
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

        public void Dispose()
        {
            _messages = null;
            _referralService = null;
            _response = null;
            _statusCode = HttpStatusCode.OK;
            referralId = null;
        }
    }
}
