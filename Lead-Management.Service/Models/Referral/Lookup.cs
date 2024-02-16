using System.Collections.Generic;

namespace Lead_Management.Service.Models.Referral
{
    public class Lookup_Request
    {
        public List<int> status { get; set; }
        public string query { get; set; }
        public List<string> categoryIds { get; set; }
        public string referred { get; set; }
        public string userId { get; set; }
        public int CurrentPage { get; set; }
    }
}
