namespace Auth.Service.Models.Admin.User
{
    public class Get_Request
    {
        public string userId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string emailId { get; set; }
        public string mobileNumber { get; set; }
        public string role { get; set; }
        public string password { get; set; }

        public string countryCode { get; set; }
        public bool isActive { get; set; }
    }
}
