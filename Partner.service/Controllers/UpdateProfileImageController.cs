


using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Partner.Service.Manager.Partner.UpdateProfileImage;
using Partner.Service.Models.Partners.UpdateProfileImage;
using Partner.Service.Repositories.Partner;
using System;

namespace Partner.Service.Controllers
{
    [Route("api/update-profile-image")]
    [ApiController]
    public class UpdateProfileImageController : BaseApiController
    {
        private IUpdatePartnerProfile _updateProfileImageService;
        private IConfiguration _iconfiguration;

        public UpdateProfileImageController(IUpdatePartnerProfile updateProfileImageService,IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            _updateProfileImageService = updateProfileImageService;
        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var s = new Insert(request, _updateProfileImageService, _iconfiguration))
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
