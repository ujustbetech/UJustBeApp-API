using Business.Service.Models.ClientPartner;

namespace Business.Service.Repositories.AddProductService
{
    public interface IClientPartnerService
    {
        bool Check_If_User_Exists(string userId);
        Get_Request Get_Client_Partner_Details(string userId);
        bool Check_If_Client_Partner_Exists(string userId);
    }
}
