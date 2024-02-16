using System;
using Business.Service.Manager.ProductServices;
using Business.Service.Models.ProductService;
using Business.Service.Repositories.ProductService;
using Microsoft.AspNetCore.Mvc;

namespace Business.Service.Controllers
{
    [Route("api/update-product-image")]
    [ApiController]
    public class InsertUpdateProductImagesController : BaseApiController
    {
        private IAddProductService _addProductService;

        public InsertUpdateProductImagesController(IAddProductService AddProductService)
        {
            _addProductService = AddProductService;
        }

        [HttpPost]
        public IActionResult Post([FromBody]Put_Request request)
        {
            try
            {
                using (var u = new Update(request, _addProductService))
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