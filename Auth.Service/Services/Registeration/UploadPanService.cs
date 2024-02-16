using Auth.Service.Models.Registeration.UploadPan;
using Auth.Service.Respositories.Registeration;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using UJBHelper.DataModel;

namespace Auth.Service.Services.Registeration
{
    public class UploadPanService : IUploadPanService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private readonly IMongoCollection<UserKYCDetails> _userKYCDetails;
        private IConfiguration _iconfiguration;

        public UploadPanService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _users = database.GetCollection<User>("Users");
            _userKYCDetails = database.GetCollection<UserKYCDetails>("UserKYCDetails");
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
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

        public void Update_Pan_Details(Post_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            if (request.panType == "Individual")
            {

                if (_userKYCDetails.Find(x => x.UserId == request.userId).CountDocuments() == 0)
                {
                    var kyc = new UserKYCDetails
                    {
                        UserId = request.userId,
                        PanCard = new PanCard
                        {
                            //PanBase64Img = request.panImgBase64,
                            //PanImgType = request.panImgType,
                            PanNumber = request.panNumber,
                            FileName = request.FileName,
                            ImageURL = request.ImageURL,
                            UniqueName = request.UniqueName,
                        },
                        AdharCard = new AdharCard(),
                        BankDetails = new BankDetails(),
                        Updated = new Updated(),
                        IsApproved = new IsApproved(),
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
                 .Set(x => x.PanCard, new PanCard
                 {
                     //PanBase64Img = request.panImgBase64,
                     //PanImgType = request.panImgType,
                     FileName = request.FileName,
                     ImageURL = request.ImageURL,
                     UniqueName = request.UniqueName,
                     PanNumber = request.panNumber,
                 })
                 .Set(x => x.Updated, new Updated
                 {
                     updated_By = request.userId,
                     updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                 })
                 );


                }
            }
            else if (request.panType == "Business")
            {
                if (_businessDetails.Find(x => x.UserId == request.userId).CountDocuments() == 0)
                {
                    var bus = new BusinessDetails
                    {
                        UserId = request.userId,
                        BusinessPan = new BusinessPan
                        {
                            //    PanBase64 = request.panImgBase64,
                            //    PanImageType = request.panImgType,
                            //    PanNumber = request.panNumber,

                            FileName = request.FileName,
                            ImageURL = request.ImageURL,
                            UniqueName = request.UniqueName,
                            PanNumber = request.panNumber,
                        },
                        Created = new Created
                        {
                            created_By = request.userId,
                            created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                        },
                        Updated = new Updated
                        {
                            updated_By = "",
                            updated_On = null
                        },
                        BusinessAddress = new BusinessAddress(),
                        Categories = new List<string>(),
                        Logo = new Logo(),
                        latitude = 0.0,
                        longitude = 0.0,
                        averageRating = 3.0
                    };

                    _businessDetails.InsertOne(bus);
                }

                else
                {
                    _businessDetails.FindOneAndUpdate(
                 Builders<BusinessDetails>.Filter.Eq(x => x.UserId, request.userId),
                 Builders<BusinessDetails>.Update
                 .Set(x => x.BusinessPan, new BusinessPan
                 {
                    // PanBase64 = request.panImgBase64,
                    // PanImageType = request.panImgType,
                     PanNumber = request.panNumber,
                     FileName = request.FileName,
                     ImageURL = request.ImageURL,
                     UniqueName = request.UniqueName,
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
}