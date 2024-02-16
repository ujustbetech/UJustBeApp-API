using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Partner.Service.Models.PartnerDetails.PartnerProfile;
using Partner.Service.Repositories.PartnerDetails;
using UJBHelper.Common;

namespace Partner.Service.Manager.PartnerDetails.PartnerProfile
{
    public class Select : IDisposable
    {
        public Get_Request _response;
        public string _UserId;
        private IGetPartnerProfile _PartnerDetailsService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Select(String UserId, IGetPartnerProfile PartnerDetailsService)
        {
            _UserId = UserId;
            _PartnerDetailsService = PartnerDetailsService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            if (Check_If_User_Exist())
            {
                GetPartnerProfile();
            }
        }

        private bool Check_If_User_Exist()
        {
            try
            {
                if (_PartnerDetailsService.Check_If_User_Exist(_UserId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No Partner Found",
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
                    Message = "No Listed Partner Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }


        private void GetPartnerProfile()
        {
            try
            {
                _response = _PartnerDetailsService.GetPartnerProfile(_UserId);

                //_messages.Add(new Message_Info { Message = " List", Type = Message_Type.SUCCESS.ToString() });

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
            _UserId = null;
            _PartnerDetailsService = null;
            _response = null;
            _messages = null;
        }
    }
}
