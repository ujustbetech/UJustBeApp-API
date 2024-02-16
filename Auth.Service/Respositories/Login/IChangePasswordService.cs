using Auth.Service.Models.ChangePassword;

namespace Auth.Service.Respositories.Login
{
    public interface IChangePasswordService
    {
        bool Verify_User(string username);
        void Create_New_Password(string user_id, string new_password);
        Get_Request Get_User_Details(string user_id);
        string Get_Password(string user_Id);
    }
}
