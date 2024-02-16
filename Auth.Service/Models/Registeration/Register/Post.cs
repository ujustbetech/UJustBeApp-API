namespace Auth.Service.Models.Registeration.Register
{
    public class Post_Request
    {
        public string Id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string mobileNumber { get; set; }
        public string countryCode { get; set; }
        public string password { get; set; }
        public string userRole { get; set; }
        public string socialMediaId { get; set; }
        public string socialMediaType { get; set; }
        public string createdBy { get; set; }
        public string middleName { get; set; }
        public bool isActive { get; set; }
        public string alternateMobileNumber { get; set; }
        public string alternateCountryCode { get; set; }
        public string  isActiveComment { get; set; }
    }
}
