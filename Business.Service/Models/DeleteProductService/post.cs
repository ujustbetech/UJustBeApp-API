using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Business.Service.Models.DeleteProductService
{
    public class Post_Request
    {
       //// [required]
        public string ProductId { get; set; }
        public string ImgUniquename { get; set; }
    }
} 
