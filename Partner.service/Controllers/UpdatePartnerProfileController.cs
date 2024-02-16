using System;
using Partner.Service.Manager.Partner.UpdateProfile;
using Partner.Service.Models.Partners.UpdateProfile;
using Partner.Service.Repositories.Partner;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Partner.Service.Controllers
{
    [Route("api/update-partner-profile")]
    [ApiController]
    public class UpdatePartnerProfileController : BaseApiController
    {
        private IUpdatePartnerProfile _updatePartnerProfileService;
        private IConfiguration _iconfiguration;

        public UpdatePartnerProfileController(IUpdatePartnerProfile updatePartnerProfileService, IConfiguration iconfiguration
)
        {
            _updatePartnerProfileService = updatePartnerProfileService;
            _iconfiguration = iconfiguration;

        }

        [HttpGet("Update-MobileNo")]
        public IActionResult Get(string UserId,string MobileNo,string CountryCode)
        {
            try
            {
                using (var s = new Select(UserId, MobileNo, CountryCode,_updatePartnerProfileService))
                {
                    s.Process();

                    _retVal.Data = s.new_otp;

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

        [HttpPost("UpdateProfile")]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var s = new Insert(request, _updatePartnerProfileService,_iconfiguration))
                {
                    s.Process();

                    _retVal.Data = null;

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
