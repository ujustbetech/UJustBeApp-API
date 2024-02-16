using Auth.Service.Models.Registeration.UploadBankDetails;

namespace Auth.Service.Respositories.Registeration
{
    public interface IUploadBankDetailsService
    {
        void Update_Bank_Details(Post_Request request);
        bool Check_If_User_Exists(string userId);
        bool Check_If_All_Docs_Uploaded(string userId);
    }
}
