using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using MongoDB.Bson.Serialization;
using UJBHelper.DataModel.Common;

namespace UJBHelper.DataModel
{
    public class System_Default
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string Default_Name { get; set; }
        public string Default_Value { get; set; }

        //[BsonElement("startDate")]
        //[BsonSerializer(typeof(FechaTweetsSerializer))]
        public DateTime startDate { get; set; }

        //[BsonElement("endDate")]
        //[BsonSerializer(typeof(FechaTweetsSerializer))]
        public DateTime endDate { get; set; }
        public string fromEmailID{ get; set; }
        public string password { get; set; }
        public string host { get; set; }
        public int port { get; set; }
        public string userName { get; set; }
        public string senderId { get; set; }
    

    }
}
