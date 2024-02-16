using Auth.Service.Models.Lookup.Country;
using Auth.Service.Respositories.Lookup;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UJBHelper.Common;


namespace Auth.Service.Manager.Lookup.Country
{
    public class Select : IDisposable
    {
        public Get_Request _response;
        private string query;
        private ICountryService _countryService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Select(ICountryService countryService)
        {            
            _countryService = countryService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            Get_Country();
        }

        private void Get_Country()
        {
            try
            {
                _response = _countryService.GetCountries();

                _messages.Add(new Message_Info { Message = "Countries List", Type = Message_Type.SUCCESS.ToString() });

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
            _countryService = null;
            _response = null;
            _messages = null;
        }
    }
}


