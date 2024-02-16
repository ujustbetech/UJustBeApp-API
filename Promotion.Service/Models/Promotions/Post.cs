using System.Collections.Generic;
using UJBHelper.DataModel;
using System;

namespace Promotion.Service.Models.Promotions
{
    public class Post_Request
    {
        public Post_Request()
        {
            Media1 = new List<PromotionMedia>();
        }
        public string PromotionId { get; set; }
        public string userId { get; set; }
       public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string IsActive { get; set; }        
        public string ReferenceUrl { get; set; }
        public string productServiceId { get; set; }        
        public string created_By { get; set; }
        public List<PMedia> Media { get; set; }
        public List<PromotionMedia> Media1 { get; set; }
    }

    public class PMedia
    {
        public string FileName { get; set; }
        public string Base64string { get; set; }
        public string FileUniqueName { get; set; }        
        public string FileURL { get; set; }
    }
}
