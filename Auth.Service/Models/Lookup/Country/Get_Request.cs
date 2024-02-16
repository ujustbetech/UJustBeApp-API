using System.Collections.Generic;
using UJBHelper.DataModel;

namespace Auth.Service.Models.Lookup.Country
{
    public class Get_Request
    {
        public List<CountryInfo> countries { get; set; }
        public int totalCount { get; set; }
    }

   
}
