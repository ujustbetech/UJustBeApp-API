using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace UJBHelper.DataModel
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string middleName { get; set; }
        public string emailId { get; set; }
        public string mobileNumber { get; set; }
        public string countryCode { get; set; }
        public int countryId { get; set; }
        public int stateId { get; set; }
        public string password { get; set; }
        public string language { get; set; }
        public string gender { get; set; }
        public DateTime? birthDate { get; set; }
        public PreferredLocations preferredLocations { get; set; }

        public string FileName { get; set; }
        public string ImageURL { get; set; }
        public string UniqueName { get; set; }
        /// public string imageUrl { get; set; }
        //public string base64Image { get; set; }            
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string imageType { get; set; }
        public string knowledgeSource { get; set; }
        public string passiveIncome { get; set; }
        public string organisationType { get; set; }
        public int userType { get; set; }
        public Address address { get; set; }
        public string mentorCode { get; set; }
        public OtpVerification otpVerification { get; set; }
        public string myMentorCode { get; set; }
        public string Role { get; set; }
        public SocialLogin socialLogin { get; set; }
        public Created Created { get; set; }
        public Updated Updated { get; set; }
        public string Registered_By { get; set; }
        public bool isActive { get; set; }
        [BsonIgnore]
        public double distance { get; set; }
        public string fcmToken { get; set; }

        public bool isMembershipAgreementAccepted { get; set; }
        public bool isPartnerAgreementAccepted { get; set; }
        public string currentAppVersion { get; set; }
        public string alternateMobileNumber { get; set; }
        public string alternateCountryCode { get; set; }
        public string isActiveComment { get; set; }
    }

    public class PreferredLocations
    {
        public string location1 { get; set; }
        public string location2 { get; set; }
        public string location3 { get; set; }
    }

    //public class ProfileImage
    //{
    //    public string ImageName { get; set; }
    //    public string ImageType { get; set; }
    //    public string ImageBase64 { get; set; }
    //}

    public class Address
    {
        public string location { get; set; }
        public string flatWing { get; set; }
        public string locality { get; set; }

        //  public int stateId { get; set; }
    }

    public class OtpVerification
    {
        public bool OTP_Verified { get; set; }
        public string OTP { get; set; }
    }

    public class SocialLogin
    {
        public bool Is_Social_Login { get; set; }
        public string Social_Site { get; set; }
        public string Social_Code { get; set; }
    }

    public class Created
    {
        public string created_By { get; set; }
        public DateTime created_On { get; set; }
    }

    public class Updated
    {
        public string updated_By { get; set; }
        public DateTime? updated_On { get; set; }
    }
}
