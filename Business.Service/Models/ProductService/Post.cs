using System;
using System.Collections.Generic;
using UJBHelper.DataModel;

namespace Business.Service.Models.ProductService
{
    public class Post_Request
    {
        public string productId { get; set; }
        public string businessId { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public double productPrice { get; set; }
        public double minimumDealValue { get; set; }
        public int? shareType { get; set; }
        public string createdBy { get; set; }
        public bool isActive { get; set; }
        public List<UploadedProductImg> productImages { get; set; }
        public List<ProductInfo> productsOrServices { get; set; }
        public int? typeOf { get; set; }
        public bool isallowdelete { get; set; }
    }

    public class ProductInfo
    {
        public string productDetailsId { get; set; }
        public int? type { get; set; }
        public double? value { get; set; }
        public int? from { get; set; }
        public int? to { get; set; }
        public string productName { get; set; }
        public bool isActive { get; set; }
        public DateTime? createdOn { get; set; }
        public DateTime? updatedOn { get; set; }
    }
}
