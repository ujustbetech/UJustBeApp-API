using System.Collections.Generic;

namespace UJBHelper.DataModel.Common
{
    public class FCM_Response
    {
        public long multicast_id { get; set; }
        public int success { get; set; }
        public int failure { get; set; }
        public int canonical_ids { get; set; }
        public List<Response_Info> results { get; set; }
    }

    public class Response_Info
    {
        public string error { get; set; }
        public string message_id { get; set; }
    }
}
