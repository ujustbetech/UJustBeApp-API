using System.Collections.Generic;

namespace Lead_Management.Service.Models.Payment
{
    public class GetBalance
    {
        public double TotalShareAmt { get; set; }
        public double ActualAmt { get; set; }
        public double PaidAmt { get; set; }
        public double PendingAmt { get; set; }
        public double BalanceRegisterationAmt { get; set; }
        public List<string> TransactionIds { get; set; }

        public double BalRegisterationAmt { get; set; }
    }
}
