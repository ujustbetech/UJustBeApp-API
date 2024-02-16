using System;
using Partner.Service.Repositories.Partner;
using Microsoft.AspNetCore.Mvc;
using Partner.Service.Manager.Partner.UpdateAddress;
using Partner.Service.Models.Partners.UpdateAddress;

namespace Partner.Service.Controllers
{
    [Route("api/update-partner-address")]
    [ApiController]
    public class UpdatePartnerAddressController:BaseApiController
    {
        private IUpdatePartnerProfile _updatePartnerAddress;

        public UpdatePartnerAddressController(IUpdatePartnerProfile updatePartnerAddress)
        {
            _updatePartnerAddress = updatePartnerAddress;
        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var s = new Insert(request, _updatePartnerAddress))
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
