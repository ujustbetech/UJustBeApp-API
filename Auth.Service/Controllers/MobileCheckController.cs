﻿using System;
using Auth.Service.Manager.Registeration.MobileCheck;
using Auth.Service.Models.Registeration.MobileCheck;
using Auth.Service.Respositories.Registeration;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Service.Controllers
{
    [Route("api/mobile-check")]
    [ApiController]
    public class MobileCheckController : BaseApiController
    {
        private IEmailCheckService _emailCheckService;

        public MobileCheckController(IEmailCheckService EmailCheckService)
        {
            _emailCheckService = EmailCheckService;
        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var i = new Insert(request, _emailCheckService))
                {
                    i.Process();

                    _retVal.Data = null;

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