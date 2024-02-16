using UJBHelper.DataModel;

namespace Partner.Service.Models.PartnerDetails.PartnerKYC
{
    public class Get_Request
    {


        public UserKYCDetails userKycInfo { get; set; }

       
        public bool isKYCComplete { get; set; }
        

    }

}
