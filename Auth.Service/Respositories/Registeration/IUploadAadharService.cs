using Auth.Service.Models.Registeration.UploadAadhar;

namespace Auth.Service.Respositories.Registeration
{
    public interface IUploadAadharService
    {
        void Update_Aadhar_Details(Post_Request request);
        bool Check_If_User_Exists(string userId);
        bool Check_If_All_Docs_Uploaded(string userId);
    }
}
