using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace UJBHelper.DataModel
{
    public class ErrorLogs
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string UserId { get; set; }
        public string Url { get; set; }
        public string screen { get; set; }
        public string Method { get; set; }
        public string message { get; set; }
        public string error { get; set; }
        public DateTime date { get; set; }
        public string source { get; set; }
        public Created Created { get; set; }
        public Updated Updated { get; set; }
    }
}
