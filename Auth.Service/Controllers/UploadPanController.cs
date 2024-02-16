using System;
using Auth.Service.Manager.Registeration.UploadPan;
using Auth.Service.Models.Registeration.UploadPan;
using Auth.Service.Respositories.Registeration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Auth.Service.Controllers
{
    [Route("api/upload-pan")]
    [ApiController]
    public class UploadPanController : BaseApiController
    {
        private IUploadPanService _uploadPanService;
        private IConfiguration _iconfiguration;
        public UploadPanController(IUploadPanService UploadPanService, IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            _uploadPanService = UploadPanService;
        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var i = new Insert(request, _uploadPanService, _iconfiguration))
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