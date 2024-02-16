using MongoDB.Driver;
using System;
using Microsoft.Extensions.Configuration;
using Partner.Service.Models.PartnerDetails;
using MongoDB.Bson;
using UJBHelper.DataModel;
using Partner.Service.Repositories.PartnerDetails;
using static UJBHelper.Common.Common;
using UJBHelper.Common;

namespace Partner.Service.Services.GetPartnerDetailsService
{
    public class GetPartnerDetailsService : IGetPartnerDetails
    {
        private readonly IMongoCollection<User> _users;

        private readonly IMongoCollection<UserKYCDetails> _userKycDetails;

        private readonly IMongoCollection<UserOtherDetails> _userOtherDetails;
        private readonly IMongoCollection<CountryInfo> _countryCode;
        private readonly IMongoCollection<StateInfo> _states;
        private IConfiguration _iconfiguration;
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private readonly IMongoCollection<DbProductService> _productService;
        public GetPartnerDetailsService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _users = database.GetCollection<User>("Users");
            _userOtherDetails = database.GetCollection<UserOtherDetails>("UsersOtherDetails");
            _userKycDetails = database.GetCollection<UserKYCDetails>("UserKYCDetails");
            _countryCode = database.GetCollection<CountryInfo>("CountryCode");
            _states = database.GetCollection<StateInfo>("States");
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
            _productService = database.GetCollection<DbProductService>("ProductsServices");
        }

        public Get_Request GetPartnerDetails(String UserId)
        {
            var res = new Get_Request();
            var UserRole = _users.Find(x => x._id == UserId).FirstOrDefault().Role.ToString();
            var filter = Builders<User>.Filter.Eq(x => x._id, UserId);
            var filter1 = Builders<UserKYCDetails>.Filter.Eq(x => x.UserId, UserId);
            var filter2 = Builders<UserOtherDetails>.Filter.Eq(x => x.UserId, UserId);

            res.userInfo = _users.Find(filter).FirstOrDefault();
            res.userKycInfo = _userKycDetails.Find(filter1).FirstOrDefault();
            if (res.userInfo != null)
            {
                res.userInfo.password = SecurePasswordHasherHelper.Decrypt(res.userInfo.password);// SecurePasswordHasherHelper.
            }
            if (res.userKycInfo != null)
            {
                if (res.userKycInfo.IsApproved.ReasonId != 0)
                {
                    res.userKycInfo.IsApproved.Reason = Enum.GetName(typeof(RejectReasons), res.userKycInfo.IsApproved.ReasonId);
                }
            }
            //if (res.userInfo.userType != 0)
            //{
            //    res.UserTypeValue = Enum.GetName(typeof(UserType), res.userInfo.userType);
            //}
            res.userOtherDetails = _userOtherDetails.Find(filter2).FirstOrDefault();
            res.countryName = _countryCode.Find(x => x.countryId == res.userInfo.countryId).Project(x => x.countryName).FirstOrDefault();
            res.stateName = _states.Find(x => x.stateId == res.userInfo.stateId).Project(x => x.stateName).FirstOrDefault();
            if (UserRole == "Partner")
            {
                //filter = filter & (Builders<User>.Filter.Eq(x => x.gender, BsonString.Empty) | Builders<User>.Filter.Eq(x => x.knowledgeSource, BsonString.Empty)
                //    | Builders<User>.Filter.Eq(x => x.organisationType, BsonString.Empty) | Builders<User>.Filter.Eq(x => x.mentorCode, BsonString.Empty)
                //    | Builders<User>.Filter.Eq(x => x.passiveIncome, BsonString.Empty)
                //    | Builders<User>.Filter.Eq(x => x.userType,0)
                ////    | Builders<User>.Filter.Eq(x => x.birthDate,null)
                //    | Builders<User>.Filter.Eq(x => x.birthDate,null));
                //filter = filter |
                //     (Builders<User>.Filter.Eq(x => x.preferredLocations.location1, BsonString.Empty)
                //    & Builders<User>.Filter.Eq(x => x.preferredLocations.location2, BsonString.Empty)
                //    & Builders<User>.Filter.Eq(x => x.preferredLocations.location3, BsonString.Empty));
                // long count = _users.Find(filter).CountDocuments();
              long cnt=  _userKycDetails.Find(x=>x.UserId==UserId).CountDocuments();
                if (cnt ==0)
                { res.isKYCComplete = false; }
                else{
                    //    filter1 = filter1 & (Builders<UserKYCDetails>.Filter.Eq(x => x.PanCard.PanNumber, BsonString.Empty)
                    //    | Builders<UserKYCDetails>.Filter.Eq(x => x.PanCard.ImageURL, BsonString.Empty)
                    //    | Builders<UserKYCDetails>.Filter.Eq(x => x.AdharCard.AdharNumber, BsonString.Empty)
                    //    | Builders<UserKYCDetails>.Filter.Eq(x => x.AdharCard.FrontImageURL, BsonString.Empty)
                    //    | Builders<UserKYCDetails>.Filter.Eq(x => x.AdharCard.BackImageURL, BsonString.Empty)
                    //    | Builders<UserKYCDetails>.Filter.Eq(x => x.BankDetails.AccountHolderName, BsonString.Empty)
                    //    | Builders<UserKYCDetails>.Filter.Eq(x => x.BankDetails.AccountNumber, BsonString.Empty)
                    //    | Builders<UserKYCDetails>.Filter.Eq(x => x.BankDetails.BankName, BsonString.Empty)
                    //    | Builders<UserKYCDetails>.Filter.Eq(x => x.BankDetails.IFSCCode, BsonString.Empty)
                    //    | Builders<UserKYCDetails>.Filter.Eq(x => x.BankDetails.ImageURL, BsonString.Empty));
                    //long count1 = _userKycDetails.Find(filter1).CountDocuments();

                    //    if (count1 == 0)
                    //    {
                    //        res.isKYCComplete = true;
                    //    }
                    //    else
                    //    {
                    //        res.isKYCComplete = false;
                    //    }
                    var _kycdetails = _userKycDetails.Find(x => x.UserId == UserId).FirstOrDefault();
                    if (_kycdetails != null)
                    {
                        if (_kycdetails.PanCard != null || _kycdetails.AdharCard != null || _kycdetails.BankDetails != null)
                        {
                            if (string.IsNullOrEmpty(_kycdetails.PanCard.PanNumber) || string.IsNullOrEmpty(_kycdetails.PanCard.ImageURL)
                                || string.IsNullOrEmpty(_kycdetails.AdharCard.AdharNumber)
                                || string.IsNullOrEmpty(_kycdetails.AdharCard.FrontImageURL)
                                || string.IsNullOrEmpty(_kycdetails.AdharCard.BackImageURL))
                                //|| string.IsNullOrEmpty(_kycdetails.BankDetails.ImageURL)
                                //|| string.IsNullOrEmpty(_kycdetails.BankDetails.AccountNumber) || string.IsNullOrEmpty(_kycdetails.BankDetails.AccountHolderName)
                                //|| string.IsNullOrEmpty(_kycdetails.BankDetails.BankName) || string.IsNullOrEmpty(_kycdetails.BankDetails.IFSCCode))
                            {
                                res.isKYCComplete = false;

                            }
                            else
                            {
                                res.isKYCComplete = true;
                            }
                        }
                        else
                        {
                            res.isKYCComplete = false;
                        }
                    }
                    else
                    {
                        res.isKYCComplete = false;
                    }

                }
                res.isRefer = false;
            }
            else if (UserRole == "Listed Partner")
            {
                res.isKYCComplete = true;
                if (_users.Find(x => x._id != UserId && x.isMembershipAgreementAccepted == true && x.isActive).CountDocuments() > 0)
                {
                    if (_businessDetails.Find(x => x.UserId != null && x.UserId != UserId && x.isApproved.Flag == 1 && x.isSubscriptionActive).CountDocuments() > 0)
                    {
                        var b = _businessDetails.Find(x => x.UserId != null && x.UserId != UserId && x.isApproved.Flag == 1 && x.isSubscriptionActive).Project(x => x.Id).FirstOrDefault();
                        if (_productService.Find(x => x.bussinessId == b).CountDocuments() > 0 && _users.Find(x => x._id == UserId && x.isActive && x.Role == "Listed Partner" && x.isMembershipAgreementAccepted == true).CountDocuments() > 0)
                        {
                            res.isRefer = true;
                        }
                        else
                        {
                            res.isRefer = false;
                        }
                    }
                    else
                    {
                        res.isRefer = false;
                    };
                }
                else
                {
                    res.isRefer = false;
                };

                //var b = _businessDetails.Find(x => x.UserId != null && x.UserId != UserId && x.isApproved.Flag == 1 && x.isSubscriptionActive).Project(x => x.Id);

                //     if (_productService.Find(x => x.bussinessId == b).CountDocuments() != 0 && _users.Find(x => x._id == b.userId && x.isActive && x.Role == "Listed Partner" && x.isMembershipAgreementAccepted == true).CountDocuments() != 0)
                //{
                //   // response.businessList.Add(b);
                //}
                // var prodBusinessIdList = _productService.Find(y => y.name.ToLower().Contains(request.searchTerm)).Project(x => x.bussinessId).ToList();

            }
            else
            {
                res.isKYCComplete = false;

            }
            //else if (res.UserRole == "Partner")
            //{
            //    filter = filter & (Builders<User>.Filter.Eq(x => x.myMentorCode, BsonString.Empty));
            //    long Count = _users.Find(filter).CountDocuments();

            //    if (Count == 0)
            //    {
            //        res.isKYCComplete = true;
            //    }
            //    else
            //    {
            //        res.isKYCComplete = false;
            //    }
            //}
            return res;
        }

        public bool Check_If_User_Exist(string UserId)
        {
            return _users.Find(x => x._id == UserId).CountDocuments() > 0;
        }

        public bool Check_If_User_IsActive(string UserId)
        {
            return _users.Find(x => x._id == UserId & x.isActive == true).CountDocuments() > 0;
        }

    

    }
}
