using System.ComponentModel.DataAnnotations;

namespace Auth.Service.Models.Login
{
    public class Post_Request
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
