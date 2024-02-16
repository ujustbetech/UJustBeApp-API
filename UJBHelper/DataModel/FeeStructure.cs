using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using UJBHelper.DataModel.Common;

namespace UJBHelper.DataModel
{
    public class FeeStructure
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string FeeTypeId { get; set; }
        public int CountryId { get; set; }
        public double Amount { get; set; }

        //[BsonElement("FromDate")]
        //[BsonSerializer(typeof(FechaTweetsSerializer))]
        public DateTime FromDate { get; set; }

        //[BsonElement("ToDate")]
        //[BsonSerializer(typeof(FechaTweetsSerializer))]
        public DateTime ToDate { get; set; }

    }
}
