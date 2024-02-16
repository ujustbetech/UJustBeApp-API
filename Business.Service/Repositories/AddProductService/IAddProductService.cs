using Business.Service.Models.ProductService;
using System.Collections.Generic;


namespace Business.Service.Repositories.ProductService
{
    public interface IAddProductService
    {
        bool Check_If_Product_Exists(string productId);
        void Insert_New_Product(Post_Request request);
        string Update_Product_Details(Post_Request request);
        void Update_Product_Images(Put_Request request);
        bool Check_If_User_Exists(string userId);
        List<Get_Request> Get_Products_By_User(string userId);
        List<Get_Request> Get_Services_By_User(string userId);
        Get_Request Get_Product_Service_Details(string prodServiceId);
        List<Get_Request> Get_Products_Services_By_User(string userId);
        bool Check_If_Referral_Exists(string productId);



    }
}
