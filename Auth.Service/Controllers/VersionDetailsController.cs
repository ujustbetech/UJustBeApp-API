using Auth.Service.Manager.MobileVersion;
using Auth.Service.Respositories.MobileVersion;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Auth.Service.Controllers
{
    [Route("api/Get-VerisonDetails")]
    [ApiController]
    public class VersionDetailsController : BaseApiController
    {
        private IGetVersionDetails _VerisonService;

        public VersionDetailsController(IGetVersionDetails VerisonService)
        {
            _VerisonService = VerisonService;
        }
        [HttpGet]
        public IActionResult Get(string Type)
        {
            try
            {
                using (var s = new Select(Type,_VerisonService))
                {
                    s.Process();

                    _retVal.Data = s._response;

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
