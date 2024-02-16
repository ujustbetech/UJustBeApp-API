namespace Reports.Service.Models.Dashboard
{
    public class Post_Request
    {
        public Post_Request()
        {
            businessStats = new BusinessStatsInfo();
            refsEarned = new ReferralEarnedInfo();
        }
        public ReferralEarnedInfo refsEarned { get; set; }
        public BusinessStatsInfo businessStats { get; set; }
        public string refsGiven { get; set; }
        public string dealsClosed { get; set; }

        public string refsEarnedTotal { get; set; }
    }

    public class ReferralEarnedInfo
    {
        public string activeIncome { get; set; }
        public string passiveIncome { get; set; }
    }

    public class BusinessStatsInfo
    {
        public string totalBusinessClosed { get; set; }
        public string totalDealsClosed { get; set; }
    }
}
