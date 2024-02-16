using System;
using Lead_Management.Service.Manager.AdminReferral;
using Lead_Management.Service.Repositories.Referral;
using Microsoft.AspNetCore.Mvc;

namespace Lead_Management.Service.Controllers
{
    [Route("api/referral-list")]
    [ApiController]
    public class AdminReferralController : BaseApiController
    {
        private IAdminReferralService _referralService;

        public AdminReferralController(IAdminReferralService ReferralService)
        {
            _referralService = ReferralService;
        }

        public IActionResult Get(string userId)
        {
            try
            {
                using (var s = new Select(userId, _referralService))
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