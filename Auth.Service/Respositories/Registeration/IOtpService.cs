using Auth.Service.Manager.Registeration.Otp;
using Auth.Service.Models.Registeration.Otp;

namespace Auth.Service.Respositories.Registeration
{
    public interface IOtpService
    {
        void Update_Otp(string userId, string otp);
        void Update_Otp_Flag(Post_Request request);
        void UpdateMobileNo(string UserId, string MobileNo,string countryCode);
        bool Check_If_User_Exists(string userId);
        User_Contact_Details Get_User_Details(string userId);
    }
}
