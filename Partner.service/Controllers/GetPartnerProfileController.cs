using Microsoft.AspNetCore.Mvc;

using Partner.Service.Manager.PartnerDetails.PartnerProfile;
using Partner.Service.Repositories.PartnerDetails;
using System;

namespace Partner.Service.Controllers
{
    [Route("api/GetPartnerDetails")]
    [ApiController]
    public class GetPartnerProfileController : BaseApiController
    {
        private IGetPartnerProfile _partnerDetailsService;
        public GetPartnerProfileController(IGetPartnerProfile PartnerDetailsService)
        {
            _partnerDetailsService = PartnerDetailsService;
        }

       
        [HttpGet("Partner-Profile")]
        public IActionResult PartnerProfile(string UserId)
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


        [HttpGet("Partner-Active")]
        public IActionResult Get(string UserId)
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