using System;
using Partner.Service.Repositories.Partner;
using Microsoft.AspNetCore.Mvc;
using Partner.Service.Manager.Partner.UpdatePartnerAppVersion;
using Partner.Service.Models.Partners.UpdatePartnerAppVersion;

namespace Partner.Service.Controllers
{
    [Route("api/update-partner-appversion")]
    [ApiController]
    public class UpdatePartnerAppVersionController : BaseApiController
    {
        private IUpdatePartnerProfile _updateupdatePartnerAppVersion;

        public UpdatePartnerAppVersionController(IUpdatePartnerProfile updatePartnerAppVersion)
        {
            _updateupdatePartnerAppVersion = updatePartnerAppVersion;
        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var s = new Insert(request, _updateupdatePartnerAppVersion))
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