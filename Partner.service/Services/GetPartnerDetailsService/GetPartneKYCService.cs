using MongoDB.Driver;
using System;
using Microsoft.Extensions.Configuration;
using Partner.Service.Models.PartnerDetails.PartnerKYC;
using MongoDB.Bson;
using UJBHelper.DataModel;
using Partner.Service.Repositories.PartnerDetails;
using static UJBHelper.Common.Common;
using UJBHelper.Common;

namespace Partner.Service.Services.GetPartnerDetailsService
{
    public class GetPartneKYCService : IGetPartnerKYC
    {
        private readonly IMongoCollection<User> _users;

        private readonly IMongoCollection<UserKYCDetails> _userKycDetails;

        private readonly IMongoCollection<UserOtherDetails> _userOtherDetails;
        private readonly IMongoCollection<CountryInfo> _countryCode;
        private readonly IMongoCollection<StateInfo> _states;
        private IConfiguration _iconfiguration;
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private readonly IMongoCollection<DbProductService> _productService;
        public GetPartneKYCService(IConfiguration config)
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

        public Get_Request GetPartnerKYC(String UserId)
        {
            var res = new Get_Request();
            var UserRole = _users.Find(x => x._id == UserId).FirstOrDefault().Role.ToString();
         
            var filter1 = Builders<UserKYCDetails>.Filter.Eq(x => x.UserId, UserId);


            long cnt = _userKycDetails.Find(x => x.UserId == UserId).CountDocuments();
            if (cnt > 0)
            {
                res.userKycInfo = _userKycDetails.Find(filter1).FirstOrDefault();
               
                if (res.userKycInfo != null)
                {
                    if (res.userKycInfo.IsApproved.ReasonId != 0)
                    {
                        res.userKycInfo.IsApproved.Reason = Enum.GetName(typeof(RejectReasons), res.userKycInfo.IsApproved.ReasonId);
                    }
                }
                

                if (UserRole == "Partner")
                {


                    if (cnt == 0)
                    { res.isKYCComplete = false; }
                    else
                    {
                       
                      //  var _kycdetails = _userKycDetails.Find(x => x.UserId == UserId).FirstOrDefault();
                        if (res.userKycInfo != null)
                        {
                            if (res.userKycInfo.PanCard != null || res.userKycInfo.AdharCard != null || res.userKycInfo.BankDetails != null)
                            {
                                if (string.IsNullOrEmpty(res.userKycInfo.PanCard.PanNumber) || string.IsNullOrEmpty(res.userKycInfo.PanCard.ImageURL)
                                    || string.IsNullOrEmpty(res.userKycInfo.AdharCard.AdharNumber)
                                    || string.IsNullOrEmpty(res.userKycInfo.AdharCard.FrontImageURL)
                                    || string.IsNullOrEmpty(res.userKycInfo.AdharCard.BackImageURL)
                                    || string.IsNullOrEmpty(res.userKycInfo.BankDetails.ImageURL)
                                    || string.IsNullOrEmpty(res.userKycInfo.BankDetails.AccountNumber) || string.IsNullOrEmpty(res.userKycInfo.BankDetails.AccountHolderName)
                                    || string.IsNullOrEmpty(res.userKycInfo.BankDetails.BankName) || string.IsNullOrEmpty(res.userKycInfo.BankDetails.IFSCCode))
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
                 
                }
                else if (UserRole == "Listed Partner")
                {
                    res.isKYCComplete = true;
                  
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
            return res;
        }

        public bool Check_If_User_Exist(string UserId)
        {
            return _userKycDetails.Find(x => x.UserId == UserId).CountDocuments() > 0;
        }

        public bool Check_If_User_IsActive(string UserId)
        {
            return _users.Find(x => x._id == UserId & x.isActive == true).CountDocuments() > 0;
        }

    

    }
}
