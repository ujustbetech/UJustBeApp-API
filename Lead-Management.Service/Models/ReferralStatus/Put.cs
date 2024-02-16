namespace Lead_Management.Service.Models.ReferralStatus
{
    public class Put_Request
    {
        public string updatedBy { get; set; }
        public string leadId { get; set; }
        public int statusId { get; set; }
    }

  
}
