using System.Collections.Generic;
using UJBHelper.DataModel;

namespace Lead_Management.Service.Models.Payment
{
    public class Post_Request
    {
        public string PaymentId { get; set; }
        public int paymentType { get; set; }
        public int PayType { get; set; }
        public int paymentFor { get; set; }
        public string paymentFrom { get; set; }
        public string PaymentTo { get; set; }
        public string leadId { get; set; }       
        public double amount { get; set; }
        public string bankName { get; set; }
        public string branchName { get; set; }
        public string IFSCCode { get; set; }
        public string accountHolderName { get; set; }
        public string chequeNo { get; set; }
        public string ReferrenceNo { get; set; }
        public string TransactionDate { get; set; }
        public string paymentDate { get; set; }
        public string updated_By { get; set; }
        public string created_By { get; set; }
        public string cashPaidName { get; set; }
        public string mobileNumber { get; set; }
        public string emailId { get; set; }
        public string countryCode { get; set; }
        public string Description { get; set; }
        public List<string> AdjustedTransactionIds { get; set; }
        public double CPReceivedAmt { get; set; }
        public ShareRecievedByPartners ShareRecvdByPartners { get; set; }
        public double percSharedRecvdFrmPramotion { get; set; }
        public double amtRecvdFrmPramotion { get; set; }
        public double adjustedRegiFeefrmPromotion { get; set; }
        public int sharedId { get; set; }

    }   
}
