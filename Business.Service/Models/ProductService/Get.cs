using System;
using System.Collections.Generic;
using UJBHelper.DataModel;

namespace Business.Service.Models.ProductService
{
    public class Get_Request
    {
        public Get_Request()
        {
            productsOrServices = new List<ProductInfo>();
        }
        public string productId { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public double productPrice { get; set; }
        public double minimumDealValue { get; set; }
        public bool isActive { get; set; }
        public List<ProductImg> productImages { get; set; }
        public List<ProductInfo> productsOrServices { get; set; }
        public int? shareType { get; set; }
        public int? typeOf { get; set; }
        public Created Created { get; set; }
        public Updated Updated { get; set; }
        public bool IsAllowDelete { get; set; }
}
}
