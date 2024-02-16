using UJBHelper.DataModel;

namespace Partner.Service.Models.PartnerDetails
{
    public class Get_Request
    {
             
        public User userInfo { get; set; }
        public UserKYCDetails userKycInfo { get; set; }

        public UserOtherDetails userOtherDetails { get; set; }
        public bool isKYCComplete { get; set; }
        public string countryName { get; set; }
        public string stateName { get; set; }

        public string UserTypeValue { get; set; }
        public bool isRefer { get; set; }

    }

   
}
