using System.Collections.Generic;
using Business.Service.Models.Company.UpdateBusiness;

namespace Business.Service.Repositories.Company
{
    public interface IUpdateBusinessService
    {
        Post_Request Update_Business_Categories(Post_Request request);
        bool Check_If_Business_Exists(string businessId, string userId);
        bool Check_If_User_Exists(string userId);
        void Update_Business_Admin(Put_Request request);
        void Insert_Business_Admin(Put_Request request);
        string Insert_Business_Categories(Post_Request request);
        List<Get_Request> Get_Business_List();
        string Check_If_User_Bussiness_Exists(string userId);
        string UpdateBanner(Business.Service.Models.Company.UpdateBusiness.UpdateBanner.Put_Request request);
        string Get_Coordinates_From_Address(string flatWing, string companylocation, string locality);
    }
}
