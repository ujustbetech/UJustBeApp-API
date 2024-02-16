using Partner.Service.Manager.ApproveDisapproveBPCP;
using Partner.Service.Models.ApproveDisapproveBPCP;
using Partner.Service.Repositories.ApproveDisapproveBPCP;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Configuration;

namespace Partner.Service.Controllers
{
    [Route("api/ApproveDisapprove")]
    [ApiController]
    public class ApproveDisapproveBPCPController : BaseApiController
    {
        private IApproveDisapproveBPCPService _appDisappBPCPService;
        private IConfiguration _iconfiguration;

        public ApproveDisapproveBPCPController(IApproveDisapproveBPCPService appDisappBPCPService, IConfiguration iconfiguration)
        {
            _appDisappBPCPService = appDisappBPCPService;
            _iconfiguration = iconfiguration;

        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var s = new Update(request, _appDisappBPCPService,_iconfiguration))
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
