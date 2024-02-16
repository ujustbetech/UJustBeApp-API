using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UJBHelper.DataModel
{
    public class AdminUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string emailId { get; set; }
        public string mobileNumber { get; set; }
        public string password { get; set; }
        public string Role { get; set; }
        public string countryCode { get; set; }
        public bool allowNotifications { get; set; }
        public bool isActive { get; set; }
        public Created Created { get; set; }
        public Updated Updated { get; set; }
    }
}
