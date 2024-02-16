using System.ComponentModel.DataAnnotations;

namespace Notification.Service.Models.FCMToken
{
    public class Put_Request
    {
        [Required]
        public string userId { get; set; }
        public string token { get; set; }
    }
}
