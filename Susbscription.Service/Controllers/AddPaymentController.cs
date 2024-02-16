using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Susbscription.Service.Manager.FeePayment;
using Susbscription.Service.Models.FeePayment;
using Susbscription.Service.Repositories.FeePayment;

namespace Susbscription.Service.Controllers
{
    [Route("api/add-Fee-service")]

    [ApiController]
    public class AddPaymentController : BaseApiController
    {
       
        private IFeePaymentService _addPaymentService;
        private IConfiguration _iconfiguration;

        public AddPaymentController(IFeePaymentService AddPaymentService, IConfiguration iconfiguration
)
        {
            _addPaymentService = AddPaymentService;
            _iconfiguration = iconfiguration;

        }

        [HttpGet("Get-FeeType")]
        public IActionResult Get(string UserId)
        {
            try
            {
                using (var s = new Select_FeeType(UserId, _addPaymentService))
                {
                    s.Process();

                    _retVal.Data = s._response;

                    _retVal.Message = s._messages;

                    _statusCode = s._statusCode;
                }
                return StatusCode(Convert.ToInt32(_statusCode), _retVal.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost("AddPaymentDetails")]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var s = new Insert(request, _addPaymentService,_iconfiguration))
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

        [HttpGet("Get-FeeBreakup")]
        public IActionResult Get(string UserId,string FeeType)
        {
            try
            {
                using (var s = new Select_FeeBreakup(UserId, FeeType, _addPaymentService))
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

        [HttpPost("AdjustRegisterationFee")]
        public IActionResult PostFee([FromBody]Post_RegisterationFee request)
        {
            try
            {
                using (var s = new Insert_RegisterationFee(request, _addPaymentService))
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