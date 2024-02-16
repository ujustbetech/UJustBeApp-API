using Business.Service.Models.DeleteProductService;
using System.Collections.Generic;

namespace Business.Service.Repositories.DeleteProductService
{
    public interface IDeleteProductService
    {
        void Delete_Products_service(string ProdServiceId);
        void Delete_ProductsDetails(string ProdServiceId);
        bool Check_If_ProductDetails_Exists(string productDeatailsId);
        void Delete_Products_service_Images(Post_Request request);

        bool Check_If_Product_Image_Exists(Post_Request request);
        bool Check_If_Product_Exists(string productId);
        bool Check_If_Referral_Exists(string productId);
    }
}
