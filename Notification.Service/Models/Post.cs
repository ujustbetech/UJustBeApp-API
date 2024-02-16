using System.ComponentModel.DataAnnotations;

namespace Notification.Service.Models
{
    public class Post_Request
    {
        [Required]
        public string userId { get; set; }
        public int skipTotal { get; set; }
    }
}
