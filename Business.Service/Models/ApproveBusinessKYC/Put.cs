using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Service.Models.ApproveBusinessKYC
{
    public class Put_Request
    {
        public string updatedBy { get; set; }
        public string businessId { get; set; }
        public int isApproved { get; set; }
        public string rejectedReason { get; set; }

        public bool isSubscriptionActive { get; set; }
    }
}
