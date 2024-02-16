namespace Business.Service.Models.Company.UpdateCompanyAddress
{
    public class Post_Request
    {
        public string businessId { get; set; }
        public string location { get; set; }
        public string flatWing { get; set; }
        public string locality { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}
