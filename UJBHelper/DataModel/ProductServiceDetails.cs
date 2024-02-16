using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UJBHelper.DataModel
{
    public class ProductServiceDetails
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string prodservId { get; set; }
        public int? type { get; set; }
        public double? value { get; set; }
       // [BsonIgnoreIfNull]
        public int? from { get; set; }
       // [BsonIgnoreIfNull]
        public int? to { get; set; }
        public string productName { get; set; }
        public bool isActive { get; set; }
        public Created created { get; set; }
        public Updated updated { get; set; }
    }
}
