using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Promotion.Service.Models.Promotions;
using Promotion.Service.Repositories.PromotionService;
using UJBHelper.Common;


namespace Promotion.Service.Manager.PromotionService
{
    public class Get:IDisposable
    {
        public Get_Request _response;

        private IAddPromotionService _addPromotionService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        public PaginationInfo _pager;
        int _page; int _size;

        public Get(IAddPromotionService addPromotionService)
        {

            _addPromotionService = addPromotionService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            Get_PromotionListedPartner();
        }
        

        private void Get_PromotionListedPartner()
        {
            try
            {
               
                _response = _addPromotionService.Get_PromotionListedPartnerList();
                //_response.PromotionMedia = ShuffleList(_response.PromotionMedia);
                _messages.Add(new Message_Info { Message = "Promotion Listed partner List", Type = Message_Type.SUCCESS.ToString() });

                _statusCode = HttpStatusCode.OK;

            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Exception Occured", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.InternalServerError;
                throw;
            }

        }

        public void Dispose()
        {
            _addPromotionService = null;
            _response = null;
            _messages = null;
        }
    }
}
