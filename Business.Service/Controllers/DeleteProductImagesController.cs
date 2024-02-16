using System;
using Business.Service.Manager.DeleteProductservice;
using Business.Service.Models.DeleteProductService;
using Business.Service.Repositories.DeleteProductService;
using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;


namespace Business.Service.Controllers
{
    [Route("api/Delete-product")]
    [ApiController]

    public class DeleteProductImagesController :BaseApiController
    {
        private IDeleteProductService _deleteProductService;
        private IConfiguration _iconfiguration;
        public DeleteProductImagesController(IDeleteProductService DeleteProductService, IConfiguration iconfiguration)
        {
            _deleteProductService = DeleteProductService;
            _iconfiguration = iconfiguration;
        }

         [HttpPost("Images")]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var u = new DeleteProductImages(request, _deleteProductService, _iconfiguration))
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

        [HttpGet("ProductService")]
        public IActionResult Get(string ProdServiceId)
        {
            try
            {
                using (var u = new DeleteProductService(ProdServiceId, _deleteProductService))
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
        [HttpGet("ProductServiceDetails")]
        public IActionResult ProductServiceDetails(string ProdServicedetailId)
        {
            try
            {
                using (var u = new DeleteProductServiceDetails(ProdServicedetailId, _deleteProductService))
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