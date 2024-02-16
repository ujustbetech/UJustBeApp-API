using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UJBHelper.DataModel
{
    public class UserOtherDetails
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string UserId { get; set; }
       // public string father_Husband_Name { get; set; }
        public string maritalStatus { get; set; }
        public string nationality { get; set; }
        public string phoneNo { get; set; }
        public string Hobbies { get; set; }
        public string areaOfInterest { get; set; }
        public bool? canImpartTraining { get; set; }
        public string aboutMe { get; set; }
        public Created Created { get; set; }
        public Updated Updated { get; set; }
    }
}
