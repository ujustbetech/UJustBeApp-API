using System.Collections.Generic;

namespace Business.Service.Models.ProductService
{
    public class Put_Request
    {
        public string productId { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public string updatedBy { get; set; }
    }

    public class ProductImage
    {
        public string prodImgBase64 { get; set; }
        public string prodImgName { get; set; }
        public bool isDefaultImg { get; set; }
    }
}
