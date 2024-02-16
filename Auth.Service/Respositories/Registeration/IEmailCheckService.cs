namespace Auth.Service.Respositories.Registeration
{
    public interface IEmailCheckService
    {
        bool Check_If_Email_Exists(string emailId);
        bool Check_If_Mobile_Exists(string mobileNo);
    }
}
