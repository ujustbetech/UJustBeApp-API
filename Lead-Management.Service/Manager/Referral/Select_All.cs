using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Lead_Management.Service.Models.Referral;
using Lead_Management.Service.Repositories.Referral;
using UJBHelper.Common;

namespace Lead_Management.Service.Manager.Referral
{
    public class Select_All : IDisposable
    {
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        public List<Get_Request> _response = null;
        private Lookup_Request request;
        public PaginationInfo _pager;
        private IReferralService _referralService;

        public Select_All(Lookup_Request request, IReferralService referralService)
        {
            this.request = request;
            _referralService = referralService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            Search_For_Leads();
        }

        private void Search_For_Leads()
        {
            try
            {
                PaginationInfo pager = new PaginationInfo();
                pager.CurrentPage = Convert.ToInt32(request.CurrentPage);

                _response = _referralService.Search_For_Referrals(request);
                pager.TotalRecords = _response.Count;
                int pages = (pager.TotalRecords + pager.PageSize - 1) / pager.PageSize;
                pager.TotalPages = pages;

                if (pager.IsPagingRequired)
                {
                    _response = _response.Skip(pager.CurrentPage * pager.PageSize).Take(pager.PageSize).ToList();
                }

                _pager = pager;

                if (_response.Count == 0)
                {
                    _messages.Add(new Message_Info
                    {
                        Message = "No Referral Found",
                        Type = Message_Type.INFO.ToString()
                    });

                    _statusCode = HttpStatusCode.NotFound;
                }
                else
                {

                    _messages.Add(new Message_Info
                    {
                        Message = "Referral Details List",
                        Type = Message_Type.SUCCESS.ToString()
                    });

                    _statusCode = HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info
                {
                    Message = "Exception Occured",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.InternalServerError;
                throw;
            }
        }

        public void Dispose()
        {
            request = null;
            _referralService = null;
            _messages = null;
            _response = null;
        }
    }
}
