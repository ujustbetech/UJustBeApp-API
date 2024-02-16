using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UJBHelper.DataModel
{
    public class LeadProductServiceDetails
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string LeadId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string prodservId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string prodservdetailId { get; set; }
        public int type { get; set; }
        public double value { get; set; }
        public int? from { get; set; }
        public int? to { get; set; }
        public string productName { get; set; }
        public bool isActive { get; set; }
        public Created created { get; set; }
        public Updated updated { get; set; }
    }
}
