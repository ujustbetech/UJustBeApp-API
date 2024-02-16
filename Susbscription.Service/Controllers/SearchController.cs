using System;
using Microsoft.AspNetCore.Mvc;
using Susbscription.Service.Manager.UserDetails;
using Susbscription.Service.Repositories.FeePayment;

namespace Susbscription.Service.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class SearchController : BaseApiController
    {
        private IFeePaymentService _addPaymentService;

        public SearchController(IFeePaymentService AddPaymentService)
        {
            _addPaymentService = AddPaymentService;
        }
        [HttpGet("Get-UserAutocomplete")]
        public IActionResult Get_User(string query)
        {
            try
            {
                using (var s = new Select(query, _addPaymentService))
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

        [HttpGet("Details")]
        public IActionResult Get_UserDetails(string UserId)
        {
            try
            {
                using (var s = new Select_OtherDetails(UserId, _addPaymentService))
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