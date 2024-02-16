using System.Collections.Generic;
using UJBHelper.DataModel;

namespace Business.Service.Models.ClientPartner
{
    public class Get_Request
    {
        public Get_Request()
        {
            categories = new List<Category_Info>();
        }
        public string userId { get; set; }
        public string businessId { get; set; }
        //public string ujbId { get; set; }
        public double? rating { get; set; }
        //public List<string> categories { get; set; }
        public List<Category_Info> categories { get; set; }
        public string businessName { get; set; }
        public string tagline { get; set; }
        public BusinessAddress address { get; set; }
        public string businessDescription { get; set; }
        public string businessUrl { get; set; }
        public string businessEmail { get; set; }
        public BusinessPan businessPan { get; set; }
        public Logo businessLogo { get; set; }
        public string businessGST { get; set; }
        public Banner BannerDetails { get; set; }
        public string myMentorCode { get; set; }
        public Approved IsApproved { get; set; }
        public bool isSubscriptionActive { get; set; }
        public bool isFeePending { get; set; }

        public int userTypeId { get; set; }
        public string userType { get; set; }
        public string NameofPartner { get; set; }
        public List <string> CategoriesId { get; set; }
        public bool isRefer { get; set; }

    }

    public class Category_Info
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Request_User
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string myMentorcode { get; set; }
        public int countryId { get; set; }
    }
}
