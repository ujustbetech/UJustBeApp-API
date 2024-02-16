using System;

namespace Reports.Service.Models.NativeError
{
    public class Post_Request
    {
        public string UserId { get; set; }
        public string Url { get; set; }
        public string screen { get; set; }
        public string method { get; set; }
        public string message { get; set; }
        public string error { get; set; }
        public DateTime date { get; set; }
        public string source { get; set; }
        public string createdBy { get; set; }
    }
}
