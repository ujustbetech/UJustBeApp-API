using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Business.Service.Models.Company.UpdateBusiness
{
    public class Post_Request
    {
        [Required]
        public string userId { get; set; }
        public string businessId { get; set; }
        public List<string> categories { get; set; }
        public string tagline { get; set; }
        public int UserType { get; set; }
        public string NameofPartner { get; set; }
        public string CompanyName { get; set; }
        public string location { get; set; }
        public string flatWing { get; set; }
        public string locality { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string BusinessDescription { get; set; }


    }
}
