using System.Collections.Generic;
using UJBHelper.DataModel;

namespace Partner.Service.Models.Partners.GetConnectors
{
    public class Get_Connector_Request
    {
        //public string MyMentorCode { get; set; }
        //public string MentorCode { get; set; }
        public List<Connector> MentorUserInfo { get; set; }
        public List<Connector> ConnectorUserInfo { get; set; }

        public int NoOfConnects { get; set; }
    }

    public class Connector
    {
        public string Role { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string emailId { get; set; }
        public string countryCode { get; set; }
        public string mobileNumber { get; set; }
        public string language { get; set; }
        public string _id { get; set; }
        public string mentorCode { get; set; }
        public string myMentorCode { get; set; }
        public string imageURL { get; set; }
        public string base64Image { get; set; }
        public string imageType { get; set; }

        public Address address { get; set; }
    }
}
