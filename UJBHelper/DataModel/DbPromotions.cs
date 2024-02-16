using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System;
using MongoDB.Bson.Serialization;
using UJBHelper.DataModel.Common;

namespace UJBHelper.DataModel
{
    public class DbPromotions
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string userId { get; set; }

        public DateTime startDate { get; set; }
        //[BsonElement("startDate")]
        //[BsonSerializer(typeof(FechaTweetsSerializer))]
        //public DateTime StartFecha { get; set; }

           public DateTime endDate { get; set; } 
        //[BsonElement("endDate")]
        //[BsonSerializer(typeof(FechaTweetsSerializer))]
        //public DateTime EndFecha { get; set; }

        // string endDate { get; set; }
        //public string prodOrdercId { get; set; }
        public bool isActive { get; set; }
        //public string[] Media { get; set; }
        public string ReferenceUrl { get; set; }
        public string productServiceId { get; set; }
        public Created Created { get; set; }
        public Updated Updated { get; set; }
        
        public List<PromotionMedia> media { get; set; }

        [BsonIgnoreIfNull]
        public MediaList PMediaList { get; set; }
    }

    public class PromotionMedia
    {
        public string FileName { get; set; }
        public string ImageURL { get; set; }
        public string UniqueName { get; set; }     
       
    }

    public class MediaList
    {
        public string UserId { get; set; }
        public string PromotionId { get; set; }
        public string ImageURL { get; set; }

        public string FileName { get; set; }
       

    }

    public class PromotionsList
    {
        public string Id { get; set; }
        public string userId { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public List<PromotionMedia> media { get; set; }
        public string Name { get; set; }
        public Updated updated { get; set; }
        public Created created { get; set; }
    }

    public class PromotionsLPList
    {
        public string Id { get; set; }
        public string userId { get; set; }
        public string bussinessId { get; set; }

        public string Name { get; set; }
        public Boolean IsACtive { get; set; }

    }
}
