using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace UJBHelper.DataModel
{
    public class DealStatus
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
    }

    public class DependentStatus
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; }
    }

    public class DealDependentStatus
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public int StatusId { get; set; }
        public List<DependentStatus> DependentStatus { get; set; }
       // public DependentStatus DepenStatus { get; set; }

    }

   
}
