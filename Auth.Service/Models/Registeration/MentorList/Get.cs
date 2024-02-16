using UJBHelper.DataModel;

namespace Auth.Service.Models.Registeration.MentorList
{
    public class Get_Request
    {
        public string userId { get; set; }
        public string Base64_Image { get; set; }
        public string FullName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string mentorCode { get; set; }
        public int NoOfConnects { get; set; }
        public Address address { get; set; }
    }
}
