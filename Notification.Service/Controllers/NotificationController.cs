using Microsoft.AspNetCore.Mvc;
using Notification.Service.Manager;
using Notification.Service.Models;
using Notification.Service.Repositories;
using System;

namespace Notification.Service.Controllers
{
    [Route("api/notification")]
    [ApiController]
    public class NotificationController : BaseApiController
    {
        private INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("list")]
        public IActionResult Get([FromBody]Post_Request request)
        {
            try
            {
                using (var s = new Select_All(request, _notificationService))
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

        [HttpPut("update")]
        public IActionResult Put([FromBody]Put_Request request)
        {
            try
            {
                using (var u = new Update(request, _notificationService))
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