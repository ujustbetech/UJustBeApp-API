using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace UJBHelper.DataModel
{
    public class PaymentDetails
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string leadId { get; set; }
        public int paymentType { get; set; }
        public string Description { get; set; }
        public int PayType { get; set; }
        public string paymentFrom { get; set; }
        public List<string> paymentTo { get; set; }
        public List<string> AdjustedTransactionIds { get; set; }
        public double CPReceivedAmt { get; set; }
        public int paymentFor { get; set; }
        public double Amount { get; set; }
        public string bankName { get; set; }
        public string branchName { get; set; }
        public string IFSCCode { get; set; }
        public string accountHolderName { get; set; }
        public ChequeDetails chequeDetails { get; set; }
        public NeftDetails neftDetails { get; set; }
        public string paymentDate { get; set; }
        public Created Created { get; set; }
        public Updated Updated { get; set; }
        public string cashPaidName { get; set; }
        public string mobileNumber { get; set; }
        public string emailId { get; set; }
        public string countryCode { get; set; }
        public string PaymentTransactionId { get; set; }
        public ShareRecievedByPartners ShareRecvdByPartners { get; set; }
        public double percSharedRecvdFrmPramotion { get; set; }
        public double amtRecvdFrmPramotion { get; set; }
        public double amtRecvdbyUJB { get; set; }
        public double adjustedRegiFeefrmPromotion { get; set; }
        public int sharedId { get; set; }

    }
    public class NeftDetails
    {
        public string ReferrenceNo { get; set; }
        public string TransactionDate { get; set; }

    }

    public class ChequeDetails
    {
        public string chequeNo { get; set; }
    }
}
