using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UJBHelper.DataModel
{
    public class StateInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public int stateId { get; set; }
        public string stateName { get; set; }
        public int countryId { get; set; }
    }
}
