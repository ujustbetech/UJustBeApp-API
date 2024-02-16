using System;
using Business.Service.Manager.Company.UpdateCompanyLogo;
using Business.Service.Models.Company.UpdateCompanyLogo;
using Business.Service.Repositories.Company;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Business.Service.Controllers
{
    [Route("api/update-company-logo")]
    [ApiController]
    public class UpdateCompanyLogoController : BaseApiController
    {
        private IUpdateCompanyService _updateCompanyService;
        private IConfiguration _iconfiguration;

        public UpdateCompanyLogoController(IUpdateCompanyService UpdateCompanyService, IConfiguration iconfiguration)
        {
            _updateCompanyService = UpdateCompanyService;
            _iconfiguration = iconfiguration;
        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var s = new Insert(request, _updateCompanyService, _iconfiguration))
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