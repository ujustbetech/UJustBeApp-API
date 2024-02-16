using System;
using Auth.Service.Manager.ChangePassword;
using Auth.Service.Models.ChangePassword;
using Auth.Service.Respositories.Login;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Auth.Service.Controllers
{
    [Route("api/change-password")]
    [ApiController]
    public class ChangePasswordController : BaseApiController
    {
        private IChangePasswordService _changePasswordService;
        private IConfiguration _iconfiguration;


        public ChangePasswordController(IChangePasswordService ChangePasswordService, IConfiguration iconfiguration
)
        {
            _changePasswordService = ChangePasswordService;
            _iconfiguration = iconfiguration;

        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var i = new Insert(request, _changePasswordService,_iconfiguration))
                {
                    i.Process();

                    _retVal.Data = null;

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