using System.Collections.Generic;
using UJBHelper.DataModel;
using UJBHelper.Common;
using System;


namespace Reports.Service.Models.ReferralTracking
{
    public class Get_Request
    {
        public Get_Request()
        {
            ReferredList = new List<Request_Info>();

        }

        public List<Request_Info> ReferredList { get; set; }


    }
    public class Post_Request
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string StatusID { get; set; }
    }

    public class Put_Request
    {

        public string[] ReferralIdID { get; set; }
    }
    public class Request_Info
    {
        public string referralId { get; set; }
        public List<string> categories { get; set; }
        public string dateCreated { get; set; }
        public int refStatus { get; set; }
        public string productId { get; set; }
        public string productName { get; set; }
        public string referralDescription { get; set; }
        public bool isForSelf { get; set; }
        public string businessId { get; set; }
        public string businessName { get; set; }
        public string dealValue { get; set; }
        public bool isAccepted { get; set; }
        public string rejectionReason { get; set; }
        public ReferredTo referredToDetails { get; set; }
        public ReferredByDetails referredByDetails { get; set; }

        public ClientPartnerDetails clientPartnerDetails { get; set; }

        public int dealStatus { get; set; }
        public DateTime referralStatusUpdatedOn { get; set; }
        public string referralStatusUpdatedby { get; set; }
        public string ReferralCode { get; set; }
        public string referralStatusValue { get; set; }
        public string dealStatusValue { get; set; }

        public string DealStatusUpdatedOn { get; set; }


    }

    public class ReferredByDetails
    {
        public string referredByName { get; set; }
        public string referredByEmailId { get; set; }
        public string referredByMobileNo { get; set; }
        public string referredByCountryCode { get; set; }
    }

    public class ClientPartnerDetails
    {
        public string name { get; set; }
        public string mobileNumber { get; set; }
        public string emailId { get; set; }
        public string tagline { get; set; }
        public string countryCode { get; set; }
        public string BussEmailId { get; set; }
    }

    public class Put_Response
    {
        public Put_Response()
        {
            ReferredList = new List<Put_Response_Info>();

        }

        public List<Put_Response_Info> ReferredList { get; set; }


    }

    public class Put_Response_Info
    {
        public string referralId { get; set; }
        public List<string> categories { get; set; }
        public string dateCreated { get; set; }
        public int refStatus { get; set; }
        public string productId { get; set; }
        public string productName { get; set; }
        public string referralDescription { get; set; }
        public bool isForSelf { get; set; }
        public string businessId { get; set; }
        public string businessName { get; set; }
        public string dealValue { get; set; }
        public bool isAccepted { get; set; }
        public string rejectionReason { get; set; }
        public ReferredTo referredToDetails { get; set; }
        public ReferredByDetails referredByDetails { get; set; }
        public MentorDetails PartnerMentorDetails { get; set; }
        public ClientPartnerDetails clientPartnerDetails { get; set; }
        public MentorDetails LPMentorDetails { get; set; }
        public int dealStatus { get; set; }
        public DateTime referralStatusUpdatedOn { get; set; }
        public string referralStatusUpdatedby { get; set; }
        public string ReferralCode { get; set; }
        public string referralStatusValue { get; set; }
        public string dealStatusValue { get; set; }
        public double SharedValuewithUJB { get; set; }
        public int SharedwithUJB { get; set; }
        public string DealStatusUpdatedOn { get; set; }
        public double Amount_Recieved_by_lp { get; set; }
        public DateTime? Amount_Recieved_by_lp_Payment_Date { get; set; }
        public string Amount_Recieved_by_lp_Payment_Mode { get; set; }
        public double Amount_Transfered_to_ujb_Amount { get; set; }
        public double UJb_transfered_to_partner { get; set; }
        public DateTime? UJb_transfered_to_partner_payment_Date { get; set; }
        public string UJb_transfered_to_partner_payment_mode { get; set; }
        public double Amount_Ujb_transfered_to_partner_mentor { get; set; }
        public DateTime? Amount_Ujb_transfered_to_partner_mentor_payment_Date { get;set;}
        public string Amount_Ujb_transfered_to_partner_mentor_payment_mode { get;set;}
        public double  Amount_ujb_ransfered_to_lpmentor  { get;set;}
        public double Balance_remaining_with_UJB { get; set; }
        public DateTime? Amount_ujb_ransfered_to_lpmentor_payment_Date { get; set; }
        public string Amount_ujb_ransfered_to_lpmentor_payment_mode { get; set; }

    }
}

public class MentorDetails
{
    public string Name { get; set; }
    public string EmailId { get; set; }
    public string MobileNo { get; set; }
    public string CountryCode { get; set; }
}


