using UJBHelper.DataModel;

namespace Auth.Service.Respositories.Login
{
    public interface ILoginService
    {
        string Verify_User(string username, string password);
        User Get_Post_Login_Details(string userid);
        string Verify_Admin_User(string username, string password);
        AdminUser Get_Post_Login_Admin_Details(string userid);
        string Get_Business_Id(string id);
        string VerifyUser(string username, string password);
    }
}
