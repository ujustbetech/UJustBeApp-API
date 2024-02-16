using UJBHelper.DataModel;

namespace Auth.Service.Models.Registeration.User
{
    public class Get_Request
    {
        public Get_Request()
        {
            //businessDetails.bsnsAdd = new BusinessAddress();
        }
        public string userId { get; set; }
        public string emailId { get; set; }
        public string countryCode { get; set; }
        public string mobileNumber { get; set; }
        public string imgUrl { get; set; }
        public string base64Img { get; set; }
        public string imgType { get; set; }
        public string languagePreference { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        //public string currentStatus { get; set; }
        public string ujbId { get; set; }
        public bool isMentor { get; set; }
        public string password { get; set; }
        public string socialMediaId { get; set; }
        public string role { get; set; }
        public bool isKycAdded { get; set; }
        public string kycApprovalStatus { get; set; }
        public string gender { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public bool isActive { get; set; }
        public string myMentorCode { get; set; }
        public Address address { get; set; }
        public int noOfLeads { get; set; }
        public string MentorCode { get; set; }
        public string MentorName { get; set; }
        public bool isPartnerAgreementAccepted { get; set; }
       public bool isMembershipAgreementAccepted { get; set; }
        public string PartnerAgreementURL { get; set; }
        public string ListedPartnerAgreementURL { get; set; }       
        public bsnsDetails1 businessDetails { get; set; }     
    }

    public class bsnsDetails1
    {
        public string CompanyName { get; set; }
        public string BusinessEmail { get; set; }
        public BusinessAddress bsnsAdd { get; set; }
        public int useTypeId { get; set; }
        public string userType { get; set; }
        public string PartnerName { get; set; }
    } 
}
