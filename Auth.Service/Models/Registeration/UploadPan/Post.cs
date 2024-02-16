using System.ComponentModel.DataAnnotations;

namespace Auth.Service.Models.Registeration.UploadPan
{
    public class Post_Request
    {
        [Required]
        public string userId { get; set; }
        public string panNumber { get; set; }
        public string panImgBase64 { get; set; }
        public string panImgType { get; set; }
        public string panType { get; set; }
        public string FileName { get; set; }
        public string ImageURL { get; set; }
        public string UniqueName { get; set; }
    }
}
