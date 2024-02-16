using Partner.Service.Models.PartnerDetails.PartnerProfile;

namespace Partner.Service.Repositories.PartnerDetails
{
    public interface IGetPartnerProfile
    {
        bool Check_If_User_Exist(string UserId);
        bool Check_If_User_IsActive(string UserId);
        Get_Request GetPartnerProfile(string UserId);
        
    }

}
