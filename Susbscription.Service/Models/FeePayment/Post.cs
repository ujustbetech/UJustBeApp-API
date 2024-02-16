using System;
using System.Collections.Generic;

namespace Susbscription.Service.Models.FeePayment
{
    public class Post_Request
    {
        public string userId { get; set; }
        public string countryId { get; set; }
        public string emailId { get; set; }
        public string CashPaidName { get; set; }
        public string mobileNumber { get; set; }
        public string referralId { get; set; }
        public string paymentType { get; set; }
        public string feeType { get; set; }
        public double feeAmount { get; set; }
        public string PaidTransactionId { get; set; }
        public double amount { get; set; }
        public string bankName { get; set; }
        public string branchName { get; set; }
        public string IFSCCode { get; set; }
        public string accountHolderName { get; set; }
        public string chequeNo { get; set; }
        public string referenceNo { get; set; }        
        public DateTime? transactionDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Description { get; set; }

        public List<string> PaymentTransactionId { get; set; }
        public string Created_By { get; set; }        
        public string Updated_By { get; set; }
    }
}
