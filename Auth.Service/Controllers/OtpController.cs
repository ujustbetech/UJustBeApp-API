using System;
using Auth.Service.Manager.Registeration.Otp;
using Auth.Service.Models.Registeration.Otp;
using Auth.Service.Respositories.Registeration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Auth.Service.Controllers
{
    [Route("api/otp")]
    [ApiController]
    public class OtpController : BaseApiController
    {
        private IOtpService _otpService;
        private IConfiguration _iconfiguration;


        public OtpController(IOtpService Otpservice, IConfiguration iconfiguration)
        {
            _otpService = Otpservice;

            _iconfiguration = iconfiguration;

        }

        [HttpPut("resend-otp", Name = "ResendOtp")]
        public IActionResult Put([FromBody]Put_Request request)
        {
            try
            {
                using (var u = new Update(request, _otpService))
                {
                    u.Process();

                    _retVal.Data = u.new_otp;

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

        [HttpPost("validate-otp", Name = "ValidateOtp")]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var i = new Insert(request, _otpService, _iconfiguration))
                {
                    i.Process();

                    _retVal.Data = null;

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
    }
}