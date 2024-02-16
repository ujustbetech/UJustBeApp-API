using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UJBHelper.DataModel;

namespace Business.Service.Models.Company.UpdateBusiness
{
    public class Get_Request
    {
        public string businessId { get; set; }
        [Required]
        public string userId { get; set; }
        [Required]
        public string createdBy { get; set; }
        public List<string> categories { get; set; }
        public string tagLine { get; set; }
        public string companyName { get; set; }
        public string BusinessEmail { get; set; }
        public string BusinessDescription { get; set; }
        public string WebsiteUrl { get; set; }
        public string GSTNumber { get; set; }
        public Logo Logo { get; set; }
        public BusinessAddress Address { get; set; }
        public BusinessPan Pan { get; set; }
        public int UserTypeId { get; set; }
        public string UserType { get; set; }
        public string NameOfPartner { get; set; }
    }
}
