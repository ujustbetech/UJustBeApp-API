using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace UJBHelper.DataModel
{
    public class Agreement
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string Type { get; set; }
        public string AgreementContent { get; set; }       
        public Created Created { get; set; }
        public Updated Updated { get; set; }
    }
}
