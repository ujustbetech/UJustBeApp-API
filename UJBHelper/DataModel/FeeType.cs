using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UJBHelper.DataModel
{
    public class FeeType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
       public string feeType { get; set; }
    }
}
