using System;
using Business.Service.Models.Company.Category;
using Business.Service.Repositories.Company;
using Microsoft.AspNetCore.Mvc;

namespace Business.Service.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryLookupController : BaseApiController
    {
        private ICategoryService _categoryService;

        public CategoryLookupController(ICategoryService CategoryService)
        {
            _categoryService = CategoryService;
        }

        [HttpGet("all",Name = "GetAllCategories")]
        public IActionResult Get(string query,int CurrentPage)
        {
            try
            {
                using (var s = new Select(query,CurrentPage, _categoryService))
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