namespace Lead_Management.Service.Models.Referral
{
    public class Put_Request
    {
        public string userId { get; set; }
        public string dealId { get; set; }
        public int referralStatus { get; set; }
        public string rejectionReason { get; set; }
        //public string status { get; set; }
    }
}
