using Auth.Service.Models.Registeration.User;
using Auth.Service.Respositories.Registeration;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using UJBHelper.DataModel;
using static UJBHelper.Common.Common;
using Get_Request = Auth.Service.Models.Registeration.User.Get_Request;

namespace Auth.Service.Services.Registeration
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IMongoCollection<UserKYCDetails> _userKycDetails;
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<AdminUser> _Adminusers;
        private readonly IMongoCollection<UserOtherDetails> _userOtherDetails;
        private readonly IMongoCollection<Leads> _leads;
        private readonly IMongoCollection<AgreementDetails> _agreementDetails;
        private IConfiguration _iconfiguration;

        public UserInfoService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _users = database.GetCollection<User>("Users");
            _userKycDetails = database.GetCollection<UserKYCDetails>("UserKYCDetails");
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
            _Adminusers = database.GetCollection<AdminUser>("AdminUsers");
            _userOtherDetails = database.GetCollection<UserOtherDetails>("UsersOtherDetails");
            _leads = database.GetCollection<Leads>("Leads");
            _agreementDetails = database.GetCollection<AgreementDetails>("AgreementDetails");
        }

        public bool Check_If_Admin_User_Exists(string userId)
        {
            return _Adminusers.Find(x => x._id == userId).CountDocuments() > 0;
        }

        public bool Check_If_User_Exists(string userId)
        {
            return _users.Find(x => x._id == userId).CountDocuments() > 0;
        }

        public bool Check_If_User_Other_Exists(string userId)
        {
            return _userOtherDetails.Find(x => x.UserId == userId).CountDocuments() > 0;
        }
        public bool Check_If_User_IsActive(string userId)
        {
            return _users.Find(x => x._id == userId && x.isActive == true).CountDocuments() > 0;
        }

        public void Insert_UserOther_Details(Put_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var p = new UserOtherDetails
            {
                UserId = request.UserId,
               // father_Husband_Name = request.father_Husband_Name,
                maritalStatus = request.maritalStatus,
                nationality = request.nationality,
                phoneNo = request.phoneNo,
                Hobbies = request.Hobbies,
                aboutMe = request.aboutMe,
                areaOfInterest = request.areaOfInterest,
                canImpartTraining = request.canImpartTraining,
                Created = new Created
                {
                    created_By = request.created_By,
                    created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                },
                Updated = new Updated
                {
                    updated_By = null,
                    updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                }
            };

            _userOtherDetails.InsertOne(p);
        }

        public void Update_UserOtherDetails(Put_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            _userOtherDetails.FindOneAndUpdate(
                Builders<UserOtherDetails>.Filter.Eq(x => x.UserId, request.UserId),
                Builders<UserOtherDetails>.Update
              // .Set(x => x.father_Husband_Name, request.father_Husband_Name)
                .Set(x => x.maritalStatus, request.maritalStatus)
                .Set(x => x.nationality, request.nationality)
                .Set(x => x.phoneNo, request.phoneNo)
                .Set(x => x.Hobbies, request.Hobbies)
                .Set(x => x.aboutMe, request.aboutMe)
                .Set(x => x.areaOfInterest, request.areaOfInterest)
                .Set(x => x.canImpartTraining, request.canImpartTraining)
                .Set(x => x.Updated, new Updated
                {
                    updated_By = request.created_By,
                    updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                })
                );
        }

        public Models.Admin.User.Get_Request Get_Admin_User_Details(string userId)
        {
            return _Adminusers.Find(x => x.Role == "Admin").Project(x => new Models.Admin.User.Get_Request
            {
                firstName = x.firstName,
                lastName = x.lastName,
                emailId = x.emailId,
                mobileNumber = x.mobileNumber,
                role = x.Role,
                userId = x._id
            }).FirstOrDefault();
        }

        public List<Models.Admin.User.Get_Request> Get_Admin_User_List(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                query = "";
            }
            else
            {
                query = query.ToLower();
            }

            return _Adminusers.Find(x =>
            x.firstName.ToLower().Contains(query)
            || x.lastName.ToLower().Contains(query)
            || x.emailId.ToLower().Contains(query))

            .Project(x => new Models.Admin.User.Get_Request
            {
                firstName = x.firstName,
                lastName = x.lastName,
                emailId = x.emailId,
                mobileNumber = x.mobileNumber,
                role = x.Role,
                userId = x._id,
                password = x.password,
                countryCode = x.countryCode,
                isActive = x.isActive
            }).ToList();
        }

        public Get_Request Get_User_Details(string userId)
        {
            var res = new Get_Request();
            BusinessDetails _bsns = new BusinessDetails();
            var role = _users.Find(x => x._id == userId).Project(x => x.Role).FirstOrDefault();
            res.noOfLeads = 0;
            bool kycAdded = true;
            string kycApproved = "";
            string AgreementURL = "";
            string ListedPAgreementURL = "";
            if (_agreementDetails.Find(x => x.UserId == userId && x.type == "Partner Agreement" && x.accepted.isAccepted == false).CountDocuments() > 0)
            {
                AgreementURL = _agreementDetails.Find(x => x.UserId == userId && x.type == "Partner Agreement" && x.accepted.isAccepted == false).FirstOrDefault().PdfURL;
            }
            else
            {
                AgreementURL = "";
            }

            if(role=="Listed Partner")
            { 

            _bsns = _businessDetails.Find(x => x.UserId == userId).FirstOrDefault();

            if (_agreementDetails.Find(x => x.UserId == userId && x.BusinessId == _bsns.Id && x.type == "Listed Partner Agreement" && x.accepted.isAccepted == false).CountDocuments() > 0)
            {
                ListedPAgreementURL = _agreementDetails.Find(x => x.UserId == userId && x.BusinessId == _bsns.Id && x.type == "Listed Partner Agreement" && x.accepted.isAccepted == false).FirstOrDefault().PdfURL;
            }
            else
            {
                ListedPAgreementURL = "";
            }
            }
            switch (role)
            {
                case "Partner":
                    

                    var kycApprovalStatus = _userKycDetails.Find(x => x.UserId == userId).Project(x => x.IsApproved).FirstOrDefault();
                    if (kycApprovalStatus == null)
                    {
                        kycAdded = false;
                        kycApproved = "Pending";
                    }
                    else
                    {
                        UserKYCDetails _kycdetails = new UserKYCDetails();
                        _kycdetails = _userKycDetails.Find(x => x.UserId == userId).FirstOrDefault();
                        if (_kycdetails != null)
                        {
                            if (_kycdetails.PanCard != null || _kycdetails.AdharCard != null)
                            {
                                if (string.IsNullOrEmpty(_kycdetails.PanCard.PanNumber) || string.IsNullOrEmpty(_kycdetails.PanCard.ImageURL)
                                    || string.IsNullOrEmpty(_kycdetails.AdharCard.AdharNumber)
                                    || string.IsNullOrEmpty(_kycdetails.AdharCard.FrontImageURL)
                                    || string.IsNullOrEmpty(_kycdetails.AdharCard.BackImageURL))
                                    //|| string.IsNullOrEmpty(_kycdetails.BankDetails.ImageURL)
                                    //|| string.IsNullOrEmpty(_kycdetails.BankDetails.AccountNumber) || string.IsNullOrEmpty(_kycdetails.BankDetails.AccountHolderName)
                                    //|| string.IsNullOrEmpty(_kycdetails.BankDetails.BankName) || string.IsNullOrEmpty(_kycdetails.BankDetails.IFSCCode))
                                {
                                    kycAdded = false;

                                }
                                else
                                {
                                    kycAdded = true;
                                }
                            }
                            else
                            {
                                kycAdded = false;
                            }
                        }
                        else
                        {
                            kycAdded = false;
                        }
                        ////var userPan = _userKycDetails.Find(x => x.UserId == userId).Project(x => x.PanCard.PanNumber).FirstOrDefault();
                        ////var Image
                        ////var userAadhar = _userKycDetails.Find(x => x.UserId == userId).Project(x => x.AdharCard.AdharNumber).FirstOrDefault();
                        //// var userBank = _userKycDetails.Find(x => x.UserId == userId).Project(x => x.BankDetails.AccountNumber).FirstOrDefault();
                        //if (string.IsNullOrWhiteSpace(userPan) || string.IsNullOrWhiteSpace(userAadhar) || string.IsNullOrWhiteSpace(userBank))
                        //{
                        //    kycAdded = false;
                        //}
                        if (kycApprovalStatus.Flag == true)
                        {
                            kycApproved = "Approved";
                        }
                        else if (kycApprovalStatus.Reason != null)
                        {
                            kycApproved = "Rejected";
                        }
                        else
                        {
                            kycApproved = "Pending";
                        }
                    }
                    //kycApproved = ;
                    break;
                case "Listed Partner":
                    
                    var businessApprovalStatus = _businessDetails.Find(x => x.UserId == userId).Project(x => x.isApproved.Flag).FirstOrDefault();
                    // var businessPan = _businessDetails.Find(x => x.UserId == userId).Project(x => x.BusinessPan.PanNumber).FirstOrDefault();
                    if (string.IsNullOrEmpty(_bsns.BusinessPan.PanNumber) || string.IsNullOrEmpty(_bsns.BusinessPan.ImageURL))
                    {
                        kycAdded = false;
                    }
                    if(businessApprovalStatus==1)
                    {
                        kycApproved = "Approved";
                    }
                    else if(businessApprovalStatus == 2)
                    {
                        kycApproved = "Rejected";
                    }
                    else
                    {
                        kycApproved = "Pending";
                    }
                    // kycApproved = businessApprovalStatus ? "Approved" : "Rejected";
                    var businessId = _businessDetails.Find(x => x.UserId == userId).Project(x => x.Id).FirstOrDefault();
                   
                    res.noOfLeads = int.Parse(_leads.Find(x => x.referralStatus == 1 && x.referredBusinessId == businessId).CountDocuments().ToString());
                    break;
            }

            var filter = Builders<User>.Filter.Eq(x => x._id, userId);

            res = _users.Find(filter).Project(x => new Get_Request
            {
                userId = x._id,
               // base64Img = x.base64Image,
                countryCode = x.countryCode,
                emailId = x.emailId,
                firstName = x.firstName,
                imgType = x.imageType,
                imgUrl = x.ImageURL,
                isMentor = !string.IsNullOrWhiteSpace(x.myMentorCode),
                languagePreference = x.language,
                lastName = x.lastName,
                mobileNumber = x.mobileNumber,
                password = x.password,
                socialMediaId = x.socialLogin.Social_Code,
                //currentStatus = currentStatus,
                ujbId = x.myMentorCode,
                role = x.Role,
                isKycAdded = kycAdded,
                kycApprovalStatus = kycApproved,
                PartnerAgreementURL = AgreementURL,
                ListedPartnerAgreementURL = ListedPAgreementURL,
                gender = x.gender,
                address = x.address,
                isActive = x.isActive,
                latitude = x.latitude,
                longitude = x.longitude,
                myMentorCode = x.myMentorCode,
                isMembershipAgreementAccepted = x.isMembershipAgreementAccepted,
                isPartnerAgreementAccepted = x.isPartnerAgreementAccepted,
                MentorCode = x.mentorCode,
              

            }).FirstOrDefault();

            res.MentorName =  _users.Find(x => x.myMentorCode == res.MentorCode).Project(x => x.firstName+ " " + x.lastName).FirstOrDefault();
            res.businessDetails = new bsnsDetails1();
            res.businessDetails.CompanyName = _bsns.CompanyName;
            res.businessDetails.BusinessEmail = _bsns.BusinessEmail;
            res.businessDetails.bsnsAdd = _bsns.BusinessAddress;
            res.businessDetails.useTypeId = _bsns.UserType;
            if(_bsns.UserType!=0)
            { 
            res.businessDetails.userType = Enum.GetName(typeof(UserType), _bsns.UserType).Replace("_", "/");
            }
            res.businessDetails.PartnerName = _bsns.NameOfPartner;
            return res;
        }

        Models.Admin.User.Get_Request IUserInfoService.Get_Admin_User_Details(string userId)
        {
            var filter = Builders<AdminUser>.Filter.Eq(x => x._id, userId);

            return _Adminusers.Find(filter).Project(x => new Models.Admin.User.Get_Request
            {
                userId = x._id,
                //base64Img = x.base64Image,
                countryCode = x.countryCode,
                emailId = x.emailId,
                firstName = x.firstName,
                isActive = x.isActive,
                //imgType = x.imageType,
                //imgUrl = x.imageUrl,
                //isMentor = !string.IsNullOrWhiteSpace(x.myMentorCode),
                //languagePreference = x.language,
                lastName = x.lastName,
                mobileNumber = x.mobileNumber,
                role = x.Role,
                password = x.password
                //password = x.password,
                //socialMediaId = x.socialLogin.Social_Code,
                //currentStatus = "Pending",  //kyccc
                //ujbId = x.myMentorCode
            }).FirstOrDefault();
        }
    }
}
