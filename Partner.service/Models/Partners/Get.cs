using System;
using System.Collections.Generic;
using UJBHelper.DataModel;

namespace Partner.Service.Models.Partners
{
    public class Get_Request
    {
        public List<PartnerUsers> PartnersList { get; set; }        
        public int totalCount { get; set; }
       
    }

    public class PartnerUsers
    {
        public string _id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string emailId { get; set; }
        public string mobileNumber { get; set; }
        public string gender { get; set; }
        public DateTime? birthDate { get; set; }
        public string passiveIncome { get; set; }
        public Address address { get; set; }
        public string Role { get; set; }
        public bool isActive { get; set; }       
        public bool isApproved { get; set; }

        public int ReasonId { get; set; } 
        public string Reason { get; set; }
        public string UJBCode { get; set; }
        public Created Created { get; set; }
        public bool isMembershipAgreementAccepted { get; set; }
        public bool isPartnerAgreementAccepted { get; set; }
        public bool isKycComplete { get; set; }

        public string isBusinessApproved { get; set; }
        public string BusinessApprovalReason { get; set; }
        public int order_by { get; set; }
        public int is_Active { get; set; }

        public bool isBankComplete { get; set; }
        //  public Updated Updated { get; set; }
    }


}
