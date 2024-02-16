using System;
using Business.Service.Manager.ClientPartner;
using Business.Service.Repositories.AddProductService;
using Microsoft.AspNetCore.Mvc;

namespace Business.Service.Controllers
{
    [Route("api/client-partner")]
    [ApiController]
    public class GetClientPartnerController : BaseApiController
    {
        private IClientPartnerService _clientPartnerService;

        public GetClientPartnerController(IClientPartnerService ClientPartnerService)
        {
            _clientPartnerService = ClientPartnerService;
        }

        [HttpGet("details")]
        public IActionResult Get(string userId)
        {
            try
            {
                using (var s = new Select(userId, _clientPartnerService))
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