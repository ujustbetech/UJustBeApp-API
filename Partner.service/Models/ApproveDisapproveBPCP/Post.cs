namespace Partner.Service.Models.ApproveDisapproveBPCP
{
    public class Post_Request
    {
        public string UserId { get; set; }
        public bool Flag { get; set; }
        public string reason { get; set; }
        public int reasonId { get; set; }
        public string Updated_by { get; set; }
       
    }
}
