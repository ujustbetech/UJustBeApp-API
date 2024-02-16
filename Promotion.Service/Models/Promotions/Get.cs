using System.Collections.Generic;
using UJBHelper.DataModel;

namespace Promotion.Service.Models.Promotions
{
    public class Get_Request
    {
        public List<PromotionsList> PromotionList { get; set; }
        public DbPromotions _promotions { get; set; }
        public int totalCount { get; set; }
        public List<MediaList> PromotionMedia { get; set; }

        public List<PromotionsLPList> PromotionLPList { get; set; }
    }

   
}
