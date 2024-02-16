using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UJBHelper.Data;
using UJBHelper.DataModel;

namespace SusbscriptionService.Services
{
    public class SubscriptionService
    {
        private readonly IMongoCollection<SubscriptionDetails> _subs;
        private readonly IMongoCollection<BusinessDetails> _bsnsDetails;

        public SubscriptionService()
        {
            var client = new MongoClient(DbHelper.GetConnectionString());
            var database = client.GetDatabase(DbHelper.GetDatabaseName());
            _subs = database.GetCollection<SubscriptionDetails>("SubscriptionDetails");
            _bsnsDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
        }

        internal void Check_If_Subscription_Active()
        {
            var usersIds = _bsnsDetails.Find(x => x.isApproved.Flag == 1).Project(x => x.UserId).ToList();
            foreach (var item in usersIds)
            {
                TimeSpan diff = DateTime.Now - DateTime.UtcNow;
                string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

                DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
                //DateTime startDate = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy"));
                var filter1 = Builders<SubscriptionDetails>.Filter.Gte(x => x.EndDate, CurrentDate);
                filter1 = filter1 & Builders<SubscriptionDetails>.Filter.Eq(x => x.feeType, "5d5a4534339dce0154441aac");
                filter1 = filter1 & Builders<SubscriptionDetails>.Filter.Eq(x => x.userId, item);
                if (_subs.Find(filter1).CountDocuments() == 0)
                {
                    //set isSubscriptionActive = false
                    _bsnsDetails.FindOneAndUpdate(
                Builders<BusinessDetails>.Filter.Eq(x => x.UserId, item),
                Builders<BusinessDetails>.Update
                .Set(x => x.isSubscriptionActive, false)
                );
                }
            }
        }
    }
}
