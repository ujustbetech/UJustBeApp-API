using Auth.Service.Models.Registeration.UploadBankDetails;
using Auth.Service.Respositories.Registeration;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using UJBHelper.DataModel;

namespace Auth.Service.Services.Registeration
{
    public class UploadBankDetailsService : IUploadBankDetailsService
    {
        private readonly IMongoCollection<UserKYCDetails> _userKYCDetails;
        private readonly IMongoCollection<User> _users;
        private IConfiguration _iconfiguration;

        public UploadBankDetailsService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _userKYCDetails = database.GetCollection<UserKYCDetails>("UserKYCDetails");
            _users = database.GetCollection<User>("Users");
        }

        public bool Check_If_All_Docs_Uploaded(string userId)
        {
            var kyc = _userKYCDetails.Find(x => x.UserId == userId).FirstOrDefault();

            if (!string.IsNullOrEmpty(kyc.PanCard.ImageURL) && !string.IsNullOrEmpty(kyc.PanCard.PanNumber) && !string.IsNullOrEmpty(kyc.AdharCard.BackImageURL) && !string.IsNullOrEmpty(kyc.AdharCard.FrontImageURL) && !string.IsNullOrEmpty(kyc.AdharCard.AdharNumber) && !string.IsNullOrEmpty(kyc.BankDetails.AccountNumber) && !string.IsNullOrEmpty(kyc.BankDetails.BankName) && !string.IsNullOrEmpty(kyc.BankDetails.AccountHolderName) && !string.IsNullOrEmpty(kyc.BankDetails.IFSCCode) && !string.IsNullOrEmpty(kyc.BankDetails.ImageURL))
            {
                return true;
            }
            return false;
        }

        public bool Check_If_User_Exists(string userId)
        {
            return _users.Find(x => x._id == userId).CountDocuments() > 0;
        }

        public void Update_Bank_Details(Post_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            if (_userKYCDetails.Find(x => x.UserId == request.userId).CountDocuments() < 1)
            {
                var kyc = new UserKYCDetails
                {
                    UserId = request.userId,
                    BankDetails = new BankDetails
                    {
                        AccountHolderName = request.BankDetails.accountHolderName,
                        AccountNumber = request.BankDetails.accountNumber,
                        BankName = request.BankDetails.bankName,
                        FileName = request.BankDetails.FileName,
                        ImageURL = request.BankDetails.ImageURL,
                        UniqueName = request.BankDetails.UniqueName
                        //Cancelchequebase64Img = request.BankDetails.cancelChequebase64Img,
                        //CancelchequeimgType = request.BankDetails.cancelchequeimgType,
                        //IFSCCode = request.BankDetails.IFSCCode
                    },
                    AdharCard = new AdharCard(),
                    IsApproved = new IsApproved(),
                    PanCard = new PanCard(),
                    Created = new Created
                    {
                        created_By = request.userId,
                        created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))

                    },
                    Updated = new Updated()
                };

                _userKYCDetails.InsertOne(kyc);
            }
            else
            {
                _userKYCDetails.FindOneAndUpdate(
             Builders<UserKYCDetails>.Filter.Eq(x => x.UserId, request.userId),
             Builders<UserKYCDetails>.Update
             .Set(x => x.BankDetails, new BankDetails
             {
                 AccountHolderName = request.BankDetails.accountHolderName,
                 AccountNumber = request.BankDetails.accountNumber,
                 BankName = request.BankDetails.bankName,
                 FileName = request.BankDetails.FileName,
                 ImageURL = request.BankDetails.ImageURL,
                 UniqueName = request.BankDetails.UniqueName,
                 IFSCCode = request.BankDetails.IFSCCode
             })
             .Set(x => x.Updated, new Updated
             {
                 updated_By = request.userId,
                 updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
             })
             );
            }
        }
    }
}
