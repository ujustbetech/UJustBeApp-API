using System;
using Microsoft.AspNetCore.Mvc;
using Search.Service.Manager.Dashboard;
using Search.Service.Models.Dashboard;
using Search.Service.Repositories.Dashboard;

namespace Search.Service.Controllers
{
    [Route("api/search-dashboard")]
    [ApiController]
    public class DashboardController : BaseApiController
    {
        private IDashboardService _dashboardService;

        public DashboardController(IDashboardService DashboardService)
        {
            _dashboardService = DashboardService;
        }

        [HttpGet("suggestion")]
        public IActionResult Get(string query,string UserId)
        {
            try
            {
                using (var s = new Select(query,UserId, _dashboardService))
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

        [HttpPost("business")]
        public IActionResult Post([FromBody]Post_Request request)
        {
            try
            {
                using (var i = new Insert(request, _dashboardService))
                {
                    i.Process();

                    _retVal.Data = i._response;

                    _retVal.Message = i._messages;

                    _statusCode = i._statusCode;
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