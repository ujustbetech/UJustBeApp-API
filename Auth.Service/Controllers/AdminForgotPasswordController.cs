using Auth.Service.Manager.Admin.ForgotPassword;
using Auth.Service.Models.Admin.ForgotPassword;
using Auth.Service.Respositories.Login;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace Auth.Service.Controllers
{
    [Route("api/admin-forgot-password")]
    [ApiController]
    public class AdminForgotPasswordController:BaseApiController
    {
        private IForgotPasswordService _forgotPasswordService;
        private IConfiguration _iconfiguration;

        public AdminForgotPasswordController(IForgotPasswordService forgotPasswordService, IConfiguration config)
        {
            _forgotPasswordService = forgotPasswordService;
            _iconfiguration = config;
        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var i = new AdminInsert(request, _forgotPasswordService, _iconfiguration))
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
