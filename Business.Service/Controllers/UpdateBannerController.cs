using Business.Service.Manager.Company.UpdateBusiness.UpdateBanner;
using Business.Service.Models.Company.UpdateBusiness.UpdateBanner;
using Business.Service.Repositories.Company;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Configuration;

namespace Business.Service.Controllers
{
    [Route("api/update-business-banner")]
    [ApiController]
    public class UpdateBannerController:BaseApiController
    {
        private IConfiguration _iconfiguration;
        private IUpdateBusinessService _updateBusinessService;

        public UpdateBannerController(IConfiguration iconfiguration,IUpdateBusinessService UpdateBusinessService)
        {
            _iconfiguration = iconfiguration;
            _updateBusinessService = UpdateBusinessService;
        }

        [HttpPut]
        public IActionResult Post([FromBody]Put_Request request)
        {
            try
            {
                using (var s = new Update(request, _updateBusinessService,_iconfiguration))
                {
                    s.Process();

                    _retVal.Data = null;

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
