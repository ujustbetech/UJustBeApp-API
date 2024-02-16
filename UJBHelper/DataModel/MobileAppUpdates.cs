using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UJBHelper.DataModel
{
    public class MobileAppUpdates
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string androidVersion { get; set; }
        public string iosVersion { get; set; }
        public bool isAndroidForce { get; set; }
        public bool isIOSForce { get; set; }

        public bool isActive { get; set; }
        public Created createdBy { get; set; }
        public Updated updatedBy { get; set; }
    }
}
