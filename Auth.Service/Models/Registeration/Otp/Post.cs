namespace Auth.Service.Models.Registeration.Otp
{
    public class Post_Request
    {
        public string userId { get; set; }
        public bool otpValidationFlag { get; set; }
        public string MobileNumber { get; set; }
        public string countryCode { get; set; }
        public string type { get; set; }
    }
}
