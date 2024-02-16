using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using UJBHelper.DataModel.Common;

namespace UJBHelper.DataModel
{
    public class NotificationList
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string userId { get; set; }

        [BsonSerializer(typeof(NotificationFechaTweetsSerializer))]
        public DateTime date { get; set; }
        public string messageText { get; set; }
        public string type { get; set; }
        public bool isRead { get; set; }
        public bool isSystemGenerated { get; set; }
        public string leadId { get; set; }
        public bool isReferredByMe { get; set; }
    }
}
