using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System;

namespace UJBHelper.DataModel
{
    [BsonIgnoreExtraElements]
    public class DbProductServiceArchive
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string productId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string bussinessId { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public int? typeOf { get; set; }
        public List<ProductImg> ProductImg { get; set; }
        public double productPrice { get; set; }
        public double minimumDealValue { get; set; }
        public int? shareType { get; set; }
        public bool isActive { get; set; }
        public Created Created { get; set; }
        public string UpdatedFields { get; set; }
        public string Action { get; set; }
        public DateTime? ProductCreatedOn { get; set; }
    }

   
}
