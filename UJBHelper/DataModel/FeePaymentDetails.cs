using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using UJBHelper.DataModel.Common;

namespace UJBHelper.DataModel
{
    public class FeePaymentDetails
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string userId { get; set; }
        public string countryId { get; set; }
        public string emailId { get; set; }
        public string mobileNumber { get; set; }
        public string referralId { get; set; }
        public string paymentType { get; set; }
        public string feeType { get; set; }
        public double feeAmount { get; set; }
        public double amount { get; set; }
        public string bankName { get; set; }
        public string branchName { get; set; }
        public string IFSCCode { get; set; }
        public string accountHolderName { get; set; }
        public ChequeDetails chequeDetails { get; set; }
        public string referenceNo { get; set; }

        public string PaidTransactionId { get; set; }

        //[BsonIgnoreIfNull]
        //[BsonElement("transactionDate")]
        //[BsonSerializer(typeof(FechaTweetsSerializer))]       
        //public DateTime ConvertedTransactionDate { get; set; }

        public DateTime? transactionDate { get; set; }

        //[BsonElement("PaymentDate")]
        //[BsonSerializer(typeof(FechaTweetsSerializer))]
        //public DateTime ConvertedPaymentDate { get; set; }

        [BsonElement("PaymentDate")]
        public DateTime ConvertedPaymentDate { get; set; }
        public string Description { get; set; }
        public string CashPaidName { get; set; }
        public Created Created { get; set; }
        public Updated Updated { get; set; }
        public List<string> PaymentTransactionId { get; set; }
    }
}
