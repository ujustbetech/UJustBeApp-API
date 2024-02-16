using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Service.Manager.ApproveBusinessKYC;
using Business.Service.Models.ApproveBusinessKYC;
using Business.Service.Repositories.ApproveBusinessKYC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Business.Service.Controllers
{
    [Route("api/approve-business-kyc")]
    [ApiController]
    public class ApproveBusinessKYCController : BaseApiController
    {
        private IApproveBusinessKYCService _approveBusinessKYCService;

        public ApproveBusinessKYCController(IApproveBusinessKYCService approveBusinessKYCService)
        {
            _approveBusinessKYCService = approveBusinessKYCService;
        }

        [HttpPut]
        public IActionResult Put([FromBody]Put_Request request)
        {
            try
            {
                using (var u = new Update(request, _approveBusinessKYCService))
                {
                    u.Process();

                    _retVal.Data = null;

                    _retVal.Message = u._messages;

                    _statusCode = u._statusCode;
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