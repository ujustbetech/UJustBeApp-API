using System;
using Auth.Service.Manager.Lookup.State;
using Auth.Service.Respositories.Lookup;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Service.Controllers
{
    [Route("api/search-state/{countryId}/{searchTerm}")]
    [ApiController]
    public class StateLookupController:BaseApiController
    {
        private IStateService _stateService;
        public StateLookupController(IStateService stateService)
        {
            _stateService = stateService;
        }

        [HttpGet]
        public IActionResult Get(int countryId,string searchTerm)
        {
            try
            {
                using (var s = new Select(countryId,searchTerm, _stateService))
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
