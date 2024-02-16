using System.Collections.Generic;
using UJBHelper.DataModel;

namespace Search.Service.Models.Dashboard
{
    public class Get_Request
    {
        public Get_Request()
        {
            businessList = new List<Business_Info>(); 
        }
        public List<Business_Info> businessList { get; set; }
        public int listCount { get; set; }
        public bool Is_Active { get; set; }
    }

    public class Business_Info
    {
        public Business_Info()
        {
            categories = new List<string>();
            categories_details= new List<Category_Details>();
        }
        public string userId { get; set; }
        public string businessId { get; set; }
        //public string ujbId { get; set; }
        public double? rating { get; set; }       
        public List<string> categories { get; set; }
        public List<Category_Details> categories_details { get; set; }
        public string businessName { get; set; }
        public string tagline { get; set; }
        public BusinessAddress address { get; set; }
        public string businessDescription { get; set; }
        public string businessUrl { get; set; }
        public List<Share_Details> shareDetails { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public Logo Logo { get; set; }
        public double distance { get; set; }
    }

    public class Share_Details
    {
        public string shareType { get; set; }
        public string value { get; set; }
    }

    public class Business
    {
        public string businessId { get; set; }
        public string businessName { get; set; }
    }

    public class Category_Details
    {
        public string Id { get; set; }
        public string categoryName { get; set; }
        public bool PercentageShare { get; set; }
    }
}
