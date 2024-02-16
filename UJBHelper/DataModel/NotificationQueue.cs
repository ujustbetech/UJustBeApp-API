using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace UJBHelper.DataModel
{
    public class NotificationQueue
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string userId { get; set; }
        public string leadId { get; set; }
        public string notificationId { get; set; }
        public DateTime dateOfNotification { get; set; }
        public string status { get; set; }
        public string Event { get; set; }
        public string Type { get; set; }
        public string Receiver { get; set; }
        public string Message { get; set; }
        public string ContactInfo { get; set; }
        public string MessageBody { get; set; }
        public Created Created { get; set; }


    }
}
