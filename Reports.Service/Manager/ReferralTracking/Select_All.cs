using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UJBHelper.Common;
using Reports.Service.Models.ReferralTracking;
using Reports.Service.Repositories.ReferralTracking;
using System.Reflection;


namespace Reports.Service.Manager.ReferralTracking
{
    public class Select_All : IDisposable
    {
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        public Get_Request _response = null;
        private Post_Request request;
        public PaginationInfo _pager;
        private IReferralTracking _ReferralTracking;

        public Select_All( IReferralTracking referralTracking, Post_Request request)
        {
           
            _ReferralTracking = referralTracking;
            _messages = new List<Message_Info>();
            this.request = request;
        }

        public void Process()
        {
            Get_Referral_Details();
        }

        private void Get_Referral_Details()
        {
            try
            {
                _response = _ReferralTracking.Get_Referral_Details(request);
                _messages.Add(new Message_Info
                {
                    Message = "Referral Details",
                    Type = Message_Type.SUCCESS.ToString()
                });

                _statusCode = HttpStatusCode.OK;

            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info
                {
                    Message = "Unable to FETCH Referral Details",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

            }
        }

        public void Dispose()
        {
            request = null;
            _ReferralTracking = null;
            _messages = null;
            _response = null;
        }

    }
}
