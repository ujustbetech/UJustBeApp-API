using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace UJBHelper.DataModel
{
    public class AgreementDetails
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string BusinessId { get; set; }

        public double Version { get; set; }
        public string PdfURL { get; set; }

        public string type { get; set; }
        public Created created { get; set; }

        public Accepted accepted { get; set; }
    }

    public class Accepted
    {
        public string accepted_By { get; set; }
        public DateTime accepted_On { get; set; }
        public bool isAccepted { get; set; }
    }
}
