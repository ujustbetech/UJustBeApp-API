using System.Collections.Generic;

namespace Search.Service.Models.Dashboard
{
    public class Post_Request
    {
        public List<string> categoryIds { get; set; }
        public string searchTerm { get; set; }
        public SortValue? sortValue { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public SearchType SearchType { get; set; }
        public string userId { get; set; }
        public int skipTotal { get; set; }
    }

    public enum SearchType
    {
        Dashboard,
        Empty,
        Advanced
    }

    public enum SortValue
    {
        Rating,
        NearToFar,
        Ascending,
        Descending
    }
}
