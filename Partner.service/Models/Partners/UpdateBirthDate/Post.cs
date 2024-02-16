using System;
using UJBHelper.DataModel;

namespace Partner.Service.Models.Partners.UpdateBirthDate
{
    public class Post_Request
    {
        public string userId { get; set; }
        public string type { get; set; }
        public DateTime? value { get; set; }
    }
}
