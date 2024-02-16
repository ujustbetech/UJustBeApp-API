using Auth.Service.Models.Registeration.Register;

namespace Auth.Service.Respositories.Registeration
{
    public interface IRegisterService
    {
        Get_Request Insert_User(Post_Request request, string new_otp);
        Get_Request Update_User(Post_Request request);
        bool Check_If_User_PhoneNo_Exists(string MobileNo);
        bool Check_If_User_PhoneNo_Exists(string MobileNo, string UserId);
        bool Check_If_User_Email_Exists(string EmailId);
        bool Check_If_User_Email_Exists(string EmailId,string UserId);
        Get_Request Update_Admin_User(Post_Request request);
        Get_Request Insert_Admin_User(Post_Request request, string new_otp);
        string Verify_Admin_User(string userId, string oldPassword);
        void Create_New_Password(string user_id, string new_password);
        Get_Request Insert_AdminPartner(Post_Request request, string new_otp);
        Get_Request Update_AdminPartner(Post_Request request);
        void Add_To_Notification_Queue(string userId, string v1, string v2, string v3, string v4, string v5, string mobileNumber, string v6, string v7, string v8);
    }
}
