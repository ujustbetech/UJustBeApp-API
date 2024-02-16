using System;
using Auth.Service.Respositories.Registeration;
using Microsoft.AspNetCore.Mvc;
using Auth.Service.Manager.Registeration.User;
using Auth.Service.Models.Registeration.User;

namespace Auth.Service.Controllers
{
    [Route("api/user-info")]
    [ApiController]
    public class UserController : BaseApiController
    {
        private IUserInfoService _userInfoService;

        public UserController(IUserInfoService UserInfoService)
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

        [HttpPut("UpdateOtherDetails")]
        public IActionResult Put([FromBody]Put_Request request)
        {
            try
            {
                using (var i = new Update(request, _userInfoService))
                {
                    i.Process();

                    //_retVal.Data = i._response;

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