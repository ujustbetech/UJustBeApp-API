using Lead_Management.Service.Models.AdminReferral;

namespace Lead_Management.Service.Repositories.Referral
{
    public interface IAdminReferralService
    {
        bool Check_If_User_Exists(string userId);
        Get_Request Get_Referral_Details(string userId);
    }
}
