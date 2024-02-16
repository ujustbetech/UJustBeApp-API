using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace UJBHelper.DataModel
{
    public class LeadFollowUp
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string leadId { get; set; }
        public DateTime? followUpDate1 { get; set; }
        public DateTime? followUpDate2 { get; set; }
        public DateTime? followUpDate3 { get; set; }
        public string followUpDetails { get; set; }
        public string followUpDetails2 { get; set; }
        public string followUpDetails3 { get; set; }       
    }
}
