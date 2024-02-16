using Business.Service.Models.Company.Category;
using Business.Service.Repositories.Company;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using UJBHelper.Common;
using UJBHelper.DataModel;

namespace Business.Service.Services.Company
{
    public class CategoryService : ICategoryService
    {
        private readonly IMongoCollection<Categories> _categories;
        private IConfiguration _iconfiguration;
        public CategoryService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _categories = database.GetCollection<Categories>("Categories");
        }

        public Get_Request Get_Categories(string query, PaginationInfo pager)
        {
            var res = new Get_Request();

            var blist1 = new List<Category_Info>();
            if (pager.CurrentPage > 0)
            {
                var skipTotal = (pager.CurrentPage - 1) * pager.PageSize;


                res.categories = _categories.Find(x => x.categoryName.ToLower().Contains(!string.IsNullOrWhiteSpace(query) ? query : "") && x.active && x.categoryName != "Other").Project(x =>
                           new Category_Info { categoryName = x.categoryName, catId = x.Id, categoryImgBase64 = x.categoryImgBase64, PercentageShare = x.PercentageShare }).Sort("{categoryName: 1}").Skip(skipTotal).Limit(pager.PageSize).ToList();


                blist1.AddRange(res.categories);
                res.totalCount = Convert.ToInt32(_categories.Find(x => x.categoryName.ToLower().Contains(!string.IsNullOrWhiteSpace(query) ? query : "") && x.active).CountDocuments().ToString());

                int pages = (res.totalCount + pager.PageSize - 1) / pager.PageSize;
               if( pager.CurrentPage== pages)
                {
                    var categories = _categories.Find(x => x.categoryName.ToLower().Contains(!string.IsNullOrWhiteSpace(query) ? query : "") && x.active && x.categoryName == "Other").Project(x =>
                    new Category_Info { categoryName = x.categoryName, catId = x.Id, categoryImgBase64 = x.categoryImgBase64, PercentageShare = x.PercentageShare }).ToList();

                    blist1.AddRange(categories);
                }
                res.categories = blist1;
            }
            else
            {
                res.categories = _categories.Find(x => x.categoryName.ToLower().Contains(!string.IsNullOrWhiteSpace(query) ? query : "") && x.active && x.categoryName != "Other").Project(x =>
                    new Category_Info { categoryName = x.categoryName, catId = x.Id, categoryImgBase64 = x.categoryImgBase64, PercentageShare = x.PercentageShare }).Sort("{categoryName: 1}").ToList();

                var categories = _categories.Find(x => x.categoryName.ToLower().Contains(!string.IsNullOrWhiteSpace(query) ? query : "") && x.active && x.categoryName == "Other").Project(x =>
                    new Category_Info { categoryName = x.categoryName, catId = x.Id, categoryImgBase64 = x.categoryImgBase64, PercentageShare = x.PercentageShare }).ToList();
                
                blist1.AddRange(res.categories);
                blist1.AddRange(categories);

                res.categories = blist1;
                res.totalCount = Convert.ToInt32(_categories.Find(x => x.categoryName.ToLower().Contains(!string.IsNullOrWhiteSpace(query) ? query : "") && x.active).CountDocuments().ToString());
            }
            return res;
        }
    }
}
