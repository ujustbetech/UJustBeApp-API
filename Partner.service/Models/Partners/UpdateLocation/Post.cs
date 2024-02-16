using UJBHelper.DataModel;

namespace Partner.Service.Models.Partners.UpdateLocation
{
    public class Post_Request
    {
        public string UserId { get; set; }
        public PreferredLocations locations { get; set; }
    }
}
