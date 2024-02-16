

using LegalAgreement.Service.Manager.AgreementStatus;
using LegalAgreement.Service.Models.AgreementStatus;
using LegalAgreement.Service.Repositories.AgreementStatus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LegalAgreement.Service.Controllers
{
    [Route("api/agreement-status")]
    [ApiController]
    public class AgreementStatusController : BaseApiController
    {

        private IAgreementStatusService _agreementStatusService;
        private IConfiguration _iconfiguration;

        public AgreementStatusController(IAgreementStatusService AgreementStatusService, IConfiguration iconfiguration)
        {
            _agreementStatusService = AgreementStatusService;
            _iconfiguration = iconfiguration;
        }

        [HttpPut("Partner-agreement-update")]
        public IActionResult Put([FromBody]Put_Request request)
        {
            try
            {
                using (var u = new PartnerUpdate(request, _agreementStatusService, _iconfiguration))
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

        [HttpPut("ListedPartner-agreement-update")]
        public IActionResult ListedPut([FromBody]Put_Request request)
        {
            try
            {
                using (var u = new ListedPartnerUpdate(request, _agreementStatusService, _iconfiguration))
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
