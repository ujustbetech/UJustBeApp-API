using Microsoft.AspNetCore.Mvc;
using Promotion.Service.Manager.GetPromotionService;
using Promotion.Service.Repositories.GetPromotionService;
using System;

namespace Promotion.Service.Controllers
{
    [Route("api/promotion")]
    [ApiController]
    public class GetPromotionController:BaseApiController
    {
        private IGetPromotionService _getPromotionService;

        public GetPromotionController(IGetPromotionService GetPromotionService)
        {
            _getPromotionService = GetPromotionService;
        }

        [HttpGet("Get-promotion/{size}/{page}")]
        public IActionResult Get(int size, int page, string userId)
        {
            try
            {
                using (var s = new Select(userId,size,page,_getPromotionService))
                {
                    s.Process();
                    _retVal.Data = s._response.PromotionList;
                    _retVal.TotalCount = s._response.totalCount;
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
        [HttpGet("Get-PromotionDetails-ById")]
        public IActionResult Get(string PromotionId)
        {
            try
            {
                using (var s = new Select_ById(PromotionId,_getPromotionService))
                {
                    s.Process();

                    _retVal.Data = s._response;

                    _retVal.Message = s._messages;

                    _statusCode = s._statusCode;
                }
                return StatusCode(Convert.ToInt32(_statusCode), _retVal.Data._promotions);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Get-PromotionMedia/{Page}/{Size}")]
        public IActionResult Get(int Page,int Size)
        {
            try
            {
                using (var s = new Select_AllMedia(_getPromotionService,Page,Size))
                {
                    s.Process();

                    _retVal.Data = s._response.PromotionMedia;

                    _retVal.Paging = s._pager;

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

