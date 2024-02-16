using System.Data;

namespace ExcelUpload.Service.Repositories.PartnerUpload
{
    public interface IUploadPartnerDetails
    {
        bool Check_If_User_IsExist(string firstName, string lastName, string myMentorCode);
        bool Check_If_User_IsExist(string myMentorCode);
        bool Check_If_MentorCode_Exist(string mentorCode);

        void Bulk_Partner_Upload(DataTable dt,string FileType);
    }
}
