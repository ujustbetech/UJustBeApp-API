using UJBHelper.DataModel;

namespace Partner.Service.Models.PartnerDetails.PartnerProfile
{
    public class Get_Request
    {
             
        public User userInfo { get; set; }
       

        public UserOtherDetails userOtherDetails { get; set; }
        
        public string countryName { get; set; }
        public string stateName { get; set; }

        public string UserTypeValue { get; set; }
       

    }

  
}
