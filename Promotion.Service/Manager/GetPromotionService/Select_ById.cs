using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Promotion.Service.Models.Promotions;
using Promotion.Service.Repositories.GetPromotionService;
using UJBHelper.Common;

namespace Promotion.Service.Manager.GetPromotionService
{
    public class Select_ById : IDisposable
    {
        public Get_Request _response;        
        private string _PromotionId;
        private IGetPromotionService _getPromotionService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Select_ById(string PromotionId, IGetPromotionService getPromotionService)
        {
            _PromotionId = PromotionId;
            _getPromotionService = getPromotionService;
            _messages = new List<Message_Info>();
        }
        public void Process()
        {
            Get_PromotionsDetails();
        }
        private void Get_PromotionsDetails()
        {
            try
            {
                _response = _getPromotionService.Get_PromotionsDetails(_PromotionId);

                _messages.Add(new Message_Info { Message = "Promotions Details", Type = Message_Type.SUCCESS.ToString() });

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
            _PromotionId = null;
            _getPromotionService = null;
            _response = null;
            _messages = null;
        }
    }
}

   