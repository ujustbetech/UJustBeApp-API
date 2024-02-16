using System.Collections.Generic;
using UJBHelper.DataModel;
using System;
namespace Lead_Management.Service.Models.AdminReferral
{
    public class Get_Request
    {
        public Get_Request()
        {
            ReferredByMeList = new List<Request_Info>();
            ReferredBusinessList = new List<Request_Info>();
        }
        public List<Request_Info> ReferredByMeList { get; set; }
        public List<Request_Info> ReferredBusinessList { get; set; }
       
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
    }


  }
