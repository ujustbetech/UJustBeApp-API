using Auth.Service.Models.Registeration.UploadAadhar;
using Auth.Service.Respositories.Registeration;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using UJBHelper.DataModel;

namespace Auth.Service.Services.Registeration
{
    public class UploadAadharService : IUploadAadharService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<UserKYCDetails> _userKYCDetails;
        private IConfiguration _iconfiguration;
        public UploadAadharService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _users = database.GetCollection<User>("Users");
            _userKYCDetails = database.GetCollection<UserKYCDetails>("UserKYCDetails");
        }

        public bool Check_If_All_Docs_Uploaded(string userId)
        {
            var kyc = _userKYCDetails.Find(x => x.UserId == userId).FirstOrDefault();

            if(!string.IsNullOrEmpty(kyc.PanCard.ImageURL)&& !string.IsNullOrEmpty(kyc.PanCard.PanNumber) && !string.IsNullOrEmpty(kyc.AdharCard.BackImageURL)  && !string.IsNullOrEmpty(kyc.AdharCard.FrontImageURL) && !string.IsNullOrEmpty(kyc.AdharCard.AdharNumber) &&  !string.IsNullOrEmpty(kyc.BankDetails.AccountNumber) && !string.IsNullOrEmpty(kyc.BankDetails.BankName) && !string.IsNullOrEmpty(kyc.BankDetails.AccountHolderName) && !string.IsNullOrEmpty(kyc.BankDetails.IFSCCode) && !string.IsNullOrEmpty(kyc.BankDetails.ImageURL))
            {
                return true;
            }
            return false;
        }

        public bool Check_If_User_Exists(string userId)
        {
            return _users.Find(x => x._id == userId).CountDocuments() > 0;
        }

        public void Update_Aadhar_Details(Post_Request request)
        {

            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            if (_userKYCDetails.Find(x => x.UserId == request.userId).CountDocuments() == 0)
            {
                var kyc = new UserKYCDetails
                {
                    UserId = request.userId,
                    AdharCard = new AdharCard
                    {
                        FrontFileName = request.FrontFileName,
                        BackFileName = request.BackFileName,
                        FrontImageURL = request.FrontImageURL,
                        BackImageURL = request.BackImageURL,
                        FrontUniqueName = request.FrontUniqueName,
                        BackUniqueName = request.BackUniqueName,
                        AdharNumber = request.aadharNumber
                    },
                    BankDetails = new BankDetails(),
                    Updated = new Updated(),
                    IsApproved = new IsApproved(),
                    PanCard = new PanCard(),
                    Created = new Created
                    {
                        created_By = request.userId,
                        created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))

                    }
                };

                _userKYCDetails.InsertOne(kyc);
            }
            else
            {
                _userKYCDetails.FindOneAndUpdate(
             Builders<UserKYCDetails>.Filter.Eq(x => x.UserId, request.userId),
             Builders<UserKYCDetails>.Update
             .Set(x => x.AdharCard, new AdharCard
             {
                 FrontFileName = request.FrontFileName,
                 BackFileName = request.BackFileName,
                 FrontImageURL=request.FrontImageURL,
                 BackImageURL=request.BackImageURL,
                 FrontUniqueName = request.FrontUniqueName,
                 BackUniqueName = request.BackUniqueName,                
                 AdharNumber = request.aadharNumber
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
