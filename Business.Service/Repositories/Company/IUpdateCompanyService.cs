using Post_Request = Business.Service.Models.Company.UpdateCompany.Post_Request;

namespace Business.Service.Repositories.Company
{
    public interface IUpdateCompanyService
    {
        void UpdateCompanyName(Post_Request request);
        void UpdateBusinessEmail(Post_Request request);
        bool UpdateBusinessDescription(Post_Request request);
        void UpdateNameOfPartner(Post_Request request);
        void UpdateBusinessUrl(Post_Request request);
        void UpdateCompanyLogo(Models.Company.UpdateCompanyLogo.Post_Request request);
        bool Check_If_Business_Exists(string businessId);
        void UpdateCompanyAddress(Models.Company.UpdateCompanyAddress.Post_Request request);
        void UpdateBusinessGst(Post_Request request);
        string Get_Coordinates_From_Address(string flatWing, string companylocation, string locality);
    }
}
