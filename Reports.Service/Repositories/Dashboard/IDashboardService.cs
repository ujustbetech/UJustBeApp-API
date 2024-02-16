using Reports.Service.Models.Dashboard;

namespace Reports.Service.Repositories.Dashboard
{
    public interface IDashboardService
    {
        string Get_Total_Business_Closed();
        string Get_Total_Client_Partners();
        string Get_Total_Partners();
        string Get_Total_Referral_Earned();
        string Get_Total_Referral_Passed();
        Post_Request Get_Partner_Stats(string userId);
        Post_Request Get_Client_Partner_Stats(string userId);
        bool Check_If_User_Exists(string userId);
        string Get_Total_Guests_Count();
        string Get_Amount_Earned_By_UJB();
    }
}
