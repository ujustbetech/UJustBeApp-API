using Business.Service.Models.ClientPartner;
using Business.Service.Repositories.AddProductService;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using UJBHelper.DataModel;
using static UJBHelper.Common.Common;

namespace Business.Service.Services.ProductServices
{
    public class ClientPartnerService : IClientPartnerService
    {
        private readonly IMongoCollection<DbProductService> _products;
        private readonly IMongoCollection<FeePaymentDetails> _Feepayment;
        private readonly IMongoCollection<FeeStructure> _Feestructure;
        private readonly IMongoCollection<ProductServiceDetails> _productsDetails;
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Categories> _categories;
        private IConfiguration _iconfiguration;
  
        public ClientPartnerService(IConfiguration config)
        {
            //var client = new MongoClient(DbHelper.GetConnectionString());
            // var database = client.GetDatabase(DbHelper.GetDatabaseName());
            _iconfiguration = config;
            var client = new MongoClient(_iconfiguration["ConnectionString"]);

            var database = client.GetDatabase(_iconfiguration["Database"]);
            _Feepayment = database.GetCollection<FeePaymentDetails>("FeePaymentDetails");
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
            _products = database.GetCollection<DbProductService>("ProductsServices");
            _productsDetails = database.GetCollection<ProductServiceDetails>("ProductsServicesDetails");
            _users = database.GetCollection<User>("Users");
            _categories = database.GetCollection<Categories>("Categories");
            _Feestructure = database.GetCollection<FeeStructure>("FeeStructure");
           
        }

        public bool Check_If_Client_Partner_Exists(string userId)
        {
            return _users.Find(x => x._id == userId && x.Role == "Listed Partner").CountDocuments() > 0;
        }

        public bool Check_If_User_Exists(string userId)
        {
            return _users.Find(x => x._id == userId).CountDocuments() > 0;
        }

        public bool Check_If_Amt_Paid(string UserId, int CountryId)
        {
            double PaidAmount;
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });
            DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));

            var filter = Builders<FeeStructure>.Filter.Gte(x => x.ToDate, CurrentDate);
            filter = filter & Builders<FeeStructure>.Filter.Lte(x => x.FromDate, CurrentDate);
            filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.FeeTypeId, "5d5a4534339dce0154441aac");
            filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.CountryId, CountryId);
            if (_Feestructure.Find(filter).CountDocuments() > 0)
            {
                List<FeePaymentDetails> feePay = new List<FeePaymentDetails>();
                feePay = _Feepayment.Find(x => x.userId == UserId && x.feeType == "5d5a4534339dce0154441aac" && x.ConvertedPaymentDate <= CurrentDate).ToList();
                if (feePay.Count() > 0)
                {
                    PaidAmount = feePay.Sum(x => x.amount);
                }
                else
                {
                    PaidAmount = 0;
                }
                if (_Feestructure.Find(filter).FirstOrDefault().Amount > PaidAmount)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public Get_Request Get_Client_Partner_Details(string userId)
        {
            var res = new Get_Request();
            //res = _users.Find(x => x._id == userId && x.Role == "Client Partner")
            var cat = new List<string>();
            res = _businessDetails.Find(x => x.UserId == userId).Project(x => new Get_Request
            {
                userId = userId,
                address = x.BusinessAddress,
                businessDescription = x.BusinessDescription,
                businessId = x.Id,
                businessName = x.CompanyName,
                businessUrl = x.WebsiteUrl,
                CategoriesId = x.Categories,
                rating = x.averageRating,
                tagline = x.Tagline,
                businessEmail = x.BusinessEmail,
                businessPan = x.BusinessPan,
                businessLogo = x.Logo,
                businessGST = x.GSTNumber,
                BannerDetails = x.BannerDetails,
                IsApproved = x.isApproved,
                userTypeId = x.UserType,
                NameofPartner = x.NameOfPartner,
                isSubscriptionActive = x.isSubscriptionActive
                //ujbId
            }).FirstOrDefault();

            if (res.userTypeId != 0)
            {
                res.userType = Enum.GetName(typeof(UserType), res.userTypeId).Replace("_", "/");
            }


            var resUser = new Request_User();

            resUser = _users.Find(x => x._id == userId).Project(x => new Request_User
            {
                firstName = x.firstName,
                lastName = x.lastName,
                myMentorcode = x.myMentorCode,
                countryId = x.countryId
                //ujbId
            }).FirstOrDefault();
            //   var catids = _businessDetails.Find(x => x.UserId == userId).Project(x => x.Categories).FirstOrDefault();
            var catids = res.CategoriesId;
            if (catids != null && catids.Count != 0)
            {
                res.categories = _categories.Find(x => catids.Contains(x.Id)).Project(x => new Category_Info
                {
                    id = x.Id,
                    name = x.categoryName
                }).ToList();
            }
            else
            {
                res.categories = new List<Category_Info>();
            }


            //if (string.IsNullOrWhiteSpace(res.businessName))
            //{
            //    res.businessName = _users.Find(x => x._id == res.userId).Project(x => x.firstName + " " + x.lastName).FirstOrDefault();
            //}
            if (string.IsNullOrWhiteSpace(res.businessName))
            {
                res.businessName = resUser.firstName + " " + resUser.lastName;
            }
            // res.businessName = resUser.firstName + " " + resUser.lastName;//
            //    res.myMentorCode = _users.Find(x => x._id == res.userId).Project(x => x.myMentorCode).FirstOrDefault();
            //    int countryId = _users.Find(x => x._id == res.userId).Project(x => x.countryId).FirstOrDefault();

            res.myMentorCode = resUser.myMentorcode;
            int countryId = resUser.countryId;
            var isSubscriptionPaid = Check_If_Amt_Paid(res.userId, countryId);
            if (isSubscriptionPaid == false)
            {
                res.isFeePending = true;
            }
            else
            {
                res.isFeePending = false;
            }
            res.isRefer = Check_Is_Referred(res.userId);

            return res;
        }

      public bool  Check_Is_Referred(string UserId)
            {
            if (_users.Find(x => x._id != UserId && x.isMembershipAgreementAccepted == true && x.isActive).CountDocuments() > 0)
            {
                if (_businessDetails.Find(x => x.UserId != null && x.UserId != UserId && x.isApproved.Flag == 1 && x.isSubscriptionActive).CountDocuments() > 0)
                {
                    var b = _businessDetails.Find(x => x.UserId != null && x.UserId != UserId && x.isApproved.Flag == 1 && x.isSubscriptionActive).Project(x => x.Id).FirstOrDefault();
                    if (_products.Find(x => x.bussinessId == b).CountDocuments() > 0 && _users.Find(x => x._id == UserId && x.isActive && x.Role == "Listed Partner" && x.isMembershipAgreementAccepted == true).CountDocuments() > 0)
                    {
                        return  true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                };
            }
            else
            {
                return false;
            }
        }
    }
}
