using Partner.Service.Models.Partners;
using Partner.Service.Models.Partners.GetAllDetails;
using Partner.Service.Models.Partners.GetConnectors;

namespace Partner.Service.Repositories.GetPartnerService
{
    public interface IGetPartnerService
    {
        Get_Request Get_PartnerList(int size, int page);
        Get_Connector_Request GetConnectorList(string UserId);
        bool Check_If_User_Exist(string UserId);
        bool Check_If_User_IsActive(string UserId);

        Get_Details_Excel GetUserAllDetails();
    }
}
