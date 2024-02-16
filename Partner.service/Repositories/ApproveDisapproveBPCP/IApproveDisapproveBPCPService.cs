using Partner.Service.Models.ApproveDisapproveBPCP;

namespace Partner.Service.Repositories.ApproveDisapproveBPCP
{
    public interface IApproveDisapproveBPCPService
    {
        void Approve_Disapprove_BPCP(Post_Request request,string mentorCode);
    }
}
