using Auth.Service.Models.Registeration.EnrollPartner;
using UJBHelper.DataModel;

namespace Auth.Service.Respositories.Registeration
{
    public interface IEnrollPartnerService
    {
        void Update_Enrollment(Post_Request request);
        void Update_EnrollmentDetails(Post_EnrollPartner request);
        bool Check_If_User_Exists(string userId);
        string Get_Coordinates_From_Address(Address addressInfo);
        string Get_Current_Role(string userId);
    }
}
