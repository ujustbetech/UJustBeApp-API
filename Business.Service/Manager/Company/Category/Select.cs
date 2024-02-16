using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Business.Service.Repositories.Company;
using UJBHelper.Common;

namespace Business.Service.Models.Company.Category
{
    public class Select : IDisposable
    {
        public Get_Request _response;
        private string query;
        private ICategoryService _categoryService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        private int CurrentPage;
        public Select( string query, int CurrentPage, ICategoryService categoryService)
        {
            this.query = query;
            _categoryService = categoryService;
            _messages = new List<Message_Info>();
            this.CurrentPage = CurrentPage;
        }

        public void Process()
        {
            Get_Categories();
        }

        private void Get_Categories()
        {
            try
            {


                PaginationInfo pager = new PaginationInfo();
                pager.CurrentPage = Convert.ToInt32(CurrentPage);

                pager.PageSize = 30;
                _response = _categoryService.Get_Categories(query, pager);
                if (CurrentPage > 0)
                { 
                pager.TotalRecords = _response.totalCount;
                int pages = (pager.TotalRecords + pager.PageSize - 1) / pager.PageSize;
                pager.TotalPages = pages;
            }else
                {
                    pager.TotalRecords = _response.totalCount;
                    pager.PageSize = _response.totalCount;
                    int pages = 1;
                    pager.TotalPages = pages;
                }
                _response.Pager = pager;
                _messages.Add(new Message_Info { Message = "Categories List", Type = Message_Type.SUCCESS.ToString() });

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
            _categoryService = null;
            _response = null;
            _messages = null;
        }
    }
}
