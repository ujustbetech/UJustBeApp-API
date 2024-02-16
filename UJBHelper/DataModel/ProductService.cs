using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace UJBHelper.DataModel
{
    [BsonIgnoreExtraElements]
    public class DbProductService
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
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
        public Updated Updated { get; set; }
    }

    public class ProductImg
    {
        public string prodImgName { get; set; }
        public string ImageURL { get; set; }
        public string UniqueName { get; set; }
        public bool isDefaultImg { get; set; }
    }

    public class UploadedProductImg
    {
        public string prodImgBase64 { get; set; }
        public string prodImgName { get; set; }
        public string ImageURL { get; set; }
        public string UniqueName { get; set; }
        public bool isDefaultImg { get; set; }
    }
}
