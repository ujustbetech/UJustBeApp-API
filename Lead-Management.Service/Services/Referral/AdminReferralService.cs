using Lead_Management.Service.Models.AdminReferral;
using Lead_Management.Service.Repositories.Referral;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;
using UJBHelper.DataModel;
using System;

namespace Lead_Management.Service.Services.Referral
{
    public class AdminReferralService : IAdminReferralService
    {
        private readonly IMongoCollection<Categories> _categories;
        private readonly IMongoCollection<Leads> _lead;
        private readonly IMongoCollection<DbProductService> _productsAndService;
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<AdminUser> _adminUsers;
        private readonly IMongoCollection<DealStatus> _dealStatus;
        private IConfiguration _iconfiguration;
        private readonly IMongoCollection<ReferralAgreedPercentage> _ReferralAgreedPercentage;

        public AdminReferralService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _categories = database.GetCollection<Categories>("Categories");
            _lead = database.GetCollection<Leads>("Leads");
            _productsAndService = database.GetCollection<DbProductService>("ProductsServices");
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
            _users = database.GetCollection<User>("Users");
            _adminUsers = database.GetCollection<AdminUser>("AdminUsers");
            _dealStatus = database.GetCollection<DealStatus>("DealStatus");
            _ReferralAgreedPercentage = database.GetCollection<ReferralAgreedPercentage>("ReferralAgreedPercentage");
        }

        public bool Check_If_User_Exists(string userId)
        {
            return _users.Find(x => x._id == userId).CountDocuments() > 0;
        }

        public Get_Request Get_Referral_Details(string userId)
        {
            var res = new Get_Request();

            //var businessId = _businessDetails.Find(x => x.UserId == userId).Project(x => x.Id).FirstOrDefault();

            //var categoriIds = _businessDetails.Find(x => x.Id == businessId).FirstOrDefault().Categories;

            //var categoryNames = new List<string>();
            //if (categoriIds != null)
            //{
            //    categoryNames = _categories.Find(x => categoriIds.Contains(x.Id)).Project(x => x.categoryName).ToList();
            //}
            res.ReferredByMeList = _lead.Find(x => x.referredBy.userId == userId).Project(x => new Request_Info
            {
                referralId = x.Id,
                //categories = categoryNames,
                dateCreated = x.referralDate.Value.ToString("dd/MM/yyyy"),
                isForSelf = x.isForSelf,
                productId = x.referredProductORServicesId,
                productName = x.referredProductORServices,
                referralDescription = x.referralDescription,
                dealValue = x.dealValue.ToString(),
               // isAccepted = x.isAccepted,
                rejectionReason = x.rejectionReason,
                businessId = x.referredBusinessId,
                referredToDetails = x.referredTo,
                refStatus = (int)x.referralStatus,
               dealStatus=(int)x.dealStatus,
                referralStatusUpdatedOn=x.dealStatusUpdatedOn,
                referralStatusUpdatedby=x.referralStatusUpdatedby,
                ReferralCode = x.ReferralCode
            }).ToList();

            foreach (var refs in res.ReferredByMeList)
            {
                var referredById = _lead.Find(x => x.Id == refs.referralId).Project(x => x.referredBy.userId).FirstOrDefault();
                var user = _users.Find(x => x._id == referredById).FirstOrDefault();
                refs.referredByDetails = new ReferredByDetails
                {
                    referredByName = user.firstName + " " + user.lastName,
                    referredByMobileNo = user.mobileNumber,
                    referredByEmailId = user.emailId,
                    referredByCountryCode = user.countryCode
                };
                refs.referralStatusValue = Enum.GetName(typeof(ReferralStatusEnum), refs.refStatus);
                if (refs.dealStatus != null)
                {
                    var dealstatusvalue = _dealStatus.Find(x => x.StatusId == refs.dealStatus).Project(x => x.StatusName).FirstOrDefault();

                    refs.dealStatusValue = dealstatusvalue;
                }

                var user_id = _businessDetails.Find(x => x.Id == refs.businessId).Project(x => x.UserId).FirstOrDefault();
                var mobileNumber = _users.Find(y => y._id == user_id).Project(y => y.mobileNumber).FirstOrDefault();
               // var user_id = _businessDetails.Find(x => x.Id == r.businessId).Project(x => x.UserId).FirstOrDefault();
               // var mobileNumber = _users.Find(y => y._id == user_id).Project(y => y.mobileNumber).FirstOrDefault();
                //var countryCode = _users.Find(y => y._id == user_id).Project(y => y.countryCode).FirstOrDefault();
                var UserName = _users.Find(y => y._id == user_id).Project(y => y.firstName + " " + y.lastName).FirstOrDefault();
                refs.clientPartnerDetails = _businessDetails.Find(x => x.Id == refs.businessId).Project(x => new ClientPartnerDetails
                {
                    name = x.CompanyName,
                    tagline = x.Tagline,
                    emailId = x.BusinessEmail,
                    mobileNumber = mobileNumber
                }).FirstOrDefault();
                if (refs.clientPartnerDetails.name == "")
                {
                    refs.clientPartnerDetails.name = UserName;
                }
            }

            var businessId = _businessDetails.Find(x => x.UserId == userId).Project(x => x.Id).FirstOrDefault();
            if (businessId != null)
            {
                var categoriIds = _businessDetails.Find(x => x.Id == businessId).FirstOrDefault().Categories;

                var categoryNames = new List<string>();
                if (categoriIds != null)
                {
                    categoryNames = _categories.Find(x => categoriIds.Contains(x.Id)).Project(x => x.categoryName).ToList();
                }
                res.ReferredBusinessList = _lead.Find(x => x.referredBusinessId == businessId).Project(x => new Request_Info
                {
                    referralId = x.Id,
                    categories = categoryNames,
                    dateCreated = x.referralDate.Value.ToString("dd/MM/yyyy"),
                    isForSelf = x.isForSelf,
                    productId = x.referredProductORServicesId,
                    productName = x.referredProductORServices,
                    referralDescription = x.referralDescription,
                    businessId = x.referredBusinessId,
                    referredToDetails = x.referredTo,
                    refStatus = (int)x.referralStatus,
                    dealValue = x.dealValue.ToString(),
                    // isAccepted = x.isAccepted,
                    rejectionReason = x.rejectionReason,
                    dealStatus = (int)x.dealStatus,
                    referralStatusUpdatedOn = x.refStatusUpdatedOn,
                    referralStatusUpdatedby = x.referralStatusUpdatedby,
                    ReferralCode = x.ReferralCode
                }).ToList();

                foreach (var refs in res.ReferredBusinessList)
                {
                    var referredById = _lead.Find(x => x.Id == refs.referralId).Project(x => x.referredBy.userId).FirstOrDefault();
                    var user = _users.Find(x => x._id == referredById).FirstOrDefault();
                    refs.referredByDetails = new ReferredByDetails
                    {
                        referredByName = user.firstName + " " + user.lastName,
                        referredByMobileNo = user.mobileNumber,
                        referredByEmailId = user.emailId,
                        referredByCountryCode = user.countryCode
                    };
                    refs.referralStatusValue = Enum.GetName(typeof(ReferralStatusEnum), refs.refStatus);
                    if (refs.dealStatus != null)
                    {
                        var dealstatusvalue = _dealStatus.Find(x => x.StatusId == refs.dealStatus).Project(x => x.StatusName).FirstOrDefault();

                        refs.dealStatusValue = dealstatusvalue;
                    }
                    var user_id = _businessDetails.Find(x => x.Id == refs.businessId).Project(x => x.UserId).FirstOrDefault();
                    var mobileNumber = _users.Find(y => y._id == user_id).Project(y => y.mobileNumber).FirstOrDefault();
                    var UserName = _users.Find(y => y._id == user_id).Project(y => y.firstName + " " + y.lastName).FirstOrDefault();
                    refs.clientPartnerDetails = _businessDetails.Find(x => x.Id == refs.businessId).Project(x => new ClientPartnerDetails
                    {
                        name = x.CompanyName,
                        tagline = x.Tagline,
                        emailId = x.BusinessEmail,
                        mobileNumber = mobileNumber
                    }).FirstOrDefault();
                    if (refs.clientPartnerDetails.name == "")
                    {
                        refs.clientPartnerDetails.name = UserName;
                    }
                }
            }
           

              //  DateTime referraldate = _leadDetails.Find(x => x.Id == leadId).FirstOrDefault().referralDate ?? DateTime.Now;


            
            
            return res;

        }
    }
}
