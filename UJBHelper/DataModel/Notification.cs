using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace UJBHelper.DataModel
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string Event { get; set; }
        public List<string> Type { get; set; }
        public List<NotificationData> Data { get; set; }
        public bool isSystemGenerated { get; set; }
        public int EventId { get; set; }
        public bool isActive { get; set; }
         public int frequency { get; set; }
        public string frequencyType { get; set; }
    }

    public class Email
    {
        public string Subject { get; set; }
        public string Body { get; set; }

        public bool isEmailAllowed { get; set; }
    }

    public class SMS
    {
        public string SMSBody { get; set; }
        public bool  isSMSAllowed{get;set;}
    }

    public class Data
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Push
    {
        public string Title { get; set; }
        public string MessageBody { get; set; }
        public List<Data> data { get; set; }
        public bool isPushAllowed { get; set; }
    }

    public class NotificationData
    {
        public string Receiver { get; set; }
        public Email Email { get; set; }
        public SMS SMS { get; set; }
        public Push Push { get; set; }
    }
}
