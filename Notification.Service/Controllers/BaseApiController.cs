using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using UJBHelper.Common;

namespace Notification.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        public HttpStatusCode _statusCode = HttpStatusCode.OK;

        public Response _retVal = null;

        public BaseApiController()
        {
            _retVal = new Response();
        }
    }

    public class Response
    {
        public dynamic Data { get; set; }

        public List<Message_Info> Message { get; set; }
    }
}