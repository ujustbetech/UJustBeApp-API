using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Reports.Service.Manager.ReferralTracking;
using Reports.Service.Models.ReferralTracking;
using Reports.Service.Repositories.ReferralTracking;

namespace Reports.Service.Controllers
{
    [Route("api/Report/Referral")]
    [ApiController]
    public class ReferralTrackingController  : BaseApiController
    {
       private IReferralTracking _referraltracking;
        public ReferralTrackingController(IReferralTracking ReferralTracking)
        {
            _referraltracking = ReferralTracking;
        }
      
        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var s = new Select_All(_referraltracking, request))
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

      
        [HttpPut("Excel")]
        public IActionResult Put([FromBody]Put_Request request)
        {
            try
            {
                using (var s = new Put(_referraltracking, request))
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