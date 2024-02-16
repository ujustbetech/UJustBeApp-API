using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UJBHelper.DataModel
{
    public class Categories
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string categoryName { get; set; }
        public bool active { get; set; }
        public bool PercentageShare { get; set; }
        public string categoryImgBase64 { get; set; }
        public Created Created { get; set; }
        public Updated Updated { get; set; }
    }
}
