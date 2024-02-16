using MongoDB.Driver;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Partner.Service.Repositories.GetPartnerService;
using Partner.Service.Models.Partners;
using UJBHelper.DataModel;
using Partner.Service.Models.Partners.GetConnectors;
using MongoDB.Bson;
using Partner.Service.Models.Partners.GetAllDetails;
using System.Collections.Generic;
using static UJBHelper.Common.Common;

namespace Partner.Service.Services.GetPartnerService
{
    public class GetPartnerService : IGetPartnerService
    {
        private readonly IMongoCollection<User> _partners;
        private readonly IMongoCollection<UserOtherDetails> _userOtherDetails;
        private readonly IMongoCollection<CountryInfo> _countryCode;
        private readonly IMongoCollection<StateInfo> _stateCode;
        private readonly IMongoCollection<BusinessDetails> _bsns;
        private readonly IMongoCollection<UserKYCDetails> _partnerKYC;
        private readonly IMongoCollection<Categories> _category;
        private readonly IMongoCollection<AgreementDetails> _agreementDetails;
        private readonly IMongoCollection<DbProductService> _products;
        private readonly IMongoCollection<ProductServiceDetails> _productsDetails;
        private readonly IMongoCollection<SubscriptionDetails> _FeeSubscription;
        private readonly IMongoCollection<FeePaymentDetails> _Feepayment;
        private readonly IMongoCollection<PaymentDetails> _payment;
        private readonly IMongoCollection<FeeStructure> _FeeStructure;
        private IConfiguration _iconfiguration;
        public GetPartnerService(IConfiguration config)
        {
            _iconfiguration = config;
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _partners = database.GetCollection<User>("Users");
            _category = database.GetCollection<Categories>("Categories");
            _partnerKYC = database.GetCollection<UserKYCDetails>("UserKYCDetails");
            _countryCode = database.GetCollection<CountryInfo>("CountryCode");
            _stateCode = database.GetCollection<StateInfo>("States");
            _userOtherDetails = database.GetCollection<UserOtherDetails>("UsersOtherDetails");
            _bsns = database.GetCollection<BusinessDetails>("BussinessDetails");
            _agreementDetails = database.GetCollection<AgreementDetails>("AgreementDetails");
            _products = database.GetCollection<DbProductService>("ProductsServices");
            _productsDetails = database.GetCollection<ProductServiceDetails>("ProductsServicesDetails");
            _FeeSubscription = database.GetCollection<SubscriptionDetails>("SubscriptionDetails");
            _payment = database.GetCollection<PaymentDetails>("PaymentDetails");
            _FeeStructure = database.GetCollection<FeeStructure>("FeeStructure");
            _Feepayment = database.GetCollection<FeePaymentDetails>("FeePaymentDetails");

        }

        public bool Check_If_User_Exist(string UserId)
        {
            return _partners.Find(x => x._id == UserId).CountDocuments() > 0;
        }

        public bool Check_If_User_IsActive(string UserId)
        {
            return _partners.Find(x => x._id == UserId & x.isActive == true).CountDocuments() > 0;
        }

        public Get_Details_Excel GetUserAllDetails()
        {
            var res = new Get_Details_Excel();
            var filter = Builders<User>.Filter.Empty;
            res.UserExcelInfo = _partners.Find(filter).Project(x => new UserExcel()
            {
                id = x._id,
                firstName = x.firstName,
                lastName = x.lastName,
                middleName = x.middleName,
                emailId = x.emailId,
                mobileNumber = x.mobileNumber,
                countryId = x.countryId,
                alternateMobileNumber = x.alternateMobileNumber,
                alternateCountryCode = x.alternateCountryCode,
                stateId = x.stateId,
                birthDate = x.birthDate,
                gender = x.gender,
                //language = x.language,
                //preferredLocations = x.preferredLocations.location1 + "," +x.preferredLocations.location2 +"," + x.preferredLocations.location3,
                //knowledgeSource = x.knowledgeSource,
                //organisationType = x.organisationType,
                //userType = x.userType,
                passiveIncome = x.passiveIncome,
                address = new Address
                {
                    locality = x.address.locality,
                    location = x.address.location,
                    flatWing = x.address.flatWing
                },
                mentorCode = x.mentorCode,
                myMentorCode = x.myMentorCode,
                Role = x.Role,
                CreatedOn = x.Created.created_On,
                isActiveComment = x.isActiveComment,
                isActive = x.isActive,

            }).ToList();

            foreach (var item in res.UserExcelInfo)
            {
                if (item.Role != "Guest")
                {
                    //         item.userTypeValue= Enum.GetName(typeof(UserType), item.userType).Replace("_","/");
                    if (item.countryId != 0)
                    {
                        item.countryName = _countryCode.Find(x => x.countryId == item.countryId).FirstOrDefault().countryName;
                    }
                    if (item.stateId != 0)
                    {
                        item.StateName = _stateCode.Find(x => x.stateId == item.stateId).FirstOrDefault().stateName;
                    }
                    if (item.mentorCode != null && item.mentorCode != "")
                    {
                        item.MentorName = _partners.Find(x => x.myMentorCode == item.mentorCode).FirstOrDefault().firstName + " " + _partners.Find(x => x.myMentorCode == item.mentorCode).FirstOrDefault().lastName;
                    }
                    if (item.isActive == true)
                    {
                        item.Active = "Active";
                    }
                    else
                    {
                        item.Active = "InActive";
                    }
                    //Get UserKYCDetails
                    UserKYCExcel _kycdetails = new UserKYCExcel();
                    var filter1 = Builders<UserKYCDetails>.Filter.Eq(x => x.UserId, item.id);

                    if (_partnerKYC.Find(filter1).CountDocuments() > 0)
                    {
                        UserKYCDetails _kycdetail = new UserKYCDetails();
                        _kycdetail = _partnerKYC.Find(x => x.UserId == item.id).FirstOrDefault();
                        _kycdetails = _partnerKYC.Find(filter1).Project(x => new UserKYCExcel()
                        {

                            KYCIsApprovedFlag = x.IsApproved.Flag,
                            KYCReason = x.IsApproved.Reason,
                            KYCReasonId = x.IsApproved.ReasonId,
                            KYVApproveOn = x.IsApproved.ApprovedOn,
                        }).FirstOrDefault();

                        item.KYVApproveOn = _kycdetails.KYVApproveOn;
                        if (_kycdetails.KYCIsApprovedFlag == true)
                        {
                            item.isApporved = "Approved";
                        }
                        else if (_kycdetails.KYCReasonId != 0 && _kycdetails.KYCIsApprovedFlag == false)
                        {
                            item.isApporved = "Rejected";
                        }
                        else
                        {
                            item.isApporved = "Pending";
                        }

                        if (string.IsNullOrEmpty(_kycdetail.PanCard.PanNumber) || string.IsNullOrEmpty(_kycdetail.PanCard.ImageURL)
                                  || string.IsNullOrEmpty(_kycdetail.AdharCard.AdharNumber)
                                  || string.IsNullOrEmpty(_kycdetail.AdharCard.FrontImageURL)
                                  || string.IsNullOrEmpty(_kycdetail.AdharCard.BackImageURL))
                        {
                            item.isApporved = "KYC not yet submitted";
                            item.isPartnerAgreementAccepted = "KYC not yet submmited";
                        }
                    }
                    else
                    {
                        item.isApporved = "KYC not yet submitted";
                        item.isPartnerAgreementAccepted = "KYC not yet submmited";
                    }
                    UserOtherDetailExcel _otherdetails = new UserOtherDetailExcel();
                    var filter2 = Builders<UserOtherDetails>.Filter.Eq(x => x.UserId, item.id);
                    if (_userOtherDetails.Find(filter2).CountDocuments() > 0)
                    {
                        _otherdetails = _userOtherDetails.Find(filter2).Project(x => new UserOtherDetailExcel()
                        {
                            //    maritalStatus = x.maritalStatus,
                            //   nationality = x.nationality,
                            Hobbies = x.Hobbies,
                            //    aboutMe = x.aboutMe,
                            //     areaOfInterest = x.areaOfInterest,
                            canImpartTraining = x.canImpartTraining
                        }).FirstOrDefault();

                        item.Hobbies = _otherdetails.Hobbies;
                        item.can_impart_Training = _otherdetails.canImpartTraining == true ? "yes" : "not decided";
                        //item._BussinessDetails = Get_UserBsnsDetails_Excel(item.id);
                    }

                    UserAgreementDetailExcel _UserAgreementDetailExcel = new UserAgreementDetailExcel();
                    var filter3 = Builders<AgreementDetails>.Filter.Eq(x => x.UserId, item.id)
                          & Builders<AgreementDetails>.Filter.Eq(x => x.BusinessId, null);
                    if (_agreementDetails.Find(filter3).CountDocuments() > 0)
                    {
                        _UserAgreementDetailExcel = _agreementDetails.Find(filter3).Project(x => new UserAgreementDetailExcel()
                        {
                            Accepted = x.accepted.isAccepted,
                            AccetedOn = x.accepted.accepted_On

                        }).SortByDescending(x => x.Version).FirstOrDefault();
                        //item._BussinessDetails = Get_UserBsnsDetails_Excel(item.id);

                        if (_UserAgreementDetailExcel.Accepted == true)
                        {
                            item.isPartnerAgreementAccepted = "Accepted";
                        }
                        else
                        {
                            item.isPartnerAgreementAccepted = "Rejected";
                        }
                        item.PartnerAgreementAcceptedDate = _UserAgreementDetailExcel.AccetedOn;
                    }
                    else if (item.isApporved == "Approved")
                    {

                        item.isPartnerAgreementAccepted = "Pending";
                    }
                    else if (item.isApporved == "Pending")
                    {

                        item.isPartnerAgreementAccepted = "KYC yet to be approved";
                    }
                    else if (item.isApporved == "Rejected")
                    {

                        item.isPartnerAgreementAccepted = "KYC Rejected";
                    }
                    else
                    {
                        item.isPartnerAgreementAccepted = "KYC not yet submitted";
                    }
                    if (item.Role == "Listed Partner")
                    {
                        try
                        {
                            UserBusinessDetailsExcel _bsnsdetails = new UserBusinessDetailsExcel();
                            var filter4 = Builders<BusinessDetails>.Filter.Eq(x => x.UserId, item.id);
                            if (_bsns.Find(filter4).CountDocuments() > 0)
                            {
                                _bsnsdetails = _bsns.Find(filter4).Project(x => new UserBusinessDetailsExcel()
                                {
                                    BussineesId = x.Id,
                                    Categories = x.Categories,
                                    Tagline = x.Tagline,
                                    CompanyName = x.CompanyName,
                                    BusinessEmail = x.BusinessEmail,
                                    WebSiteURL = x.WebsiteUrl,
                                    Locality = x.BusinessAddress.Locality,
                                    Flat_Wing = x.BusinessAddress.Flat_Wing,
                                    Location = x.BusinessAddress.Location,
                                    GSTNumber = x.GSTNumber,
                                    PanNo = x.BusinessPan.PanNumber,
                                    AverageRating = x.averageRating,
                                    BsnsIsApprovedFlag = x.isApproved.Flag,
                                    BsnsIsAppovedReason = x.isApproved.Reason,
                                    BsnsApproveOn = x.isApproved.ApprovedOn,
                                    isSubscriptionActive = x.isSubscriptionActive,
                                    businessDescription = x.BusinessDescription,
                                    userTypeId = x.UserType,
                                    BusinessRegisterationDate = x.Created.created_On,
                                    ImageURL = x.BusinessPan.ImageURL,
                                    FileName = x.BusinessPan.FileName,
                                    UniqueName = x.BusinessPan.UniqueName
                                }).FirstOrDefault();

                                item.businessDescription = _bsnsdetails.businessDescription;
                                item.AverageRating = _bsnsdetails.AverageRating;
                                item.Locality = _bsnsdetails.Locality;
                                item.Flat_Wing = _bsnsdetails.Flat_Wing;
                                item.Location = _bsnsdetails.Location;
                                item.CompanyName = _bsnsdetails.CompanyName;
                                item.BusinessEmail = _bsnsdetails.BusinessEmail;
                                item.BsnsApproveOn = _bsnsdetails.BsnsApproveOn;
                                item.BusinessRegisterationDate = _bsnsdetails.BusinessRegisterationDate;

                                if (_bsnsdetails.userTypeId != 0)
                                {
                                    item.userType = Enum.GetName(typeof(UserType), _bsnsdetails.userTypeId).Replace("_", "/");
                                }

                                if (_bsnsdetails.isSubscriptionActive == false)
                                {
                                    item.SusbscriptionActive = "Inactive";
                                }
                                else
                                {
                                    item.SusbscriptionActive = "Active";
                                }

                                if (string.IsNullOrEmpty(_bsnsdetails.PanNo) || string.IsNullOrEmpty(_bsnsdetails.ImageURL))
                                {
                                    item.BsnsIsApproved = "KYC not yet submitted";
                                    item.isMembershipAgreementAccepted = "KYC not yet submmited";
                                }
                                else if (_bsnsdetails.BsnsIsApprovedFlag == 0)
                                {
                                    item.BsnsIsApproved = "Pending";
                                }
                                else if (_bsnsdetails.BsnsIsApprovedFlag == 1)
                                {
                                    item.BsnsIsApproved = "Approved";
                                }
                                else if (_bsnsdetails.BsnsIsApprovedFlag == 2)
                                {
                                    item.BsnsIsApproved = "Rejected";
                                }

                                for (int i = 0; i < _bsnsdetails.Categories.Count; i++)
                                {
                                    if (i == 0)
                                    {
                                        item.Categories1 = (_category.Find(x => x.Id == _bsnsdetails.Categories[i]).FirstOrDefault().categoryName);
                                    }
                                    else
                                    {
                                        item.Categories2 = (_category.Find(x => x.Id == _bsnsdetails.Categories[i]).FirstOrDefault().categoryName);
                                    }
                                }

                                if (_products.Find(x => x.bussinessId == _bsnsdetails.BussineesId && x.isActive).ToList().Count() > 0)
                                {
                                    int prod = _products.Find(x => x.bussinessId == _bsnsdetails.BussineesId && x.isActive && x.type == "Product").ToList().Count();
                                    int serv = _products.Find(x => x.bussinessId == _bsnsdetails.BussineesId && x.isActive && x.type == "Service").ToList().Count();
                                    if (prod > 0)
                                    {
                                        item.prodserve = "Product";
                                    }
                                    if (serv > 0)
                                    {
                                        item.prodserve = "Service";
                                    }
                                    if (prod > 0 && serv > 0)
                                    {
                                        item.prodserve = "Both";
                                    }
                                }
                                else
                                {
                                    item.prodserve = "None";
                                }

                                UserAgreementDetailExcel _userAgreementDetailExcel = new UserAgreementDetailExcel();
                                var filter5 = Builders<AgreementDetails>.Filter.Eq(x => x.UserId, item.id)
                                    & Builders<AgreementDetails>.Filter.Eq(x => x.BusinessId, _bsnsdetails.BussineesId);

                                if (_agreementDetails.Find(filter5).CountDocuments() > 0)
                                {
                                    _UserAgreementDetailExcel = _agreementDetails.Find(filter5).Project(x => new UserAgreementDetailExcel()
                                    {
                                        Accepted = x.accepted.isAccepted,
                                        AccetedOn = x.accepted.accepted_On

                                    }).SortByDescending(x => x.Version).FirstOrDefault();
                                    //item._BussinessDetails = Get_UserBsnsDetails_Excel(item.id);
                                    if (_UserAgreementDetailExcel != null)
                                    {
                                        if (_UserAgreementDetailExcel.Accepted == true)
                                        {
                                            item.isMembershipAgreementAccepted = "Accepted";
                                        }
                                        else
                                        {
                                            item.isMembershipAgreementAccepted = "Rejected";
                                        }
                                    }
                                    else
                                    {
                                        item.isMembershipAgreementAccepted = "Pending";
                                    }
                                    item.MembershipAgreementAcceptedDate = _UserAgreementDetailExcel.AccetedOn;
                                }
                                else
                                {
                                    if (item.BsnsIsApproved == "Approved")
                                    {
                                        item.isMembershipAgreementAccepted = "Pending";
                                    }
                                    else if (item.BsnsIsApproved == "Pending")
                                    {
                                        item.isMembershipAgreementAccepted = "KYC yet to be approved";
                                    }
                                    else if (item.BsnsIsApproved == "Rejected")
                                    {
                                        item.isMembershipAgreementAccepted = "KYC Rejected";
                                    }
                                    else
                                    {
                                        item.isMembershipAgreementAccepted = "KYC not yet submmited";
                                    }
                                }

                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }

            res._productService_Excel = new List<ProductService_Excel>();
            List<ProductServiceExcel> _productServiceExcel = new List<ProductServiceExcel>();
            _productServiceExcel = Get_Products_Service();
            ProductService_Excel Pd_Excel = new ProductService_Excel();
            List<ProductService_Excel> _productService_excel = new List<ProductService_Excel>();
            foreach (ProductServiceExcel _productService in _productServiceExcel)
            {
                Pd_Excel = new ProductService_Excel();
                Pd_Excel.firstName = _productService.firstName;
                Pd_Excel.UJBCode = _productService.UJBCode;
                Pd_Excel.lastName = _productService.lastName;
                if (_productService.lstProductServiceExcel != null)
                {
                    foreach (ProductService prodserv in _productService.lstProductServiceExcel)
                    {
                        Pd_Excel = new ProductService_Excel();
                        Pd_Excel.firstName = _productService.firstName;
                        Pd_Excel.UJBCode = _productService.UJBCode;
                        Pd_Excel.lastName = _productService.lastName;
                        Pd_Excel.name = prodserv.name;
                        Pd_Excel.minimumDealValue = prodserv.minimumDealValue;
                        Pd_Excel.productPrice = prodserv.productPrice;
                        Pd_Excel.type = prodserv.type;
                        Pd_Excel.url = prodserv.url;
                        Pd_Excel.description = prodserv.description;
                        if (prodserv.typeOf == 1)
                        {
                            Pd_Excel.singlemultiple = "Single";
                        }
                        else
                        {
                            Pd_Excel.singlemultiple = "Multiple";
                        }

                        if (prodserv.shareType == 1)
                        {
                            Pd_Excel.shareType = "Slab";
                        }
                        else if (prodserv.shareType == 2)
                        {
                            Pd_Excel.shareType = "Product";
                        }
                        else
                        {
                            Pd_Excel.shareType = "NA";
                        }
                        if (prodserv.lstProductServiceDetailsExcel != null)
                        {

                            foreach (ProductServiceDetailsExcel prodservdetails in prodserv.lstProductServiceDetailsExcel)
                            {
                                Pd_Excel = new ProductService_Excel();
                                Pd_Excel.firstName = _productService.firstName;
                                Pd_Excel.UJBCode = _productService.UJBCode;
                                Pd_Excel.lastName = _productService.lastName;
                                Pd_Excel.name = prodserv.name;
                                Pd_Excel.minimumDealValue = prodserv.minimumDealValue;
                                Pd_Excel.productPrice = prodserv.productPrice;
                                Pd_Excel.type = prodserv.type;
                                Pd_Excel.url = prodserv.url;
                                Pd_Excel.description = prodserv.description;
                                if (prodserv.typeOf == 1)
                                {
                                    Pd_Excel.singlemultiple = "Single";
                                }
                                else
                                {
                                    Pd_Excel.singlemultiple = "Multiple";
                                }

                                if (prodserv.shareType == 1)
                                {
                                    Pd_Excel.shareType = "Slab";
                                }
                                else if (prodserv.shareType == 2)
                                {
                                    Pd_Excel.shareType = "Product";
                                }
                                else
                                {
                                    Pd_Excel.shareType = "NA";
                                }
                                if (prodservdetails.type == 1)
                                {
                                    Pd_Excel.percAmt = "%";
                                }
                                else
                                {
                                    Pd_Excel.percAmt = "Amount";
                                }
                                Pd_Excel.value = prodservdetails.value;
                                if (prodservdetails.productName != null && prodservdetails.productName != "" && prodserv.shareType == 2)
                                {
                                    Pd_Excel.productName = prodservdetails.productName;
                                }
                                else
                                {
                                    Pd_Excel.productName = "NA";
                                }

                                if (prodservdetails.to != null && prodservdetails.to != 0 && prodserv.shareType == 1)
                                {
                                    Pd_Excel.to = prodservdetails.to.ToString();
                                }
                                else
                                {
                                    Pd_Excel.to = "NA";
                                }
                                if (prodservdetails.from != null && prodservdetails.from != 0 && prodserv.shareType == 1)
                                {
                                    Pd_Excel.from = prodservdetails.from.ToString();
                                }
                                else
                                {
                                    Pd_Excel.from = "NA";
                                }
                                res._productService_Excel.Add(Pd_Excel);
                            }
                        }
                        else
                        {
                            res._productService_Excel.Add(Pd_Excel);
                        }
                    }

                }
                else
                {
                    res._productService_Excel.Add(Pd_Excel);
                }
            }


            res._subscriptionDetailsExcel = new List<SubscriptionDetailsExcel>();

            List<SubscriptionDetails_Excel> _subscriptionDetails_Excel = new List<SubscriptionDetails_Excel>();
            _subscriptionDetails_Excel = Get_LP_SubscriptionDetails();
            SubscriptionDetailsExcel SubscriptionDetailsExcel = new SubscriptionDetailsExcel();
            foreach (SubscriptionDetails_Excel SD in _subscriptionDetails_Excel)
            {
                SubscriptionDetailsExcel = new SubscriptionDetailsExcel();

                SubscriptionDetailsExcel.firstName = SD.firstName;
                SubscriptionDetailsExcel.UJBCode = SD.UJBCode;
                SubscriptionDetailsExcel.lastName = SD.lastName;
                SubscriptionDetailsExcel.RegisterationDate = SD.RegisterationDate;
                SubscriptionDetailsExcel.BusinessRegisterationDate = SD.BusinessRegisterationDate;
                SubscriptionDetailsExcel.RenewalDate = SD.RenewalDate;
                SubscriptionDetailsExcel.FeeAmount = SD.FeeAmount;
                SubscriptionDetailsExcel.PaidAmount = SD.PaidAmount;
                SubscriptionDetailsExcel.BalanceAmount = SD.BalanceAmount;

                if (SD.Payment_details != null)
                {
                    foreach (FeeDetails FD in SD.Payment_details)
                    {
                        SubscriptionDetailsExcel = new SubscriptionDetailsExcel();
                        SubscriptionDetailsExcel.firstName = SD.firstName;
                        SubscriptionDetailsExcel.UJBCode = SD.UJBCode;
                        SubscriptionDetailsExcel.lastName = SD.lastName;
                        SubscriptionDetailsExcel.RegisterationDate = SD.RegisterationDate;
                        SubscriptionDetailsExcel.BusinessRegisterationDate = SD.BusinessRegisterationDate;
                        SubscriptionDetailsExcel.RenewalDate = SD.RenewalDate;
                        SubscriptionDetailsExcel.FeeAmount = SD.FeeAmount;
                        SubscriptionDetailsExcel.PaidAmount = SD.PaidAmount;
                        SubscriptionDetailsExcel.BalanceAmount = SD.BalanceAmount;
                        SubscriptionDetailsExcel.Amount = FD.Amount;
                        SubscriptionDetailsExcel.TransactionID = FD.TransactionID;
                        SubscriptionDetailsExcel.StartDate = FD.StartDate;
                        SubscriptionDetailsExcel.EndDate = FD.EndDate;
                        SubscriptionDetailsExcel.TransactionDate = FD.TransactionDate;
                        SubscriptionDetailsExcel.PaymentDate = FD.PaymentDate;
                        SubscriptionDetailsExcel.PaymentMode = FD.PaymentMode;
                        res._subscriptionDetailsExcel.Add(SubscriptionDetailsExcel);
                    }
                }
                else
                {
                    res._subscriptionDetailsExcel.Add(SubscriptionDetailsExcel);
                }
            }

            return res;
        }


        public List<SubscriptionDetails_Excel> Get_LP_SubscriptionDetails()
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var res1 = new List<SubscriptionDetails_Excel>();

            DateTime ApproveOn;
            //  int CountryId = _userDetails.Find(x => x._id == UserId).FirstOrDefault().countryId;

            List<SubscriptionDetails_Excel> result = new List<SubscriptionDetails_Excel>();
            // var filter = Builders<User>.Filter.Eq(x => x._id, UserId);
            var filter = Builders<User>.Filter.Eq(x => x.Role, "Listed Partner");  //Builders<User>.Filter.Empty;
            var UserExcelInfo = _partners.Find(filter).Project(x => new UserExcel()
            {
                id = x._id,
                firstName = x.firstName,
                lastName = x.lastName,
                middleName = x.middleName,
                myMentorCode = x.myMentorCode,
                Role = x.Role
            }).ToList();

            foreach (var item in UserExcelInfo)
            {
                var res = new SubscriptionDetails_Excel();
                res.firstName = item.firstName;
                res.lastName = item.lastName;
                res.UJBCode = item.myMentorCode;
                res.Payment_details = new List<FeeDetails>();
                if (_bsns.Find(x => x.UserId == item.id).CountDocuments() > 0)
                {
                    res.bussinessId = _bsns.Find(x => x.UserId == item.id).FirstOrDefault().Id;
                    res.BusinessRegisterationDate = _bsns.Find(x => x.UserId == item.id).FirstOrDefault().Created.created_On;
                }

                if (_FeeSubscription.Find(x => x.userId == item.id && x.feeType == "5d5a4534339dce0154441aac").CountDocuments() > 0)
                {
                    DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
                    var filter1 = Builders<SubscriptionDetails>.Filter.Gte(x => x.EndDate, CurrentDate);
                    filter1 = filter1 & Builders<SubscriptionDetails>.Filter.Lte(x => x.StartDate, CurrentDate);
                    filter1 = filter1 & Builders<SubscriptionDetails>.Filter.Eq(x => x.feeType, "5d5a4534339dce0154441aac");
                    filter1 = filter1 & Builders<SubscriptionDetails>.Filter.Eq(x => x.userId, item.id);
                    if (_FeeSubscription.Find(filter1).CountDocuments() > 0)
                    {
                        DateTime EndDate = _FeeSubscription.Find(filter1).FirstOrDefault().EndDate;
                        res.RenewalDate = EndDate.AddDays(1);
                    }
                    else
                    {
                        DateTime EndDate = _FeeSubscription.Find(x => x.userId == item.id).SortByDescending(x => x.EndDate).FirstOrDefault().EndDate;
                        res.RenewalDate = EndDate.AddDays(1);
                    }
                }
                else
                {
                    res.RenewalDate = null;
                }
                string SusbscriptionId = "";
                var Approveed = _bsns.Find(x => x.UserId == item.id).FirstOrDefault().isApproved;
                if (Approveed != null)
                {
                    ApproveOn = _bsns.Find(x => x.UserId == item.id).FirstOrDefault().isApproved.ApprovedOn;
                }

                List<FeePaymentDetails> feePaymentList = new List<FeePaymentDetails>();

                feePaymentList = _Feepayment.Find(x => x.userId == item.id && x.feeType == "5d5a4534339dce0154441aac").ToList();

                SubscriptionDetails subscription = new SubscriptionDetails();
                List<SubscriptionDetails> subs = new List<SubscriptionDetails>();

                DateTime CurrentDate1 = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
                //CurrentDate = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day);

                var filter12 = Builders<SubscriptionDetails>.Filter.Gte(x => x.EndDate, CurrentDate1);
                filter12 = filter12 & Builders<SubscriptionDetails>.Filter.Lte(x => x.StartDate, CurrentDate1);
                filter12 = filter12 & Builders<SubscriptionDetails>.Filter.Eq(x => x.feeType, "5d5a4534339dce0154441aac");
                filter12 = filter12 & Builders<SubscriptionDetails>.Filter.Eq(x => x.userId, item.id);

                if (_FeeSubscription.Find(filter12).CountDocuments() > 0)
                {
                    subscription = _FeeSubscription.Find(filter12).FirstOrDefault();
                    SusbscriptionId = subscription._id;
                    res.FeeAmount = subscription.Amount;
                    DateTime FromDate = subscription.StartDate;
                    DateTime EndDate = subscription.EndDate;

                    var filter2 = Builders<SubscriptionDetails>.Filter.Eq(x => x.feeType, "5d5a4534339dce0154441aac");
                    filter2 = filter2 & Builders<SubscriptionDetails>.Filter.Eq(x => x.userId, item.id);
                    filter2 = filter2 & Builders<SubscriptionDetails>.Filter.Ne(x => x._id, SusbscriptionId);

                    List<FeePaymentDetails> feePay = new List<FeePaymentDetails>();
                    if (_FeeSubscription.Find(filter2).CountDocuments() > 0)
                    {
                        DateTime CompareDate = _FeeSubscription.Find(filter2).SortByDescending(x => x.Created.created_On).FirstOrDefault().StartDate;

                        // List<FeePaymentDetails> feePay = new List<FeePaymentDetails>();
                        feePay = _Feepayment.Find(x => x.userId == item.id && x.feeType == "5d5a4534339dce0154441aac" && x.ConvertedPaymentDate <= FromDate).ToList();
                        if (feePay.Where(x => x.ConvertedPaymentDate >= CompareDate).ToList().Count() > 0)
                        {
                            feePay = feePay.Where(x => x.ConvertedPaymentDate >= CompareDate).ToList();
                            res.PaidAmount = feePay.Sum(x => x.amount);
                        }
                        else
                        {
                            res.PaidAmount = 0;
                        }
                    }
                    else
                    {
                        feePay = _Feepayment.Find(x => x.userId == item.id && x.feeType == "5d5a4534339dce0154441aac" && x.ConvertedPaymentDate <= FromDate).ToList();
                        if (feePay.ToList().Count() > 0)
                        {
                            res.PaidAmount = feePay.Sum(x => x.amount);
                        }
                        else
                        {
                            res.PaidAmount = feePay.Sum(x => x.amount);
                        }
                    }
                    res.BalanceAmount = res.FeeAmount - res.PaidAmount;

                    //foreach (var fefe in feePay)
                    //{
                    //    FeeDetails fefe1 = new FeeDetails();
                    //    fefe1.Amount = fefe.amount;
                    //    fefe1.TransactionID = fefe.referenceNo;
                    //    fefe1.TransactionDate = (fefe.transactionDate == BsonNull.Value ? null : fefe.transactionDate);
                    //    fefe1.PaymentDate = fefe.ConvertedPaymentDate;
                    //    fefe1.StartDate = FromDate;
                    //    fefe1.EndDate = EndDate;
                    //    fefe1.PaymentMode = Enum.GetName(typeof(PaymentType), int.Parse(fefe.paymentType));

                    //    res.feeBreakUp1.Add(fefe1);
                    //}

                    foreach (var item1 in feePaymentList)
                    {
                        List<SubscriptionDetails> subs1 = new List<SubscriptionDetails>();
                        SubscriptionDetails subscription1 = new SubscriptionDetails();
                        DateTime Date = item1.ConvertedPaymentDate;
                        DateTime End = Date.AddYears(1).AddDays(-1);
                        var filter3 = Builders<SubscriptionDetails>.Filter.Gte(x => x.StartDate, Date);
                        filter3 = filter3 & Builders<SubscriptionDetails>.Filter.Lte(x => x.StartDate, End);
                        filter3 = filter3 & Builders<SubscriptionDetails>.Filter.Eq(x => x.feeType, "5d5a4534339dce0154441aac");
                        filter3 = filter3 & Builders<SubscriptionDetails>.Filter.Eq(x => x.userId, item.id);

                        if (_FeeSubscription.Find(filter3).CountDocuments() > 0)
                        {
                            subscription1 = _FeeSubscription.Find(filter3).FirstOrDefault();

                            FeeDetails fee = new FeeDetails();
                            fee.Amount = item1.amount;
                            fee.TransactionID = item1.referenceNo == BsonNull.Value ? "NA" : item1.referenceNo;
                            fee.TransactionDate = (item1.transactionDate == BsonNull.Value ? null : item1.transactionDate);
                            fee.PaymentDate = item1.ConvertedPaymentDate;
                            fee.StartDate = subscription1.StartDate;
                            fee.EndDate = subscription1.EndDate;
                            fee.PaymentMode = Enum.GetName(typeof(PaymentType), int.Parse(item1.paymentType));

                            res.Payment_details.Add(fee);
                        }
                    }
                }

                else if (_FeeSubscription.Find(x => x.userId == item.id && x.feeType == "5d5a4534339dce0154441aac").CountDocuments() > 0)
                {
                    FeeStructure fee = new FeeStructure();
                    int CountryId1 = _partners.Find(x => x._id == item.id).FirstOrDefault().countryId;
                    DateTime CurrentDate2 = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));

                    var filter3 = Builders<FeeStructure>.Filter.Gte(x => x.ToDate, CurrentDate2);
                    filter3 = filter3 & Builders<FeeStructure>.Filter.Lte(x => x.FromDate, CurrentDate2);
                    filter3 = filter3 & Builders<FeeStructure>.Filter.Eq(x => x.FeeTypeId, "5d5a4534339dce0154441aac");
                    filter3 = filter3 & Builders<FeeStructure>.Filter.Eq(x => x.CountryId, CountryId1);
                    if (_FeeStructure.Find(filter3).CountDocuments() > 0)
                    {
                        res.FeeAmount = _FeeStructure.Find(filter3).FirstOrDefault().Amount;
                    }

                    List<SubscriptionDetails> SubsList = new List<SubscriptionDetails>();
                    string SubId = ""; DateTime LastStartDate = DateTime.MinValue; DateTime LastEndDate = DateTime.MinValue;
                    SubsList = _FeeSubscription.Find(x => x.userId == item.id && x.feeType == "5d5a4534339dce0154441aac").ToList();
                    foreach (var item3 in SubsList)
                    {
                        List<FeePaymentDetails> feePay = new List<FeePaymentDetails>();
                        int index = SubsList.IndexOf(item3);
                        if (index == 0)
                        {
                            feePay = _Feepayment.Find(x => x.userId == item.id && x.feeType == "5d5a4534339dce0154441aac" && x.ConvertedPaymentDate <= item3.StartDate).ToList();
                            foreach (var item1 in feePay)
                            {
                                FeeDetails fee1 = new FeeDetails();
                                fee1.Amount = item1.amount;
                                fee1.TransactionID = item1.referenceNo;
                                fee1.TransactionDate = (item1.transactionDate == BsonNull.Value ? null : item1.transactionDate);
                                fee1.PaymentDate = item1.ConvertedPaymentDate;
                                fee1.StartDate = item3.StartDate;
                                fee1.EndDate = item3.EndDate;
                                fee1.PaymentMode = Enum.GetName(typeof(PaymentType), int.Parse(item1.paymentType));

                                res.Payment_details.Add(fee1);
                            }
                            LastStartDate = item3.StartDate;
                            LastEndDate = item3.EndDate;
                        }
                        else
                        {
                            feePay = _Feepayment.Find(x => x.userId == item.id && x.feeType == "5d5a4534339dce0154441aac" && x.ConvertedPaymentDate <= item3.StartDate && x.ConvertedPaymentDate >= LastStartDate).ToList();

                            foreach (var item1 in feePay)
                            {
                                FeeDetails fee1 = new FeeDetails();
                                fee1.Amount = item1.amount;
                                fee1.TransactionID = item1.referenceNo;
                                fee1.TransactionDate = (item1.transactionDate == BsonNull.Value ? null : item1.transactionDate);
                                fee1.PaymentDate = item1.ConvertedPaymentDate;
                                fee1.StartDate = item3.StartDate;
                                fee1.EndDate = item3.EndDate;
                                fee1.PaymentMode = Enum.GetName(typeof(PaymentType), int.Parse(item1.paymentType));

                                res.Payment_details.Add(fee1);
                            }
                            LastStartDate = item3.StartDate;
                            LastEndDate = item3.EndDate;

                        }
                    }

                    List<FeePaymentDetails> feeDetails = new List<FeePaymentDetails>();
                    feeDetails = _Feepayment.Find(x => x.userId == item.id && x.feeType == "5d5a4534339dce0154441aac" && x.ConvertedPaymentDate <= CurrentDate1).ToList();
                    if (feeDetails.Where(x => x.ConvertedPaymentDate >= LastStartDate).Count() > 0)
                    {
                        feeDetails = feeDetails.Where(x => x.ConvertedPaymentDate >= LastStartDate).ToList();
                        res.PaidAmount = feeDetails.Sum(x => x.amount);
                    }
                    else
                    {
                        res.PaidAmount = 0;
                    }

                    res.BalanceAmount = res.FeeAmount - res.PaidAmount;
                    foreach (var fefe in feeDetails)
                    {
                        FeeDetails fefe1 = new FeeDetails();
                        fefe1.Amount = fefe.amount;
                        fefe1.TransactionID = fefe.referenceNo;
                        fefe1.TransactionDate = (fefe.transactionDate == BsonNull.Value ? null : fefe.transactionDate);
                        fefe1.PaymentDate = fefe.ConvertedPaymentDate;
                        fefe1.StartDate = LastEndDate.AddDays(1);
                        fefe1.EndDate = LastEndDate.AddDays(1).AddYears(1).AddDays(-1);
                        fefe1.PaymentMode = Enum.GetName(typeof(PaymentType), int.Parse(fefe.paymentType));

                        res.Payment_details.Add(fefe1);
                    }

                }

                else
                {
                    List<FeeStructure> _feeStructure = new List<FeeStructure>();
                    FeeStructure fee = new FeeStructure();
                    int CountryId1 = _partners.Find(x => x._id == item.id).FirstOrDefault().countryId;
                    DateTime CurrentDate4 = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
                    //CurrentDate1 = new DateTime(CurrentDate1.Year, CurrentDate1.Month, CurrentDate1.Day);

                    var filter6 = Builders<FeeStructure>.Filter.Gte(x => x.ToDate, CurrentDate4);
                    filter6 = filter6 & Builders<FeeStructure>.Filter.Lte(x => x.FromDate, CurrentDate4);
                    filter6 = filter6 & Builders<FeeStructure>.Filter.Eq(x => x.FeeTypeId, "5d5a4534339dce0154441aac");
                    filter6 = filter6 & Builders<FeeStructure>.Filter.Eq(x => x.CountryId, CountryId1);
                    if (_FeeStructure.Find(filter6).CountDocuments() > 0)
                    {
                        res.FeeAmount = _FeeStructure.Find(filter6).FirstOrDefault().Amount;

                        List<FeePaymentDetails> feePay = new List<FeePaymentDetails>();
                        feePay = _Feepayment.Find(x => x.userId == item.id && x.feeType == "5d5a4534339dce0154441aac" && x.ConvertedPaymentDate <= CurrentDate1).ToList();
                        if (feePay.Count() > 0)
                        {
                            res.PaidAmount = feePay.Sum(x => x.amount);
                        }
                        res.BalanceAmount = res.FeeAmount - res.PaidAmount;

                        foreach (var item2 in feePaymentList)
                        {
                            FeeDetails fee1 = new FeeDetails();
                            fee1.Amount = item2.amount;
                            fee1.TransactionID = item2.referenceNo;
                            fee1.TransactionDate = (item2.transactionDate == BsonNull.Value ? null : item2.transactionDate);
                            fee1.PaymentDate = item2.ConvertedPaymentDate;
                            fee1.StartDate = null;
                            fee1.EndDate = null;
                            fee1.PaymentMode = Enum.GetName(typeof(PaymentType), int.Parse(item2.paymentType)).Replace("_", " ");

                            res.Payment_details.Add(fee1);
                        }
                    }

                    //else
                    //{
                    //    res._messages.Add(new Message_Info { Message = "Fee not found", Type = Message_Type.SUCCESS.ToString() });
                    //}
                }
                result.Add(res);
            }
            return result;
        }


        public List<ProductServiceExcel> Get_Products_Service()
        {
            List<ProductServiceExcel> res = new List<ProductServiceExcel>();

            var filter = Builders<User>.Filter.Eq(x => x.Role, "Listed Partner");  //Builders<User>.Filter.Empty;
            var UserExcelInfo = _partners.Find(filter).Project(x => new UserExcel()
            {
                id = x._id,
                firstName = x.firstName,
                lastName = x.lastName,
                middleName = x.middleName,
                myMentorCode = x.myMentorCode,
                Role = x.Role


            }).ToList();

            foreach (var item in UserExcelInfo)
            {
                ProductServiceExcel result = new ProductServiceExcel();
                var businessid = _bsns.Find(x => x.UserId == item.id).FirstOrDefault().Id;
                //var catids = _bsns.Find(x => x.UserId == userId).Project(x => x.Categories).FirstOrDefault();
                //var filter4 = Builders<BusinessDetails>.Filter.Eq(x => x.UserId, item.id);
                result.firstName = item.firstName;
                result.lastName = item.lastName;
                result.UJBCode = item.myMentorCode;

                List<ProductService> _productService = new List<ProductService>();
                _productService = _products.Find(x => x.bussinessId == businessid && x.isActive).Project(x => new ProductService
                {
                    Id = x.Id,
                    name = x.name,
                    type = x.type,
                    url = x.url,
                    productPrice = x.productPrice,
                    minimumDealValue = x.minimumDealValue,
                    isActive = x.isActive,
                    description = x.description,

                    shareType = x.shareType,
                    typeOf = x.typeOf
                }).ToList();

                foreach (var r in _productService)
                {
                    List<ProductServiceDetailsExcel> _productServiceDetailsExcel = new List<ProductServiceDetailsExcel>();
                    _productServiceDetailsExcel = _productsDetails.Find(x => x.prodservId == r.Id && x.isActive).Project(x => new ProductServiceDetailsExcel
                    {
                        productName = x.productName,
                        Id = x.Id,
                        from = x.from,
                        to = x.to,
                        isActive = x.isActive,
                        type = x.type,
                        value = x.value,

                    }).ToList();
                    r.lstProductServiceDetailsExcel = _productServiceDetailsExcel;

                }
                result.lstProductServiceExcel = _productService;
                res.Add(result);
            }

            return res;
        }

        public UserKYCExcel Get_UserKYC_Excel(string UserId)
        {
            UserKYCExcel _kycdetails = new UserKYCExcel();
            var filter = Builders<UserKYCDetails>.Filter.Eq(x => x.UserId, UserId);
            if (_partnerKYC.Find(filter).CountDocuments() > 0)
            {
                _kycdetails = _partnerKYC.Find(filter).Project(x => new UserKYCExcel()
                {

                    KYCIsApprovedFlag = x.IsApproved.Flag,
                    KYCReason = x.IsApproved.Reason,
                    KYCReasonId = x.IsApproved.ReasonId,
                    KYVApproveOn = x.IsApproved.ApprovedOn,
                }).FirstOrDefault();

                //if (_kycdetails.KYCReasonId != 0)
                //{
                //    _kycdetails.KYCReason = Enum.GetName(typeof(RejectReasons), _kycdetails.KYCReasonId);
                //}
                if (_kycdetails.KYCIsApprovedFlag == true)
                {
                    _kycdetails.KYCIsApproved = "Approved";
                }
                else
                {
                    _kycdetails.KYCIsApproved = "Rejected";
                }
            }
            return _kycdetails;
        }

        public UserOtherDetailExcel Get_UserOtherDetails_Excel(string UserId)
        {
            UserOtherDetailExcel _otherdetails = new UserOtherDetailExcel();
            var filter = Builders<UserOtherDetails>.Filter.Eq(x => x.UserId, UserId);
            if (_userOtherDetails.Find(filter).CountDocuments() > 0)
            {
                _otherdetails = _userOtherDetails.Find(filter).Project(x => new UserOtherDetailExcel()
                {
                    //    maritalStatus = x.maritalStatus,
                    //   nationality = x.nationality,
                    Hobbies = x.Hobbies,
                    //    aboutMe = x.aboutMe,
                    //     areaOfInterest = x.areaOfInterest,
                    canImpartTraining = x.canImpartTraining
                }).FirstOrDefault();
            }

            return _otherdetails;
        }

        public UserBusinessDetailsExcel Get_UserBsnsDetails_Excel(string UserId)
        {
            UserBusinessDetailsExcel _bsnsdetails = new UserBusinessDetailsExcel();
            var filter = Builders<BusinessDetails>.Filter.Eq(x => x.UserId, UserId);
            if (_bsns.Find(filter).CountDocuments() > 0)
            {
                _bsnsdetails = _bsns.Find(filter).Project(x => new UserBusinessDetailsExcel()
                {
                    Categories = x.Categories,
                    Tagline = x.Tagline,
                    CompanyName = x.CompanyName,
                    BusinessEmail = x.BusinessEmail,
                    WebSiteURL = x.WebsiteUrl,
                    Locality = x.BusinessAddress.Locality,
                    Flat_Wing = x.BusinessAddress.Flat_Wing,
                    Location = x.BusinessAddress.Location,
                    GSTNumber = x.GSTNumber,
                    PanNo = x.BusinessPan.PanNumber,
                    AverageRating = x.averageRating,
                    BsnsIsApprovedFlag = x.isApproved.Flag,
                    BsnsIsAppovedReason = x.isApproved.Reason,
                    BsnsApproveOn = x.isApproved.ApprovedOn,
                    isSubscriptionActive = x.isSubscriptionActive
                }).FirstOrDefault();

                if (_bsnsdetails.isSubscriptionActive == false)
                {
                    _bsnsdetails.SusbscriptionActive = "Inactive";
                }
                else
                {
                    _bsnsdetails.SusbscriptionActive = "Active";
                }

                if (_bsnsdetails.BsnsIsApprovedFlag == 0)
                {
                    _bsnsdetails.BsnsIsApproved = "Pending";
                }
                else if (_bsnsdetails.BsnsIsApprovedFlag == 1)
                {
                    _bsnsdetails.BsnsIsApproved = "Approved";
                }
                else
                {
                    _bsnsdetails.BsnsIsApproved = "Pending";
                }
                foreach (var cat in _bsnsdetails.Categories)
                {
                    _bsnsdetails.Categories1 = (_category.Find(x => x.Id == cat).FirstOrDefault().categoryName) + " , ";
                }
                _bsnsdetails.Categories1 = _bsnsdetails.Categories1.TrimEnd(',');
            }
            return _bsnsdetails;
        }

        public Get_Connector_Request GetConnectorList(String UserId)
        {
            var res = new Get_Connector_Request();
            var MyMentorCode = _partners.Find(x => x._id == UserId & (x.myMentorCode != null | x.myMentorCode != "")).FirstOrDefault().myMentorCode.ToString();
            var MentorCode = _partners.Find(x => x._id == UserId & (x.mentorCode != null | x.mentorCode != "")).FirstOrDefault().mentorCode.ToString();
            if (!string.IsNullOrEmpty(MyMentorCode))
            {
                var filter = (Builders<User>.Filter.Eq(x => x.mentorCode, MyMentorCode));
                filter = filter & (Builders<User>.Filter.Nin(x => x.Role, new[] { "Guest" }));
                ////filter = filter & (Builders<User>.Filter.Eq(x => x.isActive, true));
                res.ConnectorUserInfo = _partners.Find(filter).Project(x =>
                         new Connector
                         {
                             _id = x._id,
                             firstName = x.firstName,
                             lastName = x.lastName,
                             emailId = x.emailId,
                             mobileNumber = x.mobileNumber,
                             language = x.language,
                             myMentorCode = x.myMentorCode,
                             mentorCode = x.mentorCode,
                             imageURL = x.ImageURL,
                             countryCode = x.countryCode,
                             //imageType = x.imageType,
                             //base64Image = x.base64Image,
                             Role = x.Role,
                             address = new Address
                             {
                                 locality = x.address.locality,
                                 location = x.address.location,
                                 flatWing = x.address.flatWing
                             }
                         }).ToList();
            }
            if (!string.IsNullOrEmpty(MentorCode))
            {

                var Filter1 = Builders<User>.Filter.Eq(x => x.myMentorCode, MentorCode);
                /// Filter1 = Filter1 & (Builders<User>.Filter.Eq(x => x.isActive, true));
                res.MentorUserInfo = _partners.Find(Filter1).Project(x =>
                          new Connector
                          {
                              _id = x._id,
                              firstName = x.firstName,
                              lastName = x.lastName,
                              emailId = x.emailId,
                              mobileNumber = x.mobileNumber,
                              language = x.language,
                              myMentorCode = x.myMentorCode,
                              mentorCode = x.mentorCode,
                              imageURL = x.ImageURL,
                              countryCode = x.countryCode,
                              //imageType = x.imageType,
                              //base64Image = x.base64Image,
                              Role = x.Role,
                              address = new Address
                              {
                                  locality = x.address.locality,
                                  location = x.address.location,
                                  flatWing = x.address.flatWing
                              }
                          }).ToList();
            }
            res.NoOfConnects = res.ConnectorUserInfo.Count();
            if (res.NoOfConnects == 0)
            {
                res.ConnectorUserInfo = null;
            }
            return res;
        }

        public Get_Request Get_PartnerList(int size, int page)
        {
            var res = new Get_Request();
            var filter = Builders<User>.Filter.Empty;
            //var filter = Builders<User>.Filter.Eq(x => x.Role, "Client Partner");
            res.PartnersList = _partners.Find(filter).Project(x =>
                                 new PartnerUsers()
                                 {
                                     _id = x._id,
                                     firstName = x.firstName,
                                     lastName = x.lastName,
                                     emailId = x.emailId,
                                     mobileNumber = x.mobileNumber,
                                     birthDate = x.birthDate,
                                     gender = x.gender,
                                     passiveIncome = x.passiveIncome,
                                     address = new Address
                                     {
                                         location = x.address.location,
                                         flatWing = x.address.flatWing,
                                         locality = x.address.locality
                                     },
                                     Role = x.Role,
                                     isActive = x.isActive,

                                     UJBCode = x.myMentorCode,
                                     isPartnerAgreementAccepted = x.isPartnerAgreementAccepted,
                                     isMembershipAgreementAccepted = x.isMembershipAgreementAccepted,
                                     Created = new Created
                                     {

                                         created_By = x.Created.created_By,
                                         created_On = x.Created.created_On

                                     }
                                 }
                     ).SortByDescending(x => x.Created.created_On).ToList();
            //.ThenByDescending(x=>x.isActive)
            //   res.PartnersList = res.PartnersList.Take(20);

            foreach (var r in res.PartnersList)
            {
                var _kycFilter = Builders<UserKYCDetails>.Filter.Eq(x => x.UserId, r._id);
                var _bsnsFilter = Builders<BusinessDetails>.Filter.Eq(x => x.UserId, r._id);

                if (_partnerKYC.Find(_kycFilter).CountDocuments() > 0)
                {
                    if (_partnerKYC.Find(_kycFilter).FirstOrDefault().IsApproved != null)
                    {
                        r.isApproved = _partnerKYC.Find(_kycFilter).FirstOrDefault().IsApproved.Flag;
                        r.ReasonId = _partnerKYC.Find(_kycFilter).FirstOrDefault().IsApproved.ReasonId;
                        if (r.ReasonId != 0)
                        {
                            r.Reason = Enum.GetName(typeof(RejectReasons), r.ReasonId);
                        }
                    }

                    if (r.Role == "Partner")
                    {
                        //      _kycFilter = _kycFilter & (Builders<UserKYCDetails>.Filter.Eq(x => x.PanCard.PanNumber, BsonString.Empty)
                        //        //  | Builders<UserKYCDetails>.Filter.Eq(x => x.PanCard.PanNumber, null)
                        //          | Builders<UserKYCDetails>.Filter.Eq(x => x.PanCard.ImageURL, BsonString.Empty)
                        //        //   | Builders<UserKYCDetails>.Filter.Eq(x => x.PanCard.ImageURL, null)
                        //          | Builders<UserKYCDetails>.Filter.Eq(x => x.AdharCard.AdharNumber, BsonString.Empty)
                        //        //   | Builders<UserKYCDetails>.Filter.Eq(x => x.AdharCard.AdharNumber, null)
                        //          | Builders<UserKYCDetails>.Filter.Eq(x => x.AdharCard.BackImageURL, BsonString.Empty)
                        //        //   | Builders<UserKYCDetails>.Filter.Eq(x => x.AdharCard.BackImageURL, null)
                        //          | Builders<UserKYCDetails>.Filter.Eq(x => x.AdharCard.FrontImageURL, BsonString.Empty)
                        //         //  | Builders<UserKYCDetails>.Filter.Eq(x => x.AdharCard.FrontImageURL, null)
                        //          | Builders<UserKYCDetails>.Filter.Eq(x => x.BankDetails.AccountHolderName, BsonString.Empty)
                        //           // | Builders<UserKYCDetails>.Filter.Eq(x => x.BankDetails.AccountHolderName, null)
                        //  | Builders<UserKYCDetails>.Filter.Eq(x => x.BankDetails.AccountNumber, BsonString.Empty)
                        //   //| Builders<UserKYCDetails>.Filter.Eq(x => x.BankDetails.AccountNumber, null)
                        //  | Builders<UserKYCDetails>.Filter.Eq(x => x.BankDetails.BankName, BsonString.Empty)
                        // //  | Builders<UserKYCDetails>.Filter.Eq(x => x.BankDetails.BankName, null)
                        //  | Builders<UserKYCDetails>.Filter.Eq(x => x.BankDetails.IFSCCode, BsonString.Empty)
                        // //   | Builders<UserKYCDetails>.Filter.Eq(x => x.BankDetails.IFSCCode, null)
                        //  | Builders<UserKYCDetails>.Filter.Eq(x => x.BankDetails.ImageURL, BsonString.Empty)
                        ////  | Builders<UserKYCDetails>.Filter.Eq(x => x.BankDetails.ImageURL, null)
                        //  );



                        //      long count1= _partnerKYC.Find(_kycFilter).CountDocuments();
                        //      var kyccomplete = false;
                        //      if (count1 == 0)
                        //      {
                        //          kyccomplete = true;
                        //      }
                        //      else
                        //      {
                        //          kyccomplete = false;
                        //      }
                        r.is_Active = r.isActive == true ? 1 : 0;
                        var _kycdetails = _partnerKYC.Find(x => x.UserId == r._id).FirstOrDefault();
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
                                    r.isKycComplete = false;

                                }
                                else
                                {
                                    r.isKycComplete = true;
                                }
                            }
                            else
                            {
                                r.isKycComplete = false;
                            }

                            if (_kycdetails.BankDetails != null)
                            {
                                if (
                                 string.IsNullOrEmpty(_kycdetails.BankDetails.ImageURL)
                                || string.IsNullOrEmpty(_kycdetails.BankDetails.AccountNumber) || string.IsNullOrEmpty(_kycdetails.BankDetails.AccountHolderName)
                                || string.IsNullOrEmpty(_kycdetails.BankDetails.BankName) || string.IsNullOrEmpty(_kycdetails.BankDetails.IFSCCode))
                                {
                                    r.isBankComplete = false;

                                }
                                else
                                {
                                    r.isBankComplete = true;
                                }
                            }
                            else
                            {
                                r.isBankComplete = false;
                            }
                        }
                        else
                        {
                            r.isKycComplete = false;
                        }

                    }
                    else if (r.Role == "Listed Partner")
                    {
                        r.isKycComplete = true;
                    }
                    else
                    {
                        r.isKycComplete = false;
                    }
                }
                else
                {
                    r.isApproved = false;
                    r.isKycComplete = false;
                }


                if (_bsns.Find(_bsnsFilter).CountDocuments() > 0)
                {
                    if (_bsns.Find(_bsnsFilter).FirstOrDefault().isApproved != null)
                    {

                        r.isBusinessApproved = Enum.GetName(typeof(BusinessApproval), _bsns.Find(_bsnsFilter).FirstOrDefault().isApproved.Flag);
                        if (_bsns.Find(_bsnsFilter).FirstOrDefault().isApproved.Reason != BsonNull.Value)
                        {
                            r.BusinessApprovalReason = _bsns.Find(_bsnsFilter).FirstOrDefault().isApproved.Reason;
                        }
                        else
                        {
                            r.BusinessApprovalReason = "";
                        }
                    }
                }
                if (!r.isActive)
                {
                    r.order_by = 0;

                }
                else
                {
                    if (r.isActive && r.Role == "Listed Partner" && r.isBusinessApproved == "Pending" && r.isKycComplete && r.isApproved)
                    {
                        r.order_by = 6;
                    }
                    else if (r.isActive && r.Role == "Partner" && !r.isApproved && r.isKycComplete && r.ReasonId == 0)
                    {
                        r.order_by = 5;
                    }
                    else if (r.isActive && r.Role == "Partner" && r.isApproved && !r.isBankComplete && r.isKycComplete)
                    {
                        r.order_by = 4;
                    }
                    else if (r.isActive && !r.isKycComplete && r.Role == "Partner")
                    {
                        r.order_by = 3;
                    }
                    else if (r.isActive && !r.isKycComplete && r.Role == "Listed Partner")
                    {
                        r.order_by = 2;
                    }

                    else
                    {
                        r.order_by = 1;
                    }
                }


            }

            res.totalCount = Convert.ToInt32(_partners.Find(_ => true).CountDocuments().ToString());



            var orderByResult = from s in res.PartnersList
                                orderby s.order_by descending
                                select s;

            //var orderByResultIsaActive = from v in orderByResult
            //                             orderby v.is_Active descending
            //                    select v;
            res.PartnersList = orderByResult.ToList();

            return res;

        }


    }
}
