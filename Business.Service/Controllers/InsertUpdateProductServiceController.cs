using Business.Service.Manager.ProductServices;
using Business.Service.Models.ProductService;
using Business.Service.Repositories.ProductService;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Business.Service.Controllers
{
    [Route("api/update-product-service")]
    [ApiController]
    public class InsertUpdateProductServiceController : BaseApiController
    {
        private IAddProductService _addProductService;

        public InsertUpdateProductServiceController(IAddProductService AddProductService)
        {
            _addProductService = AddProductService;
        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var s = new Insert(request, _addProductService))
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