namespace Auth.Service.Models.Registeration.Register
{
    public class Put_Request
    {
        public string userId { get; set; }
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }
}
