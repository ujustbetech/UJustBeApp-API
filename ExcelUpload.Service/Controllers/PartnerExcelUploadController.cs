using System;
using ExcelUpload.Service.Manager.PartnerUpload;
using ExcelUpload.Service.Model.PartnerUpload;
using ExcelUpload.Service.Repositories.PartnerUpload;
using Microsoft.AspNetCore.Mvc;


namespace ExcelUpload.Service.Controllers
{
    [Route("api/upload-Excel")]
    [ApiController]
    public class PartnerExcelUploadController : BaseApiController
    {
        private IUploadPartnerDetails _uploadPartnerService;

        public PartnerExcelUploadController(IUploadPartnerDetails UploadPartnerService)
        {
            _uploadPartnerService = UploadPartnerService;
            
        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var s = new Upload(request, _uploadPartnerService))
                {
                    s.Process();

                     //_retVal.Data = s.JSONValue;
                
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
