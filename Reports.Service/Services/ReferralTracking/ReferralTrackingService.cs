using Reports.Service.Models.ReferralTracking;
using Reports.Service.Repositories.ReferralTracking;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;
using UJBHelper.DataModel;
using System;
using static UJBHelper.Common.Common;

namespace Reports.Service.Services.ReferralTracking
{
    public class ReferralTrackingService : IReferralTracking
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
        private readonly IMongoCollection<PaymentDetails> _paymentDetails;

        public ReferralTrackingService(IConfiguration config)
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
            _paymentDetails = database.GetCollection<PaymentDetails>("PaymentDetails");
            _ReferralAgreedPercentage = database.GetCollection<ReferralAgreedPercentage>("ReferralAgreedPercentage");
        }

        public bool Check_If_User_Exists(string userId)
        {
            return _users.Find(x => x._id == userId).CountDocuments() > 0;
        }

        public Get_Request Get_Referral_Details(Post_Request request)
        {
            var res = new Get_Request();
            var filter = Builders<Leads>.Filter.Empty;

            //var businessId = _businessDetails.Find(x => x.UserId == userId).Project(x => x.Id).FirstOrDefault();

            //var categoriIds = _businessDetails.Find(x => x.Id == businessId).FirstOrDefault().Categories;

            //var categoryNames = new List<string>();
            //if (categoriIds != null)
            //{
            //    categoryNames = _categories.Find(x => categoriIds.Contains(x.Id)).Project(x => x.categoryName).ToList();
            //}
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
            if (request.FromDate != null && request.ToDate != null)
            {
                filter = Builders<Leads>.Filter.Gte(x => x.dealStatusUpdatedOn, request.FromDate);
                filter = filter & Builders<Leads>.Filter.Lte(x => x.dealStatusUpdatedOn, request.ToDate);
            }

            if (request.FromDate != null && request.ToDate == null)
            {
                filter = Builders<Leads>.Filter.Lte(x => x.dealStatusUpdatedOn, request.FromDate);
                //  filter = filter & Builders<Leads>.Filter.Lte(x => x.dealStatusUpdatedOn, request.ToDate);
            }
            if (request.FromDate == null && request.ToDate != null)
            {
                //filter = Builders<Leads>.Filter.Gte(x => x.dealStatusUpdatedOn, request.FromDate);
                filter = Builders<Leads>.Filter.Gte(x => x.dealStatusUpdatedOn, request.ToDate);
            }
            //   filter = filter & Builders<Leads>.Filter.Eq(x => x.dealStatusUpdatedOn, fromDate);
            //   filter = filter & Builders<Leads>.Filter.Eq(x => x.dealStatusUpdatedOn, CountryId);
            // if (_FeeStructure.Find(filter).CountDocuments() > 0)
            res.ReferredList = _lead.Find(filter).Project(x => new Request_Info
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
                dealStatus = (int)x.dealStatus,
                referralStatusUpdatedOn = x.refStatusUpdatedOn,
                referralStatusUpdatedby = x.referralStatusUpdatedby,
                ReferralCode = x.ReferralCode,
                DealStatusUpdatedOn = x.dealStatusUpdatedOn.ToString("dd/MM/yyyy"),
            }).SortByDescending(x => x.dealStatusUpdatedOn).ToList();

            foreach (var refs in res.ReferredList)
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
                refs.referralStatusValue = Enum.GetName(typeof(UJBHelper.Common.Common.ReferralStatusEnum), refs.refStatus);
                if (refs.refStatus == 1)
                {
                    var dealstatusvalue = _dealStatus.Find(x => x.StatusId == refs.dealStatus).Project(x => x.StatusName).FirstOrDefault();

                    refs.dealStatusValue = dealstatusvalue;
                    refs.dealStatus = refs.dealStatus;
                }
                else
                {
                    refs.dealStatusValue = Enum.GetName(typeof(UJBHelper.Common.Common.ReferralStatusEnum), refs.refStatus);
                    refs.dealStatus = refs.refStatus;
                }

                var user_id = _businessDetails.Find(x => x.Id == refs.businessId).Project(x => x.UserId).FirstOrDefault();
                var bussinessuser = _users.Find(y => y._id == user_id).FirstOrDefault();
                //var BussEmailId = _businessDetails.Find(x => x.Id == refs.businessId).Project(x => x.BusinessEmail).FirstOrDefault();
                // var EmailId = _users.Find(y => y._id == user_id).Project(y => y.emailId).FirstOrDefault();
                //var countryCode = _users.Find(y => y._id == user_id).Project(y => y.countryCode).FirstOrDefault();
                //var UserName = _users.Find(y => y._id == user_id).Project(y => y.firstName + " " + y.lastName).FirstOrDefault();
                refs.clientPartnerDetails = _businessDetails.Find(x => x.Id == refs.businessId).Project(x => new ClientPartnerDetails
                {
                    name = x.CompanyName,
                    tagline = x.Tagline,
                    emailId = bussinessuser.emailId,
                    mobileNumber = bussinessuser.mobileNumber,
                    countryCode = bussinessuser.countryCode,
                    BussEmailId = x.BusinessEmail
                }).FirstOrDefault();
                if (refs.clientPartnerDetails.name == "")
                {
                    refs.clientPartnerDetails.name = bussinessuser.firstName + " " + bussinessuser.lastName;
                }
            }
            List<Request_Info> myList = new List<Request_Info>();
            //var myList=new 
            if (request.StatusID != "" && request.StatusID != null)

            {
                res.ReferredList = res.ReferredList.FindAll(x => x.dealStatusValue == request.StatusID);
            }

            return res;

        }

        public Put_Response Get_Referral_Excel_Details(Put_Request request)
        {
            var res = new Put_Response();
            var filter = Builders<Leads>.Filter.Empty;
            filter = Builders<Leads>.Filter.In(x => x.Id, request.ReferralIdID);

            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
            var Referred_List = _lead.Find(filter).Project(x => new Put_Response_Info
            {
                referralId = x.Id,
                //categories = categoryNames,
                dateCreated = x.referralDate.Value.ToString("dd/MM/yyyy"),
                isForSelf = x.isForSelf,
                productId = x.referredProductORServicesId,
                productName = x.referredProductORServices,
                referralDescription = x.referralDescription,
                dealValue = x.dealValue.ToString(),
                SharedValuewithUJB = x.shareReceivedByUJB.value,
                SharedwithUJB = x.shareReceivedByUJB.percOrAmount,
                rejectionReason = x.rejectionReason,
                businessId = x.referredBusinessId,
                referredToDetails = x.referredTo,
                refStatus = (int)x.referralStatus,
                dealStatus = (int)x.dealStatus,
                referralStatusUpdatedOn = x.refStatusUpdatedOn,
                referralStatusUpdatedby = x.referralStatusUpdatedby,
                ReferralCode = x.ReferralCode,
                DealStatusUpdatedOn = x.dealStatusUpdatedOn.ToString("dd/MM/yyyy"),
            }).SortByDescending(x => x.dealStatusUpdatedOn).ToList();

            foreach (var reffal in Referred_List)
            {
                var payment = _paymentDetails.Find(x => x.leadId == reffal.referralId && x.paymentFrom != "UJB").ToList();
                foreach (var pay in payment)
                {
                    Put_Response_Info ref_s = new Put_Response_Info();
                    //  ref_s = reffal;
                    ref_s.referralId = reffal.referralId;
                    //categories = categoryNames,
                    ref_s.dateCreated = reffal.dateCreated;
                    ref_s.isForSelf = reffal.isForSelf;
                    ref_s.productId = reffal.productId;
                    ref_s.productName = reffal.productName;
                    ref_s.referralDescription = reffal.referralDescription;
                    ref_s.dealValue = reffal.dealValue;
                    ref_s.SharedValuewithUJB = reffal.SharedValuewithUJB;
                    ref_s.SharedwithUJB = reffal.SharedwithUJB;
                    ref_s.rejectionReason = reffal.rejectionReason;
                    ref_s.businessId = reffal.businessId;
                    ref_s.referredToDetails = reffal.referredToDetails;
                    ref_s.refStatus = reffal.refStatus;
                    ref_s.dealStatus = reffal.dealStatus; 
                    ref_s.referralStatusUpdatedOn = reffal.referralStatusUpdatedOn;
                    ref_s.referralStatusUpdatedby = reffal.referralStatusUpdatedby;
                    ref_s.ReferralCode = reffal.ReferralCode;
                    ref_s.DealStatusUpdatedOn = reffal.DealStatusUpdatedOn;


                    var referredById = _lead.Find(x => x.Id == reffal.referralId).Project(x => x.referredBy.userId).FirstOrDefault();
                    var user = _users.Find(x => x._id == referredById).FirstOrDefault();
                    ref_s.referredByDetails = new ReferredByDetails
                    {
                        referredByName = user.firstName + " " + user.lastName,
                        referredByMobileNo = user.mobileNumber,
                        referredByEmailId = user.emailId,
                        referredByCountryCode = user.countryCode
                    };
                    var PartnerMentor = _users.Find(x => x.myMentorCode == user.mentorCode).FirstOrDefault();
                    ref_s.PartnerMentorDetails = new MentorDetails
                    {
                        Name = PartnerMentor.firstName + " " + PartnerMentor.lastName,
                        MobileNo = PartnerMentor.mobileNumber,
                        EmailId = PartnerMentor.emailId,
                        CountryCode = PartnerMentor.countryCode
                    };
                    ref_s.referralStatusValue = Enum.GetName(typeof(UJBHelper.Common.Common.ReferralStatusEnum), reffal.refStatus);
                    if (ref_s.refStatus == 1)
                    {
                        var dealstatusvalue = _dealStatus.Find(x => x.StatusId == reffal.dealStatus).Project(x => x.StatusName).FirstOrDefault();
                        ref_s.dealStatusValue = dealstatusvalue;
                        ref_s.dealStatus = reffal.dealStatus;
                    }
                    else
                    {
                        ref_s.dealStatusValue = Enum.GetName(typeof(UJBHelper.Common.Common.ReferralStatusEnum), reffal.refStatus);
                        ref_s.dealStatus = reffal.refStatus;
                    }

                    var user_id = _businessDetails.Find(x => x.Id == reffal.businessId).Project(x => x.UserId).FirstOrDefault();
                    var bussinessuser = _users.Find(y => y._id == user_id).FirstOrDefault();

                    ref_s.clientPartnerDetails = _businessDetails.Find(x => x.Id == reffal.businessId).Project(x => new ClientPartnerDetails
                    {
                        name = x.CompanyName,
                        tagline = x.Tagline,
                        emailId = bussinessuser.emailId,
                        mobileNumber = bussinessuser.mobileNumber,
                        countryCode = bussinessuser.countryCode,
                        BussEmailId = x.BusinessEmail
                    }).FirstOrDefault();

                    var LpMentor = _users.Find(x => x.myMentorCode == bussinessuser.mentorCode).FirstOrDefault();
                    ref_s.LPMentorDetails = new MentorDetails
                    {
                        Name = LpMentor.firstName + " " + LpMentor.lastName,
                        MobileNo = LpMentor.mobileNumber,
                        EmailId = LpMentor.emailId,
                        CountryCode = LpMentor.countryCode
                    };
                    if (ref_s.clientPartnerDetails.name == "")
                    {
                        ref_s.clientPartnerDetails.name = bussinessuser.firstName + " " + bussinessuser.lastName;
                    }


                    var PartnerPayment = _paymentDetails.Find(x => x.AdjustedTransactionIds.Contains(pay.PaymentTransactionId) && x.paymentTo.Contains(referredById) && x.sharedId == 2).FirstOrDefault();
                    var PartnerMentorPayment = _paymentDetails.Find(x => x.AdjustedTransactionIds.Contains(pay.PaymentTransactionId) && x.paymentTo.Contains(PartnerMentor._id) && x.sharedId == 3).FirstOrDefault();
                    var LPMentorPayment = _paymentDetails.Find(x => x.AdjustedTransactionIds.Contains(pay.PaymentTransactionId) && x.paymentTo.Contains(LpMentor._id) && x.sharedId == 4).FirstOrDefault();
                    ref_s.Amount_Recieved_by_lp = pay.CPReceivedAmt;
                    ref_s.Amount_Recieved_by_lp_Payment_Date = Convert.ToDateTime(pay.paymentDate);
                    ref_s.Amount_Recieved_by_lp_Payment_Mode = Enum.GetName(typeof(PaymentType), pay.paymentType);
                    ref_s.Amount_Transfered_to_ujb_Amount = pay.Amount; ;
                    ref_s.Balance_remaining_with_UJB = pay.amtRecvdbyUJB;
                    if (PartnerPayment != null)
                    {
                        ref_s.UJb_transfered_to_partner = PartnerPayment.Amount;
                        ref_s.UJb_transfered_to_partner_payment_Date = Convert.ToDateTime(PartnerPayment.paymentDate);
                        ref_s.UJb_transfered_to_partner_payment_mode = Enum.GetName(typeof(PaymentType), PartnerPayment.paymentType);
                    }
                    if (PartnerMentorPayment != null)
                    {
                        ref_s.Amount_Ujb_transfered_to_partner_mentor = PartnerMentorPayment.Amount;
                        ref_s.Amount_Ujb_transfered_to_partner_mentor_payment_Date = Convert.ToDateTime(PartnerMentorPayment.paymentDate);
                        ref_s.Amount_Ujb_transfered_to_partner_mentor_payment_mode = Enum.GetName(typeof(PaymentType), PartnerMentorPayment.paymentType);
                    }
                    if (LPMentorPayment != null)
                    {
                        ref_s.Amount_ujb_ransfered_to_lpmentor = LPMentorPayment.Amount;
                        ref_s.Amount_ujb_ransfered_to_lpmentor_payment_Date = Convert.ToDateTime(LPMentorPayment.paymentDate);
                        ref_s.Amount_ujb_ransfered_to_lpmentor_payment_mode = Enum.GetName(typeof(PaymentType), LPMentorPayment.paymentType);
                    }
                    res.ReferredList.Add(ref_s);

                }
                if (payment.Count == 0)
                {
                    Put_Response_Info ref_s = new Put_Response_Info();
                    ref_s = reffal;

                    var referredById = _lead.Find(x => x.Id == reffal.referralId).Project(x => x.referredBy.userId).FirstOrDefault();
                    var user = _users.Find(x => x._id == referredById).FirstOrDefault();
                    ref_s.referredByDetails = new ReferredByDetails
                    {
                        referredByName = user.firstName + " " + user.lastName,
                        referredByMobileNo = user.mobileNumber,
                        referredByEmailId = user.emailId,
                        referredByCountryCode = user.countryCode
                    };
                    var PartnerMentor = _users.Find(x => x.myMentorCode == user.mentorCode).FirstOrDefault();
                    ref_s.PartnerMentorDetails = new MentorDetails
                    {
                        Name = PartnerMentor.firstName + " " + PartnerMentor.lastName,
                        MobileNo = PartnerMentor.mobileNumber,
                        EmailId = PartnerMentor.emailId,
                        CountryCode = PartnerMentor.countryCode
                    };
                    ref_s.referralStatusValue = Enum.GetName(typeof(UJBHelper.Common.Common.ReferralStatusEnum), reffal.refStatus);
                    if (ref_s.refStatus == 1)
                    {
                        var dealstatusvalue = _dealStatus.Find(x => x.StatusId == reffal.dealStatus).Project(x => x.StatusName).FirstOrDefault();
                        ref_s.dealStatusValue = dealstatusvalue;
                        ref_s.dealStatus = reffal.dealStatus;
                    }
                    else
                    {
                        ref_s.dealStatusValue = Enum.GetName(typeof(UJBHelper.Common.Common.ReferralStatusEnum), reffal.refStatus);
                        ref_s.dealStatus = reffal.refStatus;
                    }

                    var user_id = _businessDetails.Find(x => x.Id == reffal.businessId).Project(x => x.UserId).FirstOrDefault();
                    var bussinessuser = _users.Find(y => y._id == user_id).FirstOrDefault();

                    ref_s.clientPartnerDetails = _businessDetails.Find(x => x.Id == reffal.businessId).Project(x => new ClientPartnerDetails
                    {
                        name = x.CompanyName,
                        tagline = x.Tagline,
                        emailId = bussinessuser.emailId,
                        mobileNumber = bussinessuser.mobileNumber,
                        countryCode = bussinessuser.countryCode,
                        BussEmailId = x.BusinessEmail
                    }).FirstOrDefault();

                    var LpMentor = _users.Find(x => x.myMentorCode == bussinessuser.mentorCode).FirstOrDefault();
                    ref_s.LPMentorDetails = new MentorDetails
                    {
                        Name = LpMentor.firstName + " " + LpMentor.lastName,
                        MobileNo = LpMentor.mobileNumber,
                        EmailId = LpMentor.emailId,
                        CountryCode = LpMentor.countryCode
                    };
                    if (ref_s.clientPartnerDetails.name == "")
                    {
                        ref_s.clientPartnerDetails.name = bussinessuser.firstName + " " + bussinessuser.lastName;
                    }

                    res.ReferredList.Add(ref_s);
                }
            }
            // List<Request_Info> myList = new List<Request_Info>();



            return res;

        }
    }
}
