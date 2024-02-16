namespace Auth.Service.Models.Registeration.Otp
{
    public class Put_Request
    {
        public string userId { get; set; }
        public string MobileNumber { get; set; }
        public string countryCode { get; set; }
    }
}
