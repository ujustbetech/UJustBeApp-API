using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace UJBHelper.DataModel
{
    public class ReferralAgreedPercentage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public int transferTo { get; set; }
        public double Percentage { get; set; }
       
        public DateTime EffectiveStartDate { get; set; }

     
        public DateTime EffectiveEndDate { get; set; }
        public bool isActive { get; set; }





    }
}
