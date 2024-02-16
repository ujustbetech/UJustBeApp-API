using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using UJBHelper.DataModel.Common;

namespace UJBHelper.DataModel
{
    public class SubscriptionDetails
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id{get;set;}
        public string userId { get; set; }

        //[BsonElement("StartDate")]
        //[BsonSerializer(typeof(FechaTweetsSerializer))]
        public DateTime StartDate { get; set; }

        //[BsonElement("EndDate")]
        //[BsonSerializer(typeof(FechaTweetsSerializer))]
        public DateTime EndDate { get; set; }



       
        public double Amount { get; set; }
        public string feeType { get; set; }
        public Created Created { get; set; }

    }
}
