using Promotion.Service.Models.Promotions;

namespace Promotion.Service.Repositories.PromotionService
{
    public interface IAddPromotionService
    {
        void Insert_New_Promotion(Post_Request request);
        void Update_Promotions_Details(Post_Request request);
        Get_Request Get_PromotionListedPartnerList();

    }
}

