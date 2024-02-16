using System.Collections.Generic;
using UJBHelper.DataModel;

namespace Lead_Management.Service.Models.Payment
{
    public class Get_Request
    {
        public PayDetails paymentDetails { get; set; }
        public List<PaymentList> PaymentList { get; set; }
        public int totalCount { get; set; }
    }

    public class PayDetails
    {
        public string _id { get; set; }
        public string leadId { get; set; }
        public int paymentType { get; set; }
        public string PaymentTypeValue { get; set; }
        public string Description { get; set; }
        public int PayType { get; set; }
        public string PayTypeValue { get; set; }
        public string paymentFrom { get; set; }
        public List<string> paymentTo { get; set; }
        public List<string> AdjustedTransactionIds { get; set; }
        public double CPReceivedAmt { get; set; }
        public int paymentFor { get; set; }
        public string paymentForValue { get; set; }
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
        public string AllowEdit { get; set; }
        public double percSharedRecvdFrmPramotion { get; set; }
        public double amtRecvdFrmPramotion { get; set; }
        public double amtRecvdbyUJB { get; set; }

        public double AmtPaid { get; set; }
        public double AmtPaidbyLP { get; set; }
        public double adjustedRegiFeefrmPromotion { get; set; }
        public int sharedId { get; set; }
    }

    public class PaymentList
    {
        public string _id { get; set; }
        public string leadId { get; set; }
        public int paymentType { get; set; }
        public double RegisterationAmt { get; set; }
        public string paymentTypeValue { get; set; }
        public string paymentFrom { get; set; }
        public string paymentFromValue { get; set; }
        public List<string> paymentTo { get; set; }
        public List<string> paymentToValue { get; set; }
        public int paymentFor { get; set; }
        public string paymentForValue { get; set; }
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
        public List<string> AdjustedTransactionIds { get; set; }
        public string PaymentTransactionId { get; set; }
        public bool AllowEdit { get; set; }


        public double percSharedRecvdFrmPramotion { get; set; }
        public double amtRecvdFrmPramotion { get; set; }
        public double amtRecvdbyUJB { get; set; }

        public double AmtPaid { get; set; }
        public double TotalAmtpaidtoParter {get;set;}

    }
}
