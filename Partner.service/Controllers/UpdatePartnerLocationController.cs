using System;
using Partner.Service.Repositories.Partner;
using Microsoft.AspNetCore.Mvc;
using Partner.Service.Manager.Partner.UpdateLocation;
using Partner.Service.Models.Partners.UpdateLocation;
namespace Partner.Service.Controllers
{
    [Route("api/update-partner-locations")]
    [ApiController]
    public class UpdatePartnerLocationController:BaseApiController
    {
        private IUpdatePartnerProfile _updatePartnerLocation;

        public UpdatePartnerLocationController(IUpdatePartnerProfile updatePartnerLocation)
        {
            _updatePartnerLocation = updatePartnerLocation;
        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var s = new Insert(request, _updatePartnerLocation))
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

