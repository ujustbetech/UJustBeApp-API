using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Reports.Service.Manager.NativeError;
using Reports.Service.Models.NativeError;
using Reports.Service.Repositories.NativeError;

namespace Reports.Service.Controllers
{
    [Route("api/error-log/insert")]
    [ApiController]
    public class NativeErrorController : BaseApiController
    {
        private INativeErrorService _nativeErrorService;
        private IConfiguration _iconfiguration;

        public NativeErrorController(INativeErrorService NativeErrorService, IConfiguration iConfiguration)
        {
            _nativeErrorService = NativeErrorService;
            _iconfiguration = iConfiguration;
        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var i = new Insert(request, _nativeErrorService,_iconfiguration))
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