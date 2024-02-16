using UJBHelper.DataModel;

namespace Lead_Management.Service.Models.Referral
{
    public class DealValue_Put
    {
        public string leadId { get; set; }
        public double dealValue { get; set; }
        public int PercentOrAmt { get; set; }
        public double Value { get; set; }
        public string ProductId { get; set; }
        public ShareRecievedByPartners shareReceivedByPartner { get; set; }
        

    }
}
