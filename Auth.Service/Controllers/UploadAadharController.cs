using System;
using Auth.Service.Respositories.Registeration;
using Auth.Service.Models.Registeration.UploadAadhar;
using Microsoft.AspNetCore.Mvc;
using Auth.Service.Manager.Registeration.UploadAadhar;
using Microsoft.Extensions.Configuration;

namespace Auth.Service.Controllers
{
    [Route("api/upload-aadhar")]
    [ApiController]
    public class UploadAadharController : BaseApiController
    {
        private IUploadAadharService _uploadAadharService;
        private IConfiguration _iconfiguration;


        public UploadAadharController(IUploadAadharService UploadAadharService, IConfiguration iconfiguration)

        {
            _uploadAadharService = UploadAadharService;
            _iconfiguration = iconfiguration;

        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var i = new Insert(request, _uploadAadharService,_iconfiguration))
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