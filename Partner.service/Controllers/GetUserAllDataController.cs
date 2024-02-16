using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Partner.Service.Manager.GetUserAllData;
using Partner.Service.Repositories.GetPartnerService;

namespace Partner.Service.Controllers
{
    [Route("api/GetAllUserDetails")]
    [ApiController]
    public class GetUserAllDataController : BaseApiController
    {
        private IGetPartnerService _getPartnerService;

        public GetUserAllDataController(IGetPartnerService GetPartnerService)
        {
            _getPartnerService = GetPartnerService;
        }

        public IActionResult Get()
        {
            try
            {
                using (var s = new Select(_getPartnerService))
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