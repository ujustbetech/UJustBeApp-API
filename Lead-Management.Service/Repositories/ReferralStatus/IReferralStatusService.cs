using Lead_Management.Service.Manager.Referral;
using Lead_Management.Service.Models.ReferralStatus;

namespace Lead_Management.Service.Repositories.ReferralStatus
{
    public interface IReferralStatusService
    {
        void Update_Referral_Status(Put_Request request);
        Get_Request Get_Dependent_Status_Details(int StatusId);
        Get_Request Get_Dependent_Status(int StatusId);
        bool Check_If_Status_Exist(int StatusId);
        Email_Details Get_Referrer_Email_Id(string leadId);
        bool Is_Active_Users(string LeadId);
    }
}
