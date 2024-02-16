using System;
using Business.Service.Manager.Company.UpdateBusiness;
using Business.Service.Models.Company.UpdateBusiness;
using Business.Service.Repositories.Company;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Business.Service.Controllers
{
    [Route("api/update-business")]
    [ApiController]
    public class UpdateBusinessController : BaseApiController
    {
        private IUpdateBusinessService _updateBusinessService;
        private IConfiguration _iconfiguration;
        public UpdateBusinessController(IUpdateBusinessService UpdateBusinessService, IConfiguration iconfiguration)
        {
            _updateBusinessService = UpdateBusinessService;
            _iconfiguration = iconfiguration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                using (var s = new Select_All(_updateBusinessService))
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

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var s = new Insert(request, _updateBusinessService))
                {
                    s.Process();

                    _retVal.Data = s._businessId;

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

        [HttpPut]
        public IActionResult Put([FromBody]Put_Request request)
        {
            try
            {
                using (var u = new Update(request, _updateBusinessService, _iconfiguration))
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