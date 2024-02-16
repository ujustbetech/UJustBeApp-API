using System.Collections.Generic;
using UJBHelper.DataModel;
using UJBHelper.Common;
using System;

namespace Lead_Management.Service.Models.Referral
{
    public class Get_Request
    {
        public Get_Request()
        {
            clientPartnerDetails = new ClientPartnerDetails();
            referredToDetails = new ReferredTo();
            referredByDetails = new ReferredByDetails();
            StatusHistories = new List<ReferralStatusHistory>();
            mentorDetails = new MentorDetails();
            productDetails = new ProductDetails();
            shareReceivedByUJB = new ShareReceivedByUJB();
            shareReceivedByPartner = new ShareRecievedByPartners();
            sharedPercentageDetails = new List<SharedPercentageDetails>();

            ProjectiveshareReceivedByPartner = new ShareRecievedByPartners();
            Pager = new PaginationInfo();

        }
        public string referralId { get; set; }
        public string refMultisSlabProdId { get; set; }
        public double TotalCPAmtReceived { get; set; }
        public double TotalUJBAmtReceived { get; set; }
        public string PartnerId { get; set; }
        public string MentorId { get; set; }

        public string LPMentorId { get; set; }
        public double TotalPartnerAmtPaid { get; set; }
        public double TotalMentorAmtPaid { get; set; }

        public double TotalLPMentorAmtPaid { get; set; }
        public List<string> categories { get; set; }
        public string dateCreated { get; set; }
        public int refStatus { get; set; }
        public string referralStatusValue { get; set; }
        public string productId { get; set; }
        public string productName { get; set; }
        public string referralDescription { get; set; }
        public bool isForSelf { get; set; }
        public string businessId { get; set; }
        public string businessName { get; set; }
        public string ReferralCode { get; set; }
        public ClientPartnerDetails clientPartnerDetails { get; set; }
        public ReferredTo referredToDetails { get; set; }
        public ReferredByDetails referredByDetails { get; set; }
        public List<ReferralStatusHistory> StatusHistories { get; set; }

        public string rejectedReason { get; set; }
        public string dealValue { get; set; }
        public double CalcDealValue { get; set; }
        public MentorDetails mentorDetails { get; set; }
        public MentorDetails LPMentorDetails { get; set; }
        public ProductDetails productDetails { get; set; }
        public ShareReceivedByUJB shareReceivedByUJB { get; set; }

        public ShareRecievedByPartners shareReceivedByPartner { get; set; }

        public int dealStatus { get; set; }
        public string referralStatusUpdatedOn { get; set; }
        public string referralStatusUpdatedby { get; set; }
        public string DealStatusUpdatedOn { get; set; }
        public string dealStatusValue { get; set; }

        public PaginationInfo Pager { get; set; }
        public List<SharedPercentageDetails> sharedPercentageDetails { get; set; }

        public ShareRecievedByPartners ProjectiveshareReceivedByPartner { get; set; }
        public ShareRecievedByPartners PramotionalshareRecievedByPartners { get; set; }
        public ShareRecievedByPartners TotalshareRecievedByPartners { get; set; }
        public DateTime referralcreatedDate { get; set; }

        public string inActiveUsers { get; set; }
    }

    public class LeadProductInfo
    {
        public int? typeOf { get; set; }
        public int? shareType { get; set; }
        public List<LeadProductServiceDetails> productsOrServices = new List<LeadProductServiceDetails>();
    }
    public class MentorDetails
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }

        public string CountryCode { get; set; }
    }

    public class ProductDetails
    {
        public string name { get; set; }
        public double price { get; set; }
        public int? shareType { get; set; }
        public string id { get; set; }
    }

    public class ClientPartnerDetails
    {
        public string name { get; set; }
        public string mobileNumber { get; set; }
        public string emailId { get; set; }
        public string tagline { get; set; }
        public string CountryCode { get; set; }
        public string UserId { get; set; }
        public string companyName { get; set; }
    }

    public class ReferredByDetails
    {
        public string referredByName { get; set; }
        public string referredByEmailId { get; set; }
        public string referredByMobileNo { get; set; }
        public string referredByCountryCode { get; set; }
        public string referredByUserId { get; set; }
        public string referredByRole { get; set; }
    }

    public class ReferralStatusHistory
    {
        public DateTime date { get; set; }
        public string status { get; set; }
        public int statusCode { get; set; }
    }


    public class SharedPercentageDetails
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public double percentage { get; set; }

        public int sharedId { get; set; }
        public bool isActive { get; set; }
    }

}
