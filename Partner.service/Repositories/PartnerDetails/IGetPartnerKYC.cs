using Partner.Service.Models.PartnerDetails.PartnerKYC;

namespace Partner.Service.Repositories.PartnerDetails
{
    public interface IGetPartnerKYC
    {
        bool Check_If_User_Exist(string UserId);
        bool Check_If_User_IsActive(string UserId);
        Get_Request GetPartnerKYC(string UserId);
        
    }

}
