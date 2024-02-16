
using LegalAgreement.Service.Models.AgreementStatus;
using UJBHelper.DataModel;

namespace LegalAgreement.Service.Repositories.AgreementStatus
{
    public interface IAgreementStatusService
    {
        void Update_Partner_Agreement_Status(Put_Request request);
        void Update_Listed_Partner_Agreement_Status(Put_Request request);
        void Send_Agreement_Via_Email(string userId, bool status);
        void Send_ListedPartner_Agreement_Via_Email(string userId, bool status);
    }
}
