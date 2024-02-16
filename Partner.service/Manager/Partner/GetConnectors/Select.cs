using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Partner.Service.Models.Partners.GetConnectors;
using Partner.Service.Repositories.GetPartnerService;
using UJBHelper.Common;

namespace Partner.Service.Manager.Partner.GetConnectors
{
    public class Select : IDisposable
    {
        public Get_Connector_Request _response;
        public string _UserId;
        private IGetPartnerService _PartnerDetailsService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Select(String UserId, IGetPartnerService PartnerDetailsService)
        {
            _UserId = UserId;
            _PartnerDetailsService = PartnerDetailsService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {

            if (Verify_User())
            {
                if (Verify_UserIsActive())
                {
                    GetConnectorList();
                }
            }
        }
            
        

        private bool Verify_User()
        {
            try
            {
                if (_PartnerDetailsService.Check_If_User_Exist(_UserId))
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
                if (_PartnerDetailsService.Check_If_User_IsActive(_UserId))
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

        private void GetConnectorList()
        {
            try
            {
                _response = _PartnerDetailsService.GetConnectorList(_UserId);

                _messages.Add(new Message_Info { Message = " Connector List", Type = Message_Type.SUCCESS.ToString() });

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


    
