using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace UJBHelper.DataModel
{
    public class BusinessDetails
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
        public List<string> Categories { get; set; }
        public string Tagline { get; set; }
        public string CompanyName { get; set; }
        public Logo Logo { get; set; }
        public string BusinessEmail { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }

        public double? averageRating { get; set; }

        public BusinessAddress BusinessAddress { get; set; }
        public string BusinessDescription { get; set; }
        public string WebsiteUrl { get; set; }
        public string GSTNumber { get; set; }
        public BusinessPan BusinessPan { get; set; }
        public Approved isApproved { get; set; }
        public Created Created { get; set; }
        public Updated Updated { get; set; }
        public Banner BannerDetails { get; set; }
        public bool isSubscriptionActive { get; set; }
        public int UserType { get; set; }
        public string NameOfPartner { get; set; }
    }

    public class Approved
    {
        public int Flag { get; set; }
        public string Reason { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedOn { get; set; }
    }

    public class Logo
    {
      //  public string logoBase64 { get; set; }
        public string logoImageType { get; set; }
        public string logoImageName { get; set; }
        public string logoImageURL { get; set; }
       // public string logoFileName { get; set; }
        public string logoUniqueName { get; set; }

    }

    public class Banner
    {
        public string ImageName { get; set; }
        public string ImageURL { get; set; }
        public string UniqueName { get; set; }
    }

    public class BusinessAddress
    {
        public string Location { get; set; }
        public string Flat_Wing { get; set; }
        public string Locality { get; set; }
    }

    public class BusinessPan
    {
        public string PanNumber { get; set; }
     //  public string PanBase64 { get; set; }
      //  public string PanImageType { get; set; }

        public string FileName { get; set; }
        public string ImageURL { get; set; }
        public string UniqueName { get; set; }
    }

}
