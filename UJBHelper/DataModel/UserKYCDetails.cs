using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace UJBHelper.DataModel
{
    public class UserKYCDetails
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public PanCard PanCard { get; set; }
        public AdharCard AdharCard { get; set; }
        public BankDetails BankDetails { get; set; }
        public IsApproved IsApproved { get; set; }
        public Created Created { get; set; }
        public Updated Updated { get; set; }       
    }

    public class PanCard
    {
        public string PanNumber { get; set; }
        //public string PanBase64Img { get; set; }
        //public string PanImgType { get; set; }

        public string FileName { get; set; }
        public string ImageURL { get; set; }
        public string UniqueName { get; set; }
       
    }

    public class AdharCard
    {
        public string AdharNumber { get; set; }
        public string FrontFileName { get; set; }
        public string FrontImageURL { get; set; }
        public string FrontUniqueName { get; set; }
        public string BackFileName { get; set; }
        public string BackImageURL { get; set; }
        public string BackUniqueName { get; set; }
        //public string AdharFbase64Img { get; set; }
        //public string AdharFimgType { get; set; }
        //public string AdharBbase64Img { get; set; }
        //public string AdharBimgType { get; set; }
    }

    public class BankDetails
    {
        public string AccountHolderName { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string IFSCCode { get; set; }
        public string FileName { get; set; }
        public string ImageURL { get; set; }
        public string UniqueName { get; set; }
        //public string Cancelchequebase64Img { get; set; }
        //public string CancelchequeimgType { get; set; }
    }

    public class IsApproved
    {
        public bool Flag { get; set; }
        public string Reason { get; set; }
        public int ReasonId { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedOn { get; set; }
    }
}
