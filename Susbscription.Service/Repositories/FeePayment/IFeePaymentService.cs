using Susbscription.Service.Models.FeePayment;
using Susbscription.Service.Models.UserDetails;
using System.Collections.Generic;
using UJBHelper.DataModel;

namespace Susbscription.Service.Repositories.FeePayment
{
    public interface IFeePaymentService
    {
        void Add_FeePayment_Details(Post_Request request);
        double Check_TotalPayment_Done(string UserId,string FeeType);
        List<Get_Suggestion> Get_Users_Suggestion(string query);
        GetUserOtherDetails Get_Users_OtherDetails(string UserId);
        Get_FeeBreakup Get_FeeBreakup(string UserId, string FeeType);
        Get_FeeType Get_FeeTypes(string userId);
        void Adjust_Partner_RegisterationFee(Post_RegisterationFee request);
        void Adjust_Mentor_RegisterationFee(ShareRecievedByPartners shared,Post_RegisterationFee request);
    }
}
