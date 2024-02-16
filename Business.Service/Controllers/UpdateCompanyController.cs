using System;
using Business.Service.Controllers;
using Business.Service.Manager.Company.UpdateCompany;
using Business.Service.Models.Company.UpdateCompany;
using Business.Service.Repositories.Company;
using Microsoft.AspNetCore.Mvc;

namespace Company.Service.Controllers
{
    [Route("api/update-company")]
    [ApiController]
    public class UpdateCompanyController : BaseApiController
    {
        private IUpdateCompanyService _updateCompanyService;

        public UpdateCompanyController(IUpdateCompanyService UpdateCompanyService)
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