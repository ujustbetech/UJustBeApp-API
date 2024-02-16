using System;
using Lead_Management.Service.Manager.ReferralStatus;
using Lead_Management.Service.Models.ReferralStatus;
using Lead_Management.Service.Repositories.ReferralStatus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Lead_Management.Service.Controllers
{
    [Route("api/referral-status")]
    [ApiController]
    public class ReferralStatusController : BaseApiController
    {
        private IReferralStatusService _referralStatusService;
        private IConfiguration _iconfiguration;


        public ReferralStatusController(IReferralStatusService ReferralStatusService, IConfiguration iconfiguration
)
        {
            _referralStatusService = ReferralStatusService;
            _iconfiguration = iconfiguration;

        }

        [HttpPut("update")]
        public IActionResult Put([FromBody]Put_Request request)
        {
            try
            {
                using (var u = new Update(request, _referralStatusService,_iconfiguration))
                {
                    u.Process();

                    _retVal.Data = null;

                    _retVal.Message = u._messages;

                    _statusCode = u._statusCode;
                }
                return StatusCode(Convert.ToInt32(_statusCode), _retVal);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Get-status/{statusId}")]
        public IActionResult Get(int statusId)
        {
            try
            {
                using (var s = new Select(statusId, _referralStatusService))
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

        [HttpGet("Get-Dependentstatus/{statusId}")]
        public IActionResult GetDependentStatus(int statusId)
        {
            try
            {
                using (var s = new SelectDependent(statusId, _referralStatusService))
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