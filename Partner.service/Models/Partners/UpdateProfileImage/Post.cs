namespace Partner.Service.Models.Partners.UpdateProfileImage
{
    public class Post_Request
    {
        public string userId { get; set; }
        public string FileName { get; set; }
        public string ImageURL { get; set; }
        public string UniqueName { get; set; }       
        public string ImageBase64 { get; set; }
    }
}
