using Partner.Service.Models.PartnerDetails;

namespace Partner.Service.Repositories.PartnerDetails
{
    public interface IGetPartnerDetails
    {
        Get_Request GetPartnerDetails(string UserId);       
        bool Check_If_User_Exist(string UserId);
        bool Check_If_User_IsActive(string UserId);
      
        
    }

}
