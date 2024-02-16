using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
namespace UJBHelper.DataModel
{
    public class ProductServiceDetailsArchive
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string prodservDdtailsId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string prodservId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string BussinessId { get; set; }
        public int? type { get; set; }
        public double? value { get; set; }
        // [BsonIgnoreIfNull]
        public int? from { get; set; }
        // [BsonIgnoreIfNull]
        public int? to { get; set; }
        public string productName { get; set; }
        public bool isActive { get; set; }
        public Created created { get; set; }
        public string UpdatedFields { get; set; }
        public string Action { get; set; }
        public DateTime? DetailCreatedDated { get; set; }

    }
}
