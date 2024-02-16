using Lead_Management.Service.Manager.Referral;
using Lead_Management.Service.Models.Referral;
using Lead_Management.Service.Repositories.Referral;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace Lead_Management.Service.Controllers
{
    [Route("api/referral")]
    [ApiController]
    public class ReferralController : BaseApiController
    {
        private IReferralService _referralService;

        private IConfiguration _iconfiguration;

        public ReferralController(IReferralService ReferralService, IConfiguration iconfiguration)
        {
            _referralService = ReferralService;
            _iconfiguration = iconfiguration;

        }

        [HttpGet("details")]
        public IActionResult Get(string referralId)
        {
            try
            {
                using (var s = new Select(referralId, _referralService))
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

        [HttpPost("lookup")]
        public IActionResult Post([FromBody]Lookup_Request request)
        {
            try
            {
                using (var s = new Select_All(request, _referralService))
                {
                    s.Process();

                    _retVal.Data = s._response;

                    _retVal.Paging = s._pager;

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

        [HttpPost("create")]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var i = new Insert(request, _referralService,_iconfiguration))
                {
                    i.Process();

                    _retVal.Data = i._referralId;

                    _retVal.Message = i._messages;

                    _statusCode = i._statusCode;
                }
                return StatusCode(Convert.ToInt32(_statusCode), _retVal);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPut("updateDealValue")]
        public IActionResult Put([FromBody]DealValue_Put request)
        {
            try
            {
                using (var u = new UpdateDealValue(request, _referralService))
                {
                    u.Process();

                    _retVal.Data = u._response;

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

        [HttpPut("update")]
        public IActionResult Put([FromBody]Put_Request request)
        {
            try
            {
                using (var u = new Update(request, _referralService,_iconfiguration))
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

        [HttpGet("GetProductDetails/{LeadId}")]
        
        public IActionResult GetProductDetails(string LeadId)
        {
            try
            {
                using (var s = new Get(LeadId, _referralService))
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