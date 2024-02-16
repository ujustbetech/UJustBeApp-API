using System;
using Auth.Service.Manager.Admin.Login;
using Auth.Service.Models.Login;
using Auth.Service.Respositories.Login;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Service.Controllers
{
    [Route("api/admin-login")]
    [ApiController]
    public class AdminLoginController : BaseApiController
    {
        private ILoginService _loginService;

        public AdminLoginController(ILoginService loginservice)
        {
            _loginService = loginservice;
        }

        [HttpPost]
        public IActionResult Login([FromBody]Post_Request request)
        {
            try
            {
                using (var i = new Insert(request, _loginService))
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
                return StatusCode(500);
            }
        }
    }
}