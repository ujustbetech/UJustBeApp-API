using System.ComponentModel.DataAnnotations;

namespace Lead_Management.Service.Models.Referral
{
    public class Post_Request
    {
        [Required]
        public string businessId { get; set; }
        [Required]
        public string selectedProductId { get; set; }
        public string productServiceSlabId { get; set; }
        [Required]
        public string shortDescription { get; set; }
        [Required]
        public string referredByName { get; set; }
        [Required]
        public string referredById { get; set; }
        public string referredToName { get; set; }
        public string countryCode { get; set; }
        public string mobileNumber { get; set; }
        public string emailId { get; set; }
        public bool forSelf { get; set; }
        [Required]
        public string selectedProduct { get; set; }
    }
}
