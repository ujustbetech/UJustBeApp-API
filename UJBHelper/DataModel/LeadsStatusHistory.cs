using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UJBHelper.DataModel
{
    public class LeadsStatusHistory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string leadId { get; set; }
        //public DateTime? dealClosedDate { get; set; }
        //public Double dealValue { get; set; }
        public int statusId { get; set; }
        public Updated Updated { get; set; }

        //public ShareReceivedByUJB _shareReceived { get; set; }
    }   
}
