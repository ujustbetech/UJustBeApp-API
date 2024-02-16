using System;
using Microsoft.AspNetCore.Mvc;
using Notification.Service.Manager.FCMToken;
using Notification.Service.Models.FCMToken;
using Notification.Service.Repositories;

namespace Notification.Service.Controllers
{
    [Route("api/fcmtoken")]
    [ApiController]
    public class FCMTokenController : BaseApiController
    {
        private IFCMTokenService _fcmTokenService;

        public FCMTokenController(IFCMTokenService fcmTokenService)
        {
            _fcmTokenService = fcmTokenService;
        }

        [HttpPut("update")]
        public IActionResult Put([FromBody]Put_Request request)
        {
            try
            {
                using (var u = new Update(request, _fcmTokenService))
                {
                    u.Process();

                    _retVal.Data = null;

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
    }
}