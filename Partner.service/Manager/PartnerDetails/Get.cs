using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Partner.Service.Models.PartnerDetails;
using Partner.Service.Repositories.PartnerDetails;
using UJBHelper.Common;

namespace Partner.Service.Manager.PartnerDetails
{
    public class Get : IDisposable
    {
        private Response _response;
        public string _UserId;
        private IGetPartnerDetails _PartnerDetailsService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Get(String UserId, IGetPartnerDetails PartnerDetailsService)
        {
            _UserId = UserId;
            _PartnerDetailsService = PartnerDetailsService;
            _messages = new List<Message_Info>();
            _response = new Response();
        }

        public void Process()
        {
            if (Check_If_User_IsActive())
            {
                _response.Is_Active = true;
            }
            else
            {
                _response.Is_Active = false;
            }
        }

        private bool Check_If_User_IsActive()
        {
            try
            {
                if (_PartnerDetailsService.Check_If_User_IsActive(_UserId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No  Partner Found",
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


   
        public void Dispose()
        {
            _UserId = null;
            _PartnerDetailsService = null;
            _response = null;
            _messages = null;
        }
    }
}
class Response{
  public Boolean Is_Active { get; set; }
}

