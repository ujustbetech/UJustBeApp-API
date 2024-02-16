using MongoDB.Driver;
using System;
using Microsoft.Extensions.Configuration;
using Promotion.Service.Repositories.GetPromotionService;
using Promotion.Service.Models.Promotions;
using UJBHelper.DataModel;
using System.Collections.Generic;

using System.Linq;


namespace Promotion.Service.Services.GetPromotionService
{
    public class GetPromotionService : IGetPromotionService
    {
        private readonly IMongoCollection<DbPromotions> _promotions;
        private readonly IMongoCollection<User> _userDetails;
        private IConfiguration _iconfiguration;

        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private readonly IMongoCollection<DbProductService> _productService;

        public GetPromotionService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _promotions = database.GetCollection<DbPromotions>("Promotions");
            _userDetails = database.GetCollection<User>("Users");
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
            _productService = database.GetCollection<DbProductService>("ProductsServices");

        }

        public Get_Request Get_PromotionsDetails(string PromotionId)
        {
            var res = new Get_Request();

            var filter = Builders<DbPromotions>.Filter.Eq(x => x.Id, PromotionId);
            res._promotions = _promotions.Find(filter).Project(x =>
                     new DbPromotions
                     {
                         Id = x.Id,
                         userId = x.userId,
                         startDate = x.startDate,
                         endDate = x.endDate,
                         ReferenceUrl = x.ReferenceUrl,
                         productServiceId = x.productServiceId,
                         media = x.media,
                         isActive = x.isActive
                     }).FirstOrDefault();
            return res;
        }

        public Get_Request Get_PromotionList(int size, int page, string UserId)
        {
            //var end = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")), int.Parse(DateTime.Now.ToString("dd")));
            //var PromotionList = _promotions.Find(x => x.EndFecha >= end).ToList();
            //var filter = Builders<DbPromotions>.Filter.Gte(x => x.EndFecha,DateTime.Parse(end.ToString("yyyy-MM-dd")));

            var res = new Get_Request();
            var filter = Builders<DbPromotions>.Filter.Empty;

            if (UserId != string.Empty && UserId != "null" && UserId != null)
            {
                filter = filter & Builders<DbPromotions>.Filter.Eq(x => x.userId, UserId);
            }
            res.PromotionList = new List<PromotionsList>();

            var PromotionList = _promotions.Find(filter).SortByDescending(x => x.Updated.updated_On).Project(x =>
                    new PromotionsList
                    {
                        Id = x.Id,
                        userId = x.userId,
                        startDate = (x.startDate.ToString("yyyy-MM-dd")),
                        endDate = (x.endDate.ToString("yyyy-MM-dd")),
                        media = x.media,
                        created = x.Created,
                        updated = x.Updated

                    }).ToList();

            foreach (var r in PromotionList)
            {
                
                var Filter = Builders<User>.Filter.Eq(x => x._id, r.userId)
                    & Builders<User>.Filter.Eq(x => x.isActive, true);
                if (_userDetails.Find(Filter).CountDocuments() > 0)
                {
                    PromotionsList promotion = new PromotionsList();
                    promotion.Id = r.Id;
                    promotion.userId = r.userId;
                    promotion.startDate = r.startDate;
                    promotion.endDate = r.endDate;
                    promotion.media = r.media;
                    promotion.created = r.created;
                    promotion.updated = r.updated;
                    promotion.Name = _userDetails.Find(Filter).FirstOrDefault().firstName + " " + _userDetails.Find(Filter).FirstOrDefault().lastName;
                    res.PromotionList.Add(promotion);
                }
            }

            res.totalCount = res.PromotionList.Count;
            return res;
        }

        public Get_Request Get_PromotionMedia()
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var res = new Get_Request();
            res.PromotionMedia = new List<MediaList>();
            DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));

            // var end = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")), int.Parse(DateTime.Now.ToString("MM")), int.Parse(DateTime.Now.ToString("dd")));
            var filter = Builders<DbPromotions>.Filter.Gte(x => x.endDate, CurrentDate);
            res.PromotionList = _promotions.Find(filter).SortByDescending(x => x.Updated.updated_On).Project(x =>
                      new PromotionsList
                      {
                          userId = x.userId,
                          Id = x.Id,
                          media = x.media
                      }).ToList();
            foreach (var item in res.PromotionList)
            {
                foreach (var m in item.media)
                {
                    var prdcnt = 0;
                    var usercnt = _userDetails.Find(x => x.isMembershipAgreementAccepted == true && x.isActive == true && x._id == item.userId).CountDocuments();
                    var businesscnt = _businessDetails.Find(x => x.isApproved.Flag == 1 && x.isSubscriptionActive && x.UserId == item.userId).CountDocuments();
                    if (businesscnt > 0)
                    {
                        string bussId = _businessDetails.Find(x => x.isApproved.Flag == 1 && x.isSubscriptionActive && x.UserId == item.userId).Project(x => x.Id).FirstOrDefault();
                        prdcnt = int.Parse(_productService.Find(x => x.bussinessId == bussId).CountDocuments().ToString());
                    }
                    if (usercnt > 0 && businesscnt > 0 && prdcnt > 0)
                    {
                        res.PromotionMedia.Add(new MediaList
                        {
                            UserId = item.userId,
                            PromotionId = item.Id,
                            ImageURL = "Content" + m.ImageURL,
                            FileName = m.FileName

                        });
                    }
                }
            }
            res.totalCount = Convert.ToInt32(res.PromotionMedia.Count.ToString());
            return res;
        }



    }
}
