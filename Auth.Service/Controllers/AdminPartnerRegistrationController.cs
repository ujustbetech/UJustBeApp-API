using System;
using System.Collections.Generic;
using System;
using Auth.Service.Manager.Registeration.AdminPartnerregistration;
using Auth.Service.Models.Registeration.Register;
using Auth.Service.Respositories.Registeration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Auth.Service.Controllers
{
    [Route("api/Adminpartnerregister")]
    [ApiController]
    public class AdminPartnerRegistrationController : BaseApiController
    {

        private IRegisterService _registerService;
        private IConfiguration _iconfiguration;


        public AdminPartnerRegistrationController(IRegisterService Registerservice, IConfiguration iconfiguration)
        {
            _registerService = Registerservice;
            _iconfiguration = iconfiguration;

        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var i = new Insert(request, _registerService,_iconfiguration))
                {
                    i.ProcessAsync();

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