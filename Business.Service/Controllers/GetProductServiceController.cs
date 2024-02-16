using System;
using Business.Service.Manager.ProductServices;
using Business.Service.Repositories.ProductService;
using Microsoft.AspNetCore.Mvc;

namespace Business.Service.Controllers
{
    [Route("api/product-service")]
    [ApiController]
    public class GetProductServiceController : BaseApiController
    {
        private IAddProductService _addProductService;

        public GetProductServiceController(IAddProductService AddProductService)
        {
            _addProductService = AddProductService;
        }

        [HttpGet("all")]
        public IActionResult Get(string userId, string type)
        {
            try
            {
                using (var s = new Select_All(userId, type, _addProductService))
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

        [HttpGet("details")]
        public IActionResult Get(string prodServiceId)
        {
            try
            {
                using (var s = new Select(prodServiceId, _addProductService))
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