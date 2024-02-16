using System;
using Microsoft.AspNetCore.Mvc;
using Reports.Service.Manager.Dashboard;
using Reports.Service.Repositories.Dashboard;

namespace Reports.Service.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : BaseApiController
    {
        private IDashboardService _dashboardService;

        public DashboardController(IDashboardService DashboardService)
        {
            _dashboardService = DashboardService;
        }

        [HttpGet("ujb-status", Name = "GetUjbStatus")]
        public IActionResult Get()
        {
            try
            {
                using (var s = new Select_All(_dashboardService))
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

        [HttpGet("partner-stats", Name = "GetPartnerStatus")]
        public IActionResult Get(string userId,string type)
        {
            try
            {
                using (var s = new Select(userId,type,_dashboardService))
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