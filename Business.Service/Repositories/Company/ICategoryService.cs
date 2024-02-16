using Business.Service.Models.Company.Category;
using UJBHelper.Common;

namespace Business.Service.Repositories.Company
{
    public interface ICategoryService
    {
        Get_Request Get_Categories(string query, PaginationInfo pager);
    }
}
