namespace Auth.Service.Models.Admin.ForgotPassword
{
    public class Get_Admin_Request
    {
        public string _id { get; set; }

        public string Firstname { get; set; }

        public string emailId { get; set; }

        public bool Is_Otp_Verified { get; set; }

        public string countryCode { get; set; }

        public string mobileNumber { get; set; }

        public string newPassword { get; set; }
    }
}
