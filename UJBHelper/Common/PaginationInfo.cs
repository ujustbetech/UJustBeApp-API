namespace UJBHelper.Common
{
    public class PaginationInfo
    {
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public bool IsPagingRequired { get; set; }        

        public PaginationInfo()
        {
            IsPagingRequired = true;
            PageSize = 5;
            CurrentPage = 0;
            TotalRecords = 0;
        }
    }
}
