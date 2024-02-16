using System.Collections.Generic;
using UJBHelper.Common;

namespace Business.Service.Models.Company.Category
{
    public class Get_Request
    {
        public List<Category_Info> categories { get; set; }
        public int totalCount { get; set; }
        public PaginationInfo Pager { get; set; }
    }

    public class Category_Info
    {
        public string catId { get; set; }
        public bool PercentageShare { get; set; }
        public string categoryName { get; set; }
        public string categoryImgBase64 { get; set; }
    }
}
