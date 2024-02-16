using Promotion.Service.Models.Promotions;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using Promotion.Service.Repositories.PromotionService;
using UJBHelper.DataModel;
using System.Collections.Generic;

namespace Promotion.Service.Services.AddPromotionService
{
    public class AddPromotionService : IAddPromotionService
    {
        private readonly IMongoCollection<DbPromotions> _promotions;
        private IConfiguration _iconfiguration;
        private readonly IMongoCollection<User> _userDetails;
        private readonly IMongoCollection<BusinessDetails> _businessDetails;

        public AddPromotionService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _promotions = database.GetCollection<DbPromotions>("Promotions");
            _userDetails = database.GetCollection<User>("Users");
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
        }

        public void Insert_New_Promotion(Post_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var p = new DbPromotions
            {
                userId = request.userId,
                startDate = request.startDate.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)),
                endDate = request.endDate.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)),
                isActive = true,
                ReferenceUrl = request.ReferenceUrl,
                productServiceId = request.productServiceId,
                media = request.Media1,

                Created = new Created
                {
                    created_By = request.created_By,
                    created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                },

                Updated = new Updated
                {
                    updated_By = request.created_By,
                    updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                }
            };

            _promotions.InsertOne(p);
        }

        public void Update_Promotions_Details(Post_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });
            _promotions.FindOneAndUpdate(
                Builders<DbPromotions>.Filter.Eq(x => x.Id, request.PromotionId),
                Builders<DbPromotions>.Update
                .Set(x => x.userId, request.userId)
                .Set(x => x.startDate, request.startDate.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)))
                .Set(x => x.endDate, request.endDate.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)))
                .Set(x => x.productServiceId, request.productServiceId)
                .Set(x => x.ReferenceUrl, request.ReferenceUrl)
                .Set(x => x.media, request.Media1)
                .Set(x => x.Updated, new Updated
                {
                    updated_By = request.created_By,
                    updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                })
                );
        }

        public Get_Request Get_PromotionListedPartnerList()
        {

            var res = new Get_Request();
            List<PromotionsLPList> promotionLPList = new List<PromotionsLPList>();

            var filter = Builders<BusinessDetails>.Filter.Eq(x => x.isApproved.Flag, 1);

            var BusinessDetails = _businessDetails.Find(filter).Project(x =>
                    new BusinessDetails
                    {
                        Id = x.Id,
                        UserId = x.UserId
                    }).ToList();

            foreach (var r in BusinessDetails)
            {
                PromotionsLPList _promotionLP = new PromotionsLPList();
                var Filter = Builders<User>.Filter.Eq(x => x._id, r.UserId)
                    & Builders<User>.Filter.Eq(x => x.isMembershipAgreementAccepted, true);
                //& Builders<User>.Filter.Eq(x => x.isActive, true);
                if (_userDetails.Find(Filter).CountDocuments() > 0)
                {
                    _promotionLP.Name = _userDetails.Find(Filter).FirstOrDefault().firstName + " " + _userDetails.Find(Filter).FirstOrDefault().lastName;
                    _promotionLP.bussinessId = r.Id;
                    _promotionLP.userId = r.UserId;
                    _promotionLP.IsACtive = _userDetails.Find(Filter).FirstOrDefault().isActive;
                    promotionLPList.Add(_promotionLP);
                }

            }
            res.PromotionLPList = promotionLPList;


            return res;
        }
    }
}
