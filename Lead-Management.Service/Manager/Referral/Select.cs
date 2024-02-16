using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Lead_Management.Service.Models.Referral;
using Lead_Management.Service.Repositories.Referral;
using UJBHelper.Common;

namespace Lead_Management.Service.Manager.Referral
{
    public class Select : IDisposable
    {
        private string referralId;
        private IReferralService _referralService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        public Get_Request _response = null;

        public Select(string referralId, IReferralService referralService)
        {
            this.referralId = referralId;
            _referralService = referralService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            if (Verify_Referral())
            {
                Get_Referral_Details();
            }
        }

        private void Get_Referral_Details()
        {
            try
            {
                _response = _referralService.Get_Referral_Details(referralId);

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
                    Message = "Exception Occured",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.InternalServerError;
                throw;
            }
        }

        private bool Verify_Referral()
        {
            try
            {
                if (_referralService.Check_If_Lead_Exists(referralId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No Referral Found",
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
                    Message = "No Referral Found",
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
