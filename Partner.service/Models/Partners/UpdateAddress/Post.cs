using UJBHelper.DataModel;

namespace Partner.Service.Models.Partners.UpdateAddress
{
    public class Post_Request
    {
        public string UserId { get; set; }
        public Address address { get; set; }

        public int countryId { get; set; }


        public int stateId { get; set; }

    }
}
