using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Business.Service.Models.ClientPartner;
using Business.Service.Repositories.AddProductService;
using UJBHelper.Common;

namespace Business.Service.Manager.ClientPartner
{
    public class Select : IDisposable
    {
        private string userId;
        public Get_Request _response = null;
        private IClientPartnerService _clientPartnerService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Select(string userId, IClientPartnerService clientPartnerService)
        {
            this.userId = userId;
            _clientPartnerService = clientPartnerService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            if (Verify_Client_Partner())
            {
                Get_Client_Partner();
            }
        }

        private void Get_Client_Partner()
        {
            try
            {
                _response = _clientPartnerService.Get_Client_Partner_Details(userId);

                _messages.Add(new Message_Info
                {
                    Message = "Listed Partner Details",
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

        private bool Verify_Client_Partner()
        {
            try
            {
                if (_clientPartnerService.Check_If_Client_Partner_Exists(userId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No Listed Partner Found",
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
            userId = null;

            _response = null;

            _clientPartnerService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}
