
using Auth.Service.Models.Registeration.User;
using System.Collections.Generic;
using Get_Request = Auth.Service.Models.Registeration.User.Get_Request;

namespace Auth.Service.Respositories.Registeration
{
    public interface IUserInfoService
    {
        bool Check_If_User_Exists(string userId);
        bool Check_If_User_Other_Exists(string userId);
        bool Check_If_User_IsActive(string userId);
        Get_Request Get_User_Details(string userId);
        bool Check_If_Admin_User_Exists(string userId);
        Models.Admin.User.Get_Request Get_Admin_User_Details(string userId);
        List<Models.Admin.User.Get_Request> Get_Admin_User_List(string _query);
        void Insert_UserOther_Details(Put_Request request);
        void Update_UserOtherDetails(Put_Request request);
    }
}
