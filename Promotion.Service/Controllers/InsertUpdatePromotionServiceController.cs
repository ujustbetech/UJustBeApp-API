using Promotion.Service.Manager.PromotionService;
using Promotion.Service.Models.Promotions;
using Promotion.Service.Repositories.PromotionService;

using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Configuration;

namespace Promotion.Service.Controllers
{
    [Route("api/add-promotion-service")]
    [ApiController]
    public class InsertUpdatePromotionServiceController : BaseApiController
    {
        private IConfiguration _iconfiguration;
        private IAddPromotionService _addPromotionService;
    

        public InsertUpdatePromotionServiceController(IConfiguration iconfiguration,IAddPromotionService AddPromotionService)
        {
            _iconfiguration = iconfiguration;
            _addPromotionService = AddPromotionService;


        }

        [HttpPost]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var s = new Insert(request, _addPromotionService,_iconfiguration))
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

      
        public IActionResult Get()
        {
            try
            {
                using (var s = new Get(_addPromotionService))
                {
                    s.Process();

                    _retVal.Data = s._response.PromotionLPList;

                   

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
