using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Promotion.Service.Models.Promotions;
using Promotion.Service.Repositories.GetPromotionService;
using UJBHelper.Common;


namespace Promotion.Service.Manager.GetPromotionService
{
    public class Select_AllMedia:IDisposable
    {
        public Get_Request _response;

        private IGetPromotionService _getPromotionService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        public PaginationInfo _pager;
        int _page; int _size;

        public Select_AllMedia(IGetPromotionService getPromotionService,int Page,int Size)
        {
            _page = Page;
            _size = Size;
            _getPromotionService = getPromotionService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            Get_PromotionMedia();
        }
        

        private void Get_PromotionMedia()
        {
            try
            {
                PaginationInfo Pager = new PaginationInfo();

                Pager.CurrentPage = Convert.ToInt32(_page);
                Pager.PageSize = Convert.ToInt32(_size);

                _response = _getPromotionService.Get_PromotionMedia();

                Pager.TotalRecords = _response.totalCount;
                int pages = (Pager.TotalRecords + Pager.PageSize - 1) / Pager.PageSize;
                Pager.TotalPages = pages;

                if (Pager.IsPagingRequired)
                {
                    _response.PromotionMedia = _response.PromotionMedia.Skip(Pager.CurrentPage * Pager.PageSize).Take(Pager.PageSize).ToList();
                }

                _pager = Pager;

                //_response.PromotionMedia = ShuffleList(_response.PromotionMedia);
                _messages.Add(new Message_Info { Message = "Promotion Media List", Type = Message_Type.SUCCESS.ToString() });

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
            _getPromotionService = null;
            _response = null;
            _messages = null;
        }
    }
}
