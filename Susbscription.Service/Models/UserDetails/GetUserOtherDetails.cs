using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Susbscription.Service.Models.UserDetails
{
    public class GetUserOtherDetails
    {
        public string leadId { get; set; }
        public string Name { get; set; }
        public string mobileNumber { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime RegisterationDate { get; set; }
        public DateTime BusinessRegisterationDate { get; set; }
        public DateTime? RenewalDate { get; set; }
    }
}
