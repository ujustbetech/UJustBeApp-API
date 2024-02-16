using Reports.Service.Models.ReferralTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Reports.Service.Repositories.ReferralTracking
{
    public interface IReferralTracking
    {
        Get_Request Get_Referral_Details(Post_Request request);
        Put_Response Get_Referral_Excel_Details(Put_Request request);

    }
}
