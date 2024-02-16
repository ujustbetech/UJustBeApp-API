using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using UJBHelper.DataModel.Common;

namespace UJBHelper.DataModel
{
    public class Leads
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public DateTime? referralDate { get; set; }
        public ReferredBy referredBy { get; set; }
        public ReferredTo referredTo { get; set; }       
        public bool isAccepted { get; set; }
        public string rejectionReason { get; set; }
        public string referredProductORServices { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string referredProductORServicesId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string refMultisSlabProdId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string referredBusinessId { get; set; }
        public string referralDescription { get; set; }
        public bool isForSelf { get; set; }
        public Created Created { get; set; }
        public Updated Updated { get; set; }
        public int referralStatus { get; set; }
        public double dealValue { get; set; }
        public string ReferralCode { get; set; }
        public ShareReceivedByUJB shareReceivedByUJB { get; set; }
        public ShareRecievedByPartners shareRecievedByPartners { get; set; }
        public int dealStatus { get; set; }
        [BsonElement("referralStatusUpdatedOn")]
        [BsonSerializer(typeof(FechaTweetsSerializer))]
        public DateTime refStatusUpdatedOn { get; set; }      
        public string referralStatusUpdatedby { get; set; }        
        public DateTime dealStatusUpdatedOn { get; set; }
        public int? typeOf { get; set; }
        public int? shareType { get; set; }
    }

    public class ShareReceivedByUJB
    {
        public int percOrAmount { get; set; }
        public double value { get; set; }
    }

    public class ShareRecievedByPartners
    {
        public string partnerID { get; set; }
        public double RecievedByReferral { get; set; }
        public string mentorID { get; set; }
        public double RecievedByMentor { get; set; }
        public double RecievedByUJB { get; set; }
        public string LPmentorID { get; set; }
        public double RecievedByLPMentor { get; set; }
    }

    public enum DealStatusEnum
    {
        NotConnected = 1,
        CalledButNoResponse = 2,
        DealNotClosed = 3,
        DiscussionInProgress = 4,
        DealClosed = 5,
        ReceivedPartPayment = 6,
        WorkInProgress = 7,
        WorkCompleted = 8,
        ReceivedFullAndFinalPayment = 9,
        AgreedPercentageTransferredToUJB = 10
    }

    public enum ReferralSharedwith
    {
        UJB = 1,
        Referral = 2,
        ReferralMentor = 3,
        LPMentor = 4,
       
    }

    public class ReferredBy
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string userId { get; set; }
        public string name { get; set; }
    }

    public class ReferredTo
    {
        public string name { get; set; }
        public string countryCode { get; set; }
        public string mobileNumber { get; set; }
        public string emailId { get; set; }
    }

    public enum ReferralStatusEnum
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2
    }
}
