using System.Collections.Generic;
using Search.Service.Models.Dashboard;

namespace Search.Service.Repositories.Dashboard
{
    public interface IDashboardService
    {
        Get_Request Get_Business_By_Search(Post_Request request);
        List<Get_Suggestion> Get_Business_Suggestion(string query,string userId);
        bool Check_If_User_IsActive(string UserId);
    }
}
