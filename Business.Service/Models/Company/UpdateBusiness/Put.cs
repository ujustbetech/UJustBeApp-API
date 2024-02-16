using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Business.Service.Models.Company.UpdateBusiness
{
    public class Put_Request
    {
        public string businessId { get; set; }
        [Required]
        public string userId { get; set; }
        [Required]
        public string createdBy { get; set; }
        public List<string> categories { get; set; }
        public string tagLine { get; set; }
        public string companyName { get; set; }
        public string logoBase64 { get; set; }
        public string logoImageType { get; set; }
        public string logoImageName { get; set; }
        public string logoImageURL { get; set; }
        public string logoUniqueName { get; set; }
        public string BusinessEmail { get; set; }
        public string Location { get; set; }
        public string Flat_Wing { get; set; }
        public string Locality { get; set; }
        public string BusinessDescription { get; set; }
        public string WebsiteUrl { get; set; }
        public string GSTNumber { get; set; }
        public string PanNumber { get; set; }
        // public string PanBase64 { get; set; }
        //public string PanImageType { get; set; }

        public string FileName { get; set; }
        public string ImageURL { get; set; }
        public string UniqueName { get; set; }

        public int UserType { get; set; }
        public string NameofPartner { get; set; }
    }
}
