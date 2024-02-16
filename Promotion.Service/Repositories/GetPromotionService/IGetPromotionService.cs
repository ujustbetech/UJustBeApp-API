using Promotion.Service.Models.Promotions;

namespace Promotion.Service.Repositories.GetPromotionService
{
    public interface IGetPromotionService
    {
        Get_Request Get_PromotionList(int size, int page, string userId);

        Get_Request Get_PromotionsDetails(string PromotionId);

        Get_Request Get_PromotionMedia();

    }
    
}
