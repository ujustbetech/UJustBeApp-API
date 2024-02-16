using System.ComponentModel.DataAnnotations;

namespace Auth.Service.Models.Registeration.UploadAadhar
{
    public class Post_Request
    {
        [Required]
        public string userId { get; set; }
        public string aadharNumber { get; set; }
        public string aadharFrontBase64 { get; set; }
        public string FrontFileName { get; set; }
        public string FrontUniqueName { get; set; }
        public string FrontImageURL { get; set; }
        public string aadharBackBase64 { get; set; }
        public string BackFileName { get; set; }        
        public string BackUniqueName { get; set; }
        public string BackImageURL { get; set; }
    }
}
