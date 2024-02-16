using System;
using Auth.Service.Manager.Admin.User;
using Auth.Service.Respositories.Registeration;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Service.Controllers
{
    [Route("api/admin-user-info")]
    [ApiController]
    public class AdminUserController : BaseApiController
    {
        private IUserInfoService _userInfoService;

        public AdminUserController(IUserInfoService UserInfoService)
        {
            _userInfoService = UserInfoService;
        }

        [HttpGet]
        public IActionResult Get(string userId)
        {
            try
            {
                using (var s = new Select(userId, _userInfoService))
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

        [HttpPost("list", Name = "AdminUserList")]
        public IActionResult Post(string query)
        {
            try
            {
                using (var s = new Select_All(query, _userInfoService))
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