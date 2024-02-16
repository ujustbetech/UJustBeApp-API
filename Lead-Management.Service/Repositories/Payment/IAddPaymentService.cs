using Lead_Management.Service.Models.Payment;

namespace Lead_Management.Service.Repositories.Payment
{
    public interface IAddPaymentService
    {
        void Update_Payment_Details(Post_Request request);
        string Insert_New_Payment(Post_Request request);
        bool Check_If_Payment_Exist(string PaymentId);
        Get_Request Get_Payment_List(string LeadId);
        Get_Request Get_PaymentDetails(string PaymentId);
        bool Check_If_Lead_Exist(string LeadId);
        GetBalance Calculate_Balance(Put_Request request);
        void Update_System_Default(string System_Defaut);
        double GetSharedPercentage(string leadId, int sharedID);



    }
}
