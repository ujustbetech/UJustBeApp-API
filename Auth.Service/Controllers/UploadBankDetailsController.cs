using System;
using Auth.Service.Respositories.Registeration;
using Auth.Service.Models.Registeration.UploadBankDetails;
using Microsoft.AspNetCore.Mvc;
using Auth.Service.Manager.Registeration.UploadBankDetails;
using Microsoft.Extensions.Configuration;

namespace Auth.Service.Controllers
{
    [Route("api/upload-bank-details")]
    [ApiController]
    public class UploadBankDetailsController : BaseApiController
    {
        private IUploadBankDetailsService _uploadBankDetailsService;

        private IConfiguration _iconfiguration;


        public UploadBankDetailsController(IUploadBankDetailsService UploadBankDetailsService, IConfiguration iconfiguration
)
        {
            _uploadBankDetailsService = UploadBankDetailsService;
            _iconfiguration = iconfiguration;

        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var i = new Insert(request, _uploadBankDetailsService,_iconfiguration))
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