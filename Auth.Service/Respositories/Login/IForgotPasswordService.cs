using Auth.Service.Models.Admin.ForgotPassword;
using Auth.Service.Models.ForgotPassword;

namespace Auth.Service.Respositories.Login
{
    public interface IForgotPasswordService
    {
        string Verify_User(string username);
        string Verify_Admin_User(string username);
        void Create_New_Admin_Password(string user_id, string new_password);
        void Create_New_Password(string user_id, string new_password);
        Get_Request Get_User_Details(string user_id);
        Get_Admin_Request Get_Admin_User_Details(string user_id);
    }
}
