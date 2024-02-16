using Auth.Service.Models.Lookup.State;
using Auth.Service.Respositories.Lookup;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UJBHelper.Common;

namespace Auth.Service.Manager.Lookup.State
{
    public class Select : IDisposable
    {
        public List<Get_Request> _response = null;
        private string searchTerm;
        private int countryId;
        private IStateService _stateService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Select(int country,string searchTerm, IStateService stateService)
        {
            this.searchTerm = searchTerm;
            this.countryId = country;
            _stateService = stateService;
            _messages = new List<Message_Info>();
        }


        internal void Process()
        {
            try
            {
                _response = _stateService.Get_State_Suggestion(countryId,searchTerm);

                if (_response.Count > 0)
                {
                    _messages.Add(new Message_Info { Message = "State list", Type = Message_Type.SUCCESS.ToString() });

                    _statusCode = HttpStatusCode.OK;
                }
                else
                {
                    _messages.Add(new Message_Info { Message = "No State Found", Type = Message_Type.INFO.ToString() });

                    _statusCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Exception Occured", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.InternalServerError;
            }

        }

        public void Dispose()
        {
            _stateService = null;
            searchTerm = null;
            _messages = null;
            _statusCode = HttpStatusCode.OK;
            _response = null;
        }
    }
}
