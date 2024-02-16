using System;
using Auth.Service.Manager.Registeration.EnrollPartner;
using Auth.Service.Models.Registeration.EnrollPartner;
using Auth.Service.Respositories.Registeration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Auth.Service.Controllers
{
    [Route("api/enroll-partner")]
    [ApiController]
    public class EnrollPartnerController : BaseApiController
    {
        private IEnrollPartnerService _enrollPartnerService;
        private IConfiguration _iconfiguration;

        public EnrollPartnerController(IEnrollPartnerService EnrollPartnerService, IConfiguration iconfiguration)
        {
            _enrollPartnerService = EnrollPartnerService;
            _iconfiguration = iconfiguration;
        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var i = new Insert(request, _enrollPartnerService, _iconfiguration))
                {
                    i.Process();

                    _retVal.Data = i.new_role;

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


        [HttpPost("Details")]
        public IActionResult EnrollPartner([FromBody]Post_EnrollPartner request)
        {
            try
            {
                using (var i = new Insert_EnrollPartner(request, _enrollPartnerService, _iconfiguration))
                {
                    i.Process();

                    _retVal.Data = i.new_role;

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