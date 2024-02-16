using System;
using Business.Service.Manager.Company.UpdateCompanyAddress;
using Business.Service.Models.Company.UpdateCompanyAddress;
using Business.Service.Repositories.Company;
using Microsoft.AspNetCore.Mvc;

namespace Business.Service.Controllers
{
    [Route("api/update-company-address")]
    [ApiController]
    public class UpdateCompanyAddressController : BaseApiController
    {
        private IUpdateCompanyService _updateCompanyService;

        public UpdateCompanyAddressController(IUpdateCompanyService UpdateCompanyService)
        {
            _updateCompanyService = UpdateCompanyService;
        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var s = new Insert(request, _updateCompanyService))
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