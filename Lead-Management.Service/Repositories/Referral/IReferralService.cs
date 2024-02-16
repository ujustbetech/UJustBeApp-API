using System.Collections.Generic;
using Lead_Management.Service.Manager.Referral;
using Lead_Management.Service.Models.Referral;

namespace Lead_Management.Service.Repositories.Referral
{
    public interface IReferralService
    {
        bool Check_If_Lead_Exists(string referralId);
        Get_Request Get_Referral_Details(string referralId);
        string Create_New_Referral(Post_Request request);
        List<Get_Request> Search_For_Referrals(Lookup_Request request);
        void Update_Referral_Rejection_Status(Put_Request request);
        void Update_System_Default(string Default_Name);
        Email_Details Get_Referrer_Email_Id(string dealId);
        DealValue_Get Update_DealValue(DealValue_Put request);
        double GetSharedPercentage(string leadId, int sharedID);
        LeadProductInfo Get_Product_Service_Details(string LeadId);
        bool Is_Active_Users(string referredById, string businessId);
        bool Is_Active_Users(string LeadId);
    }
}
