using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Reports.Service.Models.Dashboard;
using Reports.Service.Repositories.Dashboard;
using UJBHelper.Common;

namespace Reports.Service.Manager.Dashboard
{
    public class Select_All : IDisposable
    {
        public Get_Request _response = null;
        private IDashboardService _dashboardService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Select_All(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;

            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            Get_UJB_Status();
        }

        private void Get_UJB_Status()
        {
            try
            {
                _response = new Get_Request();

                _response.businessClosed = _dashboardService.Get_Total_Business_Closed() ; //"16089222"; //
                _response.listedPartners = _dashboardService.Get_Total_Client_Partners();

                // _response.ClientPartners = _dashboardService.Get_Total_Client_Partners();

                _response.partners = _dashboardService.Get_Total_Partners();

                _response.refAmtEarned = _dashboardService.Get_Total_Referral_Earned();//  "157040";//

                _response.refsPassed = _dashboardService.Get_Total_Referral_Passed();

                _response.guests = _dashboardService.Get_Total_Guests_Count();

                _response.amtEarnedByUjb =  _dashboardService.Get_Amount_Earned_By_UJB(); // "196300";//

                _messages.Add(new Message_Info { Message = "UJB Status Details.", Type = Message_Type.SUCCESS.ToString() });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Exception Occured", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.InternalServerError;
            }
        }

        public void Dispose()
        {
            _dashboardService = null;
            _response = null;
            _messages = null;
        }
    }
}
