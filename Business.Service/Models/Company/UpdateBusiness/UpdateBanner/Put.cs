namespace Business.Service.Models.Company.UpdateBusiness.UpdateBanner
{
    public class Put_Request
    {
        public string FileName { get; set; }
        public string UniqueFileName { get; set; }
        public string URL { get; set; }
        public string UserId { get; set; }
        public string BusinessId { get; set; }
        public string Base64string { get; set; }
    }
}
