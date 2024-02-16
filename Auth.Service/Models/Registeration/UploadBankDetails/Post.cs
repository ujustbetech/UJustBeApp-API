using System.ComponentModel.DataAnnotations;

namespace Auth.Service.Models.Registeration.UploadBankDetails
{
    public class Post_Request
    {
        [Required]
        public string userId { get; set; }
        public BankDetailsInfo BankDetails { get; set; }
    }

    public class BankDetailsInfo
    {
        public string accountHolderName { get; set; }
        public string accountNumber { get; set; }
        public string bankName { get; set; }
        public string IFSCCode { get; set; }

        public string FileName { get; set; }
        public string ImageURL { get; set; }
        public string UniqueName { get; set; }
        public string cancelChequebase64Img { get; set; }
      
    }
}
