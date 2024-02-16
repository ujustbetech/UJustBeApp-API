using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UJBHelper.DataModel;

namespace Partner.Service.Models.Partners.GetAllDetails
{
    public class Get_Details_Excel
    {
        public List<UserExcel> UserExcelInfo { get; set; }

        public List<ProductService_Excel> _productService_Excel { get; set; }

        public List<SubscriptionDetailsExcel> _subscriptionDetailsExcel { get; set; }
    }

    public class UserExcel
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string middleName { get; set; }
        public string emailId { get; set; }
        public string mobileNumber { get; set; }
        public int countryId { get; set; }
        public string countryName { get; set; }
        public int stateId { get; set; }
        public string StateName { get; set; }
        //  public string language { get; set; }
        public string gender { get; set; }
        public DateTime? birthDate { get; set; }
        //       public string preferredLocations { get; set; }       
        //  public string knowledgeSource { get; set; }
        public string passiveIncome { get; set; }
        //public string organisationType { get; set; }
        //public int userType { get; set; }
        //public string userTypeValue { get; set; }
        public Address address { get; set; }
        public string mentorCode { get; set; }
        public string MentorName { get; set; }
        public string myMentorCode { get; set; }
        public string Role { get; set; }
        public DateTime CreatedOn { get; set; }

        public bool isActive { get; set; }
        public string Active { get; set; }
        public string isActiveComment { get; set; }
        public string Hobbies { get; set; }
        public string can_impart_Training { get; set; }

        public string isApporved { get; set; }

        public DateTime KYVApproveOn { get; set; }
        public string isMembershipAgreementAccepted { get; set; }
        public string isPartnerAgreementAccepted { get; set; }

        public DateTime MembershipAgreementAcceptedDate { get; set; }
        public DateTime PartnerAgreementAcceptedDate { get; set; }

        public List<string> Categories { get; set; }

        public string Categories1 { get; set; }
        public string Categories2 { get; set; }
        public string Tagline { get; set; }
        public string CompanyName { get; set; }
        public string BusinessEmail { get; set; }
        public double? AverageRating { get; set; }
        public string Location { get; set; }
        public string Flat_Wing { get; set; }
        public string Locality { get; set; }
        public string PanNo { get; set; }
        public string WebSiteURL { get; set; }
        public string GSTNumber { get; set; }
        public string businessDescription { get; set; }
        public int BsnsIsApprovedFlag { get; set; }

        public string BsnsIsApproved { get; set; }
        public DateTime BsnsApproveOn { get; set; }
        public string BsnsIsAppovedReason { get; set; }
        public bool isSubscriptionActive { get; set; }
        public string SusbscriptionActive { get; set; }
        public string userType { get; set; }
        public string prodserve { get; set; }
        public DateTime BusinessRegisterationDate { get; set; }
        public string alternateMobileNumber { get; set; }
        public string alternateCountryCode { get; set; }
    }

    public class ProductService_Excel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string UJBCode { get; set; }

        public string Id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string singlemultiple { get; set; }

        public double productPrice { get; set; }
        public double minimumDealValue { get; set; }
        public string shareType { get; set; }
        public bool isActive { get; set; }
        public Created Created { get; set; }
        public Updated Updated { get; set; }


        public string prodservId { get; set; }
        public string percAmt { get; set; }
        public double? value { get; set; }

        public string from { get; set; }

        public string to { get; set; }
        public string productName { get; set; }


    }
    public class UserKYCExcel
    {
        public string PanCardNo { get; set; }
        public string AdharCardNo { get; set; }
        public bool KYCIsApprovedFlag { get; set; }
        public string KYCIsApproved { get; set; }
        public string KYCReason { get; set; }
        public int KYCReasonId { get; set; }
        public string KYCReasons { get; set; }
        public DateTime KYVApproveOn { get; set; }

    }

    public class UserOtherDetailExcel
    {
        public string maritalStatus { get; set; }
        public string nationality { get; set; }
        public string phoneNo { get; set; }
        public string Hobbies { get; set; }
        public string areaOfInterest { get; set; }
        public bool? canImpartTraining { get; set; }
        public string aboutMe { get; set; }

    }


    public class UserAgreementDetailExcel
    {
        public Boolean Accepted { get; set; }
        public DateTime AccetedOn { get; set; }


    }

    public class UserBusinessDetailsExcel
    {
        public List<string> Categories { get; set; }
        public string BussineesId { get; set; }
        public string Categories1 { get; set; }
        public string Categories2 { get; set; }
        public string Tagline { get; set; }
        public string CompanyName { get; set; }
        public string BusinessEmail { get; set; }
        public double? AverageRating { get; set; }
        public string Location { get; set; }
        public string Flat_Wing { get; set; }
        public string Locality { get; set; }
        public string PanNo { get; set; }
        public string WebSiteURL { get; set; }
        public string GSTNumber { get; set; }
        public int BsnsIsApprovedFlag { get; set; }
        public string BsnsIsApproved { get; set; }
        public DateTime BsnsApproveOn { get; set; }
        public string BsnsIsAppovedReason { get; set; }
        public bool isSubscriptionActive { get; set; }
        public string SusbscriptionActive { get; set; }
        public string businessDescription { get; set; }
        public int userTypeId { get; set; }
        public DateTime BusinessRegisterationDate { get; set; }

        public string FileName { get; set; }
        public string ImageURL { get; set; }
        public string UniqueName { get; set; }
    }


    public class ProductServiceExcel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string UJBCode { get; set; }

        public string bussinessId { get; set; }
        public List<ProductService> lstProductServiceExcel = new List<ProductService>();


    }

    public class ProductService
    {
        public string Id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public int? typeOf { get; set; }
        public List<ProductImg> ProductImg { get; set; }
        public double productPrice { get; set; }
        public double minimumDealValue { get; set; }
        public int? shareType { get; set; }
        public bool isActive { get; set; }
        public Created Created { get; set; }
        public Updated Updated { get; set; }

        public List<ProductServiceDetailsExcel> lstProductServiceDetailsExcel = new List<ProductServiceDetailsExcel>();

    }
    public class ProductServiceDetailsExcel
    {
        public string Id { get; set; }
        public string prodservId { get; set; }
        public int? type { get; set; }
        public double? value { get; set; }

        public int? from { get; set; }

        public int? to { get; set; }
        public string productName { get; set; }
        public Boolean isActive { get; set; }
        public Created created { get; set; }
        public Updated updated { get; set; }
    }


    public class SubscriptionDetails_Excel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string UJBCode { get; set; }
        public string bussinessId { get; set; }

        public DateTime RegisterationDate { get; set; }
        public DateTime BusinessRegisterationDate { get; set; }
        public DateTime? RenewalDate { get; set; }

        public double FeeAmount { get; set; }
        public double PaidAmount { get; set; }
        public double BalanceAmount { get; set; }

        public List<FeeDetails> Payment_details { get; set; }

        public string UserId { get; set; }

    }


    public class SubscriptionDetailsExcel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string UJBCode { get; set; }
        public string bussinessId { get; set; }

        public DateTime RegisterationDate { get; set; }
        public DateTime BusinessRegisterationDate { get; set; }
        public DateTime? RenewalDate { get; set; }

        public double FeeAmount { get; set; }
        public double PaidAmount { get; set; }
        public double BalanceAmount { get; set; }

        public double Amount { get; set; }
        public string TransactionID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMode { get; set; }
        public string UserId { get; set; }

    }




    public class FeeDetails
    {
        public double Amount { get; set; }
        public string TransactionID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMode { get; set; }


    }
}

