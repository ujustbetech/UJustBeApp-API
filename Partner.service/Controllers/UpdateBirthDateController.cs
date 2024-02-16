using System;
using Partner.Service.Manager.Partner.UpdateBirthDate;
using Partner.Service.Models.Partners.UpdateBirthDate;
using Partner.Service.Repositories.Partner;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Partner.Service.Controllers
{
    [Route("api/Update-BirthDate")]
    [ApiController] 
    public class UpdateBirthDateController : BaseApiController
    {
        private IUpdatePartnerProfile _updatePartnerProfileService;
        private IConfiguration _iconfiguration;


        [HttpPost]
        
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var s = new Insert(request, _updatePartnerProfileService, _iconfiguration))
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