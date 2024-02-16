namespace Business.Service.Models.Company.UpdateCompanyLogo
{
    public class Post_Request
    {
        public string businessId { get; set; }
        public string logoBase64 { get; set; }
        public string logoImgType { get; set; }
        public string logoImgName { get; set; }
        public string logoImageURL { get; set; }
        public string logoUniqueName { get; set; }
    }
}
