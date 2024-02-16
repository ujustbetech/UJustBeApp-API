using Partner.Service.Models.Partners.UpdateProfile;

namespace Partner.Service.Repositories.Partner
{
    public interface IUpdatePartnerProfile
    {
        void Update_Otp(string userId, string otp);
        bool Check_If_User_Exist(string UserId);
        bool Check_If_User_IsActive(string UserId);
        void UpdateGender(Post_Request request);
        void UpdateEmail(Post_Request request);
        void UpdateName(Post_Request request);
        void UpdateMobileNo(Post_Request request);
        void UpdateAboutMe(Post_Request request);
        void UpdateHobbies(Post_Request request);
        void UpdateAreaOfInterest(Post_Request request);
        void UpdatePassiveIncome(Post_Request request);
        void UpdateCanImpartTraining(Post_Request request);
        void UpdateBirthDate(Post_Request request);
        void UpdateMaritalStatus(Post_Request request);
        void UpdateLanguage(Post_Request request);
        void UpdateKnowledgeSource(Post_Request request);
        void UpdateMentorCode(Post_Request request);
      //  void UpdateOrganisationType(Post_Request request);
      //  void UpdateUserType(Post_Request request);
        void UpdateProfileImage(Models.Partners.UpdateProfileImage.Post_Request request);
        void UpdatePartnerLocation(Models.Partners.UpdateLocation.Post_Request request);
        void UpdatePartnerAddress(Models.Partners.UpdateAddress.Post_Request request);
        void Update_Partner_AppVersion(Models.Partners.UpdatePartnerAppVersion.Post_Request request);

        bool Check_If_User_AppVersion_Exist(string UserId, string CurrentVersion);

    }
}
