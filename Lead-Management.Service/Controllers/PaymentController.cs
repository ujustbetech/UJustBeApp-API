using System;
using Lead_Management.Service.Manager.Payment;
using Lead_Management.Service.Models.Payment;
using Lead_Management.Service.Repositories.Payment;
using Microsoft.AspNetCore.Mvc;

namespace Lead_Management.Service.Controllers
{
    [Route("api/Payment-Details")]
    [ApiController]
    public class PaymentController:BaseApiController
    {
        private IAddPaymentService _addPaymentService;

        public PaymentController(IAddPaymentService AddPaymentService)
        {
            _addPaymentService = AddPaymentService;
        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var s = new Insert(request, _addPaymentService))
                {
                    s.Process();

                    _retVal.Data = s.TransactionId;

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

        [HttpGet("ById")]
        public IActionResult GetPaymentDetails(string PaymentId)
        {
            try
            {
                using (var s = new Select_ById(PaymentId, _addPaymentService))
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

        [HttpGet("Get-PaymentList")]
        public IActionResult Get(string LeadId)
        {
            try
            {
                using (var s = new Select(LeadId, _addPaymentService))
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


        [HttpPut("Get-Balanace")]
        public IActionResult Put([FromBody]Put_Request request)
        {
            try
            {
                using (var u = new Select_Balance(request, _addPaymentService))
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

        //[HttpPost("InsertFeeDetails")]
        //public IActionResult PostFeeDetails([FromBody]Post_Request request)
        //{
        //    try
        //    {
        //        using (var s = new Insert(request, _addPaymentService))
        //        {
        //            s.Process();

        //            _retVal.Data = null;

        //            _retVal.Message = s._messages;

        //            _statusCode = s._statusCode;
        //        }
        //        return StatusCode(Convert.ToInt32(_statusCode), _retVal);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500);
        //    }
        //}

      
    }
}