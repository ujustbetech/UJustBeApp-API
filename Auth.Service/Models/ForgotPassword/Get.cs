namespace Auth.Service.Models.ForgotPassword
{
    public class Get_Request
    {
        public string _id { get; set; }

        public string Firstname { get; set; }

        public string EmailId { get; set; }

        public bool Is_Otp_Verified { get; set; }

        public string countryCode { get; set; }

        public string Mobile_Number { get; set; }

        public string newPassword { get; set; }
    }
}
