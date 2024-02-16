using Auth.Service.Models.Registeration.UploadPan;

namespace Auth.Service.Respositories.Registeration
{
    public interface IUploadPanService
    {
        void Update_Pan_Details(Post_Request request);
        bool Check_If_User_Exists(string userId);
        bool Check_If_All_Docs_Uploaded(string userId);
    }
}
