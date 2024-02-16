using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Service.Models.ApproveBusinessKYC;

namespace Business.Service.Repositories.ApproveBusinessKYC
{
    public interface IApproveBusinessKYCService
    {
        bool Check_If_Business_Exists(string businessId);
        bool Check_If_SusbscriptionPaymentDone(string businessId);
        void Update_KYC_Details(Put_Request request);

        void AddSubscriptionDetails(string BusinessId);
    }
}
