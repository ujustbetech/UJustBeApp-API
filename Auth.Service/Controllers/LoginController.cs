using System;
using Microsoft.AspNetCore.Mvc;
using Auth.Service.Manager.Login;
using Auth.Service.Models.Login;
using Auth.Service.Respositories.Login;

namespace Auth.Service.Controllers
{
    [Route("api/Login")]
    [ApiController]
    public class LoginController : BaseApiController
    {
        private ILoginService _loginService;

        public LoginController(ILoginService loginservice)
        {
            _loginService = loginservice;
        }

        [HttpPost]
        public IActionResult Login([FromBody]Post_Request request)
        {
            try
            {
                using (var i = new Insert(request,_loginService))
                {
                    i.Process();

                    _retVal.Data = i._response;

                    _retVal.Message = i._messages;

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