using Auth.Service.Manager.Lookup.Country;
using Auth.Service.Respositories.Lookup;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Auth.Service.Controllers
{
    [Route("api/Get-countries")]
    [ApiController]
    public class CountryLookupController:BaseApiController
    {
        private ICountryService  _countryService;

        public CountryLookupController(ICountryService CountryService)
        {
            _countryService = CountryService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                using (var s = new Select(_countryService))
                {
                    s.Process();

                    _retVal.Data = s._response;

                    _retVal.Message = s._messages;

                    _statusCode = s._statusCode;
                }
                return StatusCode(Convert.ToInt32(_statusCode), _retVal);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
