using System;
using System.Collections.Generic;

namespace Notification.Service.Models
{
    public class Get_Request
    {
        public List<Request_Info> notifications { get; set; }
        public int totalCount { get; set; }
        public int totalUnreadCount { get; set; }
    }

    public class Request_Info
    {
        public string id { get; set; }
        public string message { get; set; }
        public DateTime date { get; set; }
        public string type { get; set; }
        public bool isRead { get; set; }
        public bool isSystemGenerated { get; set; }
        public string leadId { get; set; }
        public bool isReferredByMe { get; set; }
    }
}
