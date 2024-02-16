using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Promotion.Service.Models.Promotions;
using Promotion.Service.Repositories.GetPromotionService;
using UJBHelper.Common;

namespace Promotion.Service.Manager.GetPromotionService
{
    public class Select : IDisposable
    {
        public Get_Request _response;
        private int size;
        private int page;
        private string _userId;
       
        private IGetPromotionService _getPromotionService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Select(string userId,int size, int page, IGetPromotionService getPromotionService)
        {
            this.size = size;
            this.page = page;
            _userId = userId;
            _getPromotionService = getPromotionService;
            _messages = new List<Message_Info>();
        }

        
        public void Process()
        {
            Get_Promotions();
        }

        private void Get_Promotions()
        {
            try
            {
                _response = _getPromotionService.Get_PromotionList(size, page,_userId);

                _messages.Add(new Message_Info { Message = "Promotions List", Type = Message_Type.SUCCESS.ToString() });

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
            size = 0;
            page = 0;
            _userId = null;
            _getPromotionService = null;
            _response = null;
            _messages = null;
        }
    }
}
