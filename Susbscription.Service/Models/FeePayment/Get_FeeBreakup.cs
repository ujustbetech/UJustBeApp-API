using System;
using System.Collections.Generic;
using UJBHelper.Common;

namespace Susbscription.Service.Models.FeePayment
{
    public class Get_FeeBreakup
    {
        public Get_FeeBreakup()
        {
            feeBreakUp = new List<RegisterFeeDetails>();
            feeBreakUp1 = new List<FeeDetails>();
            _messages = new List<Message_Info>();
        }
        public double FeeAmount { get; set; }
        public double PaidAmount { get; set; }
        public double BalanceAmount { get; set; }       
        public List<RegisterFeeDetails> feeBreakUp { get; set; }
        public List<FeeDetails> feeBreakUp1 { get; set; }

        public List<Message_Info> _messages = null;
    }

    public class RegisterFeeDetails
    {
        public string ReferralNo { get; set; }
        public double EarnedAmt { get; set; }
        public double AdjustedAmt { get; set; }
        public DateTime AdjustedDate { get; set; }
        public string PaymentTransactionId { get; set; }
    }

    public class FeeDetails
    {
        public double Amount { get; set; }
        public string TransactionID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMode { get; set; }


    }
}
