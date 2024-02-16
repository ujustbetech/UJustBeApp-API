using Microsoft.AspNetCore.Mvc;
using Partner.Service.Manager.Partner.GetConnectors;
using Partner.Service.Repositories.GetPartnerService;
using System;

namespace Partner.Service.Controllers
{
    [Route("api/GetConnectors")]
    [ApiController]
    public class GetConnectorListController : BaseApiController
    {
        private IGetPartnerService _partnerDetailsService;

        public GetConnectorListController(IGetPartnerService PartnerDetailsService)
        {
            _partnerDetailsService = PartnerDetailsService;
        }

        [HttpGet]
        public IActionResult GetConnectors(string UserId)
        {
            try
            {
                using (var s = new Select(UserId, _partnerDetailsService))
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
