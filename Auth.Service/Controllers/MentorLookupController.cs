using System;
using Auth.Service.Manager.Registeration.MentorLookup;
using Auth.Service.Models.Registeration.MentorList;
using Auth.Service.Respositories.Registeration;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Service.Controllers
{
    [Route("api/mentor")]
    [ApiController]
    public class MentorLookupController : BaseApiController
    {
        private IMentorLookupService _mentorLookupService;

        public MentorLookupController(IMentorLookupService MentorLookupService)
        {
            _mentorLookupService = MentorLookupService;
        }

        [HttpPost("mentor-list", Name = "GetMentorList")]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var i = new Insert(request, _mentorLookupService))
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