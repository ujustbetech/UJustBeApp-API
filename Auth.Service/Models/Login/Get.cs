namespace Auth.Service.Models.Login
{
    public class Get_Request
    {
        public string _id { get; set; }
        public string Role { get; set; }
        public string Language { get; set; }
        public bool Is_Otp_Verified { get; set; }
        public string businessId { get; set; }
        public string mobile_number { get; set; }
        public string country_code { get; set; }
    }
}
