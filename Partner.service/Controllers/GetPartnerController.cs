using Microsoft.AspNetCore.Mvc;
using Partner.Service.Manager.GetPartnerService;
using Partner.Service.Repositories.GetPartnerService;
using System;

namespace Partner.Service.Controllers
{
    [Route("api/partner")]
    [ApiController]
    public class GetPartnerController:BaseApiController
    {
        private IGetPartnerService _getPartnerService;

        public GetPartnerController(IGetPartnerService GetPartnerService)
        {
            _getPartnerService = GetPartnerService;
        }

        //[HttpGet("Get-A/{size}/{page}")]
        ////[Route("geta")]
        //public string GetA(int Size,int page)
        //{
        //    return "A";
        //}

        [HttpGet("Get-partners/{size}/{page}")]
        public IActionResult Get(int size, int page)
        {
            try
            {
                using (var s = new Select(size,page, _getPartnerService))
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

