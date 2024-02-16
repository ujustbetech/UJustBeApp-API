namespace Lead_Management.Service.Models.Payment
{
    public class Put_Request
    {
        public string leadId { get; set; }
        public string paymentFrom { get; set; }
        public string PaymentTo { get; set; }

        public bool IsUJB { get; set; }
    }
}

