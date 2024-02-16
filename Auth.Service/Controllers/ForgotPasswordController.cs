using System;
using Auth.Service.Manager.ForgotPassword;
using Auth.Service.Models.ForgotPassword;
using Auth.Service.Respositories.Login;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Auth.Service.Controllers
{
    [Route("api/forgot-password")]
    [ApiController]
    public class ForgotPasswordController : BaseApiController
    {
        private IForgotPasswordService _forgotPasswordService;

        private IConfiguration _iconfiguration;

        public ForgotPasswordController(IForgotPasswordService forgotPasswordService, IConfiguration iconfiguration
)
        {
            _forgotPasswordService = forgotPasswordService;
            _iconfiguration = iconfiguration;
        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var i = new Insert(request, _forgotPasswordService,_iconfiguration))
                {
                   i.Process();

                    _retVal.Data = i.new_password;

                    _retVal.Message = i._message;

                    _statusCode = i._statusCode;
                }
                return StatusCode(Convert.ToInt32(_statusCode), _retVal);
            }
            catch (Exception ex)
            {
                //return StatusCode(500, new { Message = "Internal Server Error", Type = "EXCEPTION" });
                return StatusCode(500);
            }
        }
    }
}