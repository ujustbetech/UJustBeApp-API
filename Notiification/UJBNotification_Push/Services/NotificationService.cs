using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using UJBHelper.Common;
using UJBHelper.Data;
using UJBHelper.DataModel;

namespace UJBNotification_Push.Services
{
    public class NotificationService
    {
        private readonly IMongoCollection<Notification> _notification;
        private readonly IMongoCollection<NotificationQueue> _notificationQueue;
        private readonly IMongoCollection<Leads> _lead;
        private readonly IMongoCollection<DbProductService> _productsAndService;
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private readonly IMongoCollection<AdminUser> _adminUsers;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<UserKYCDetails> _userKycDetails;
        private readonly IMongoCollection<UserOtherDetails> _userOtherDetails;
        private readonly IMongoCollection<SubscriptionDetails> _FeeSubscription;
        private readonly IMongoCollection<FeeStructure> _FeeStructure;
        private readonly IMongoCollection<FeePaymentDetails> _Feepayment;

        public NotificationService()
        {
            var client = new MongoClient(DbHelper.GetConnectionString());
            var database = client.GetDatabase(DbHelper.GetDatabaseName());
            _notification = database.GetCollection<Notification>("Notification");
            _FeeStructure = database.GetCollection<FeeStructure>("FeeStructure");
            _notificationQueue = database.GetCollection<NotificationQueue>("NotificationQueue");
            _lead = database.GetCollection<Leads>("Leads");
            _productsAndService = database.GetCollection<DbProductService>("ProductsServices");
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
            _users = database.GetCollection<User>("Users");
            _userKycDetails = database.GetCollection<UserKYCDetails>("UserKYCDetails");
            _userOtherDetails = database.GetCollection<UserOtherDetails>("UsersOtherDetails");
            _adminUsers = database.GetCollection<AdminUser>("AdminUsers");
            _FeeSubscription = database.GetCollection<SubscriptionDetails>("SubscriptionDetails");
            _Feepayment = database.GetCollection<FeePaymentDetails>("FeePaymentDetails");
        }

        internal bool Check_If_Notification_Inserted()
        {
            // return _notificationQueue.Find(x => x.status == "new" && x.Type=="Email").CountDocuments() != 0;
          //  var builder = Builders<NotificationQueue>.Filter.In(x => x.status, new[] { "new", "failure" }) & Builders<NotificationQueue>.Filter.Eq(x => x.Type, "Email" );
           // var filter = builder.Filter.Lt("Age", 40) & builder.Eq("FirstName", "Peter");
            return _notificationQueue.Find(Builders<NotificationQueue>.Filter.In(x => x.status, new[] { "new","failure" }) & Builders<NotificationQueue>.Filter.Eq(x => x.Type, "Push"))
                      .ToEnumerable()
                      //.Where(x => x.attemptedTrial < x.totalTrial)
                      .Count() != 0;
        }

        internal bool Check_If_Referral_Crossed_72_Hours()
        {
            var leadIds = _lead.Find(x => x.referralStatus == (int)ReferralStatusEnum.Pending && x.dealStatus == 0).Project(x => x.Id).ToList();

            foreach (var leadId in leadIds)
            {
                var lead = _lead.Find(x => x.Id == leadId).FirstOrDefault();
                TimeSpan difference = DateTime.Now - lead.Created.created_On;
                double hours = difference.TotalHours;
                if (hours >= 72)
                {
                    var rejectionReason = "Auto Rejected due to exceeding time of accepting / rejecting a referral ( 72 Hours )";

                    Auto_Reject_Referral(leadId, rejectionReason);

                    var nq = new Notification_Queue();
                    nq.Add_To_Queue("", leadId, "", "", "new", "Lead Auto Rejection", "", "Email", "UJBAdmin", "");
                    //nq.Add_To_Queue("", leadId, "", "", "new", "Lead Auto Rejection", "", "SMS", "UJBAdmin", "");

                    nq.Add_To_Queue("", leadId, "", "", "new", "Lead Auto Rejection", "", "Push", "Referrer", "");
                    nq.Add_To_Queue("", leadId, "", "", "new", "Lead Auto Rejection", "", "Email", "Referrer", "");
                    //nq.Add_To_Queue("", leadId, "", "", "new", "Lead Auto Rejection", "", "SMS", "Referrer", "");

                    nq.Add_To_Queue("", leadId, "", "", "new", "Lead Auto Rejection", "", "Push", "Business", "");
                    nq.Add_To_Queue("", leadId, "", "", "new", "Lead Auto Rejection", "", "Email", "Business", "");
                    //nq.Add_To_Queue("", leadId, "", "", "new", "Lead Auto Rejection", "", "SMS", "Business", "");
                    return true;
                }
            }
            return false;
        }

        internal bool Check_If_KYC_Pending()
        {
            var nq = new Notification_Queue();
            var userids = _userKycDetails.Find(x => string.IsNullOrWhiteSpace(x.AdharCard.AdharNumber) || string.IsNullOrWhiteSpace(x.PanCard.PanNumber) || string.IsNullOrWhiteSpace(x.BankDetails.AccountNumber)).Project(x => x.UserId).ToList();

            foreach (var u in userids)
            {

                if (_notificationQueue.Find(x => x.userId == u && x.Event == "Skip KYC" && x.status != "new").CountDocuments() != 0 && _notificationQueue.Find(x => x.userId == u && x.Event == "Skip KYC" && x.status == "new").CountDocuments() == 0)
                {
                    var notifyDate = _notificationQueue.Find(x => x.userId == u && x.Event == "Skip KYC" && x.status != "new").SortByDescending(x => x.dateOfNotification).Project(x => x.dateOfNotification).FirstOrDefault();
                    //= notification.dateOfNotification;
                    if ((DateTime.Now - notifyDate).TotalHours >= 48)
                    {
                        nq.Add_To_Queue(u, "", "", "", "new", "Skip KYC", "", "Push", "User", "");
                        nq.Add_To_Queue(u, "", "", "", "new", "Skip KYC", "", "Email", "User", "");
                        Notification notify_template = new Notification();
                        notify_template = nq.Get_Notification_Template("Skip KYC");

                        bool isaalowed = notify_template.Data.Where(x => x.Receiver == "User").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                        if (isaalowed)
                        {
                            nq.Add_To_Queue(u, "", "", "", "new", "Skip KYC", "", "SMS", "User", "");
                        }
                        return true;
                    }
                }
                else
                {
                    var userDate = _users.Find(x => x._id == u).Project(x => x.Created.created_On).FirstOrDefault();

                    if ((DateTime.Now - userDate).TotalHours >= 48)
                    {
                        nq.Add_To_Queue(u, "", "", "", "new", "Skip KYC", "", "Push", "User", "");
                        nq.Add_To_Queue(u, "", "", "", "new", "Skip KYC", "", "Email", "User", "");
                        Notification notify_template = new Notification();
                        notify_template = nq.Get_Notification_Template("Skip KYC");

                        bool isaalowed = notify_template.Data.Where(x => x.Receiver == "User").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                        if (isaalowed)
                        {
                            nq.Add_To_Queue(u, "", "", "", "new", "Skip KYC", "", "SMS", "User", "");
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        internal string Get_Incomplete_Profile_User()
        {
            var userIds = _users.Find(x => x.isActive && x.Role != "Guest").Project(x => x._id).ToList();

            foreach (var userId in userIds)
            {
                if (_notificationQueue.Find(x => x.userId == userId && x.Event == "Incomplete Profile" && x.status != "new").CountDocuments() != 0 && _notificationQueue.Find(x => x.userId == userId && x.Event == "Incomplete Profile" && x.status == "new").CountDocuments() == 0)
                {
                    var notifyDate = _notificationQueue.Find(x => x.userId == userId && x.Event == "Incomplete Profile" && x.status != "new").Project(x => x.dateOfNotification).FirstOrDefault();

                    if ((DateTime.Now - notifyDate).TotalDays >= 14 && (DateTime.Now - notifyDate).TotalDays < 15)
                    {
                        var user = _users.Find(x => x._id == userId).FirstOrDefault();

                        if (user.Role == "Partner")
                        {
                            var userkycDetails = _userKycDetails.Find(x => x.UserId == userId).FirstOrDefault();
                            var userOtherDetails = _userOtherDetails.Find(x => x.UserId == userId).FirstOrDefault();

                            if (string.IsNullOrWhiteSpace(userOtherDetails.aboutMe) || string.IsNullOrWhiteSpace(userOtherDetails.Hobbies)
                                || string.IsNullOrWhiteSpace(userOtherDetails.areaOfInterest)
                                || string.IsNullOrWhiteSpace(userOtherDetails.maritalStatus)
                                || string.IsNullOrWhiteSpace(user.ImageURL)
                            //|| string.IsNullOrWhiteSpace(user.base64Image)
                            )
                            {
                                return userId;
                            }
                        }
                        else
                        {
                            var businessDetails = _businessDetails.Find(x => x.UserId == userId).FirstOrDefault();
                            if (
                                string.IsNullOrWhiteSpace(businessDetails.Logo.logoImageURL)
                                || string.IsNullOrWhiteSpace(businessDetails.BannerDetails.ImageURL)
                                || string.IsNullOrWhiteSpace(businessDetails.CompanyName)
                                || string.IsNullOrWhiteSpace(businessDetails.BusinessAddress.Flat_Wing)
                                || string.IsNullOrWhiteSpace(businessDetails.BusinessAddress.Location)
                                || string.IsNullOrWhiteSpace(businessDetails.BusinessAddress.Locality)
                                || string.IsNullOrWhiteSpace(businessDetails.WebsiteUrl)
                                )
                            {
                                return userId;
                            }
                        }

                    }
                }
                else
                {
                    var userDate = _users.Find(x => x._id == userId).Project(x => x.Created.created_On).FirstOrDefault();

                    if ((DateTime.Now - userDate).TotalDays >= 14 && (DateTime.Now - userDate).TotalDays < 15)
                    {
                        var user = _users.Find(x => x._id == userId).FirstOrDefault();

                        if (user.Role == "Partner")
                        {
                            var userkycDetails = _userKycDetails.Find(x => x.UserId == userId).FirstOrDefault();
                            var userOtherDetails = _userOtherDetails.Find(x => x.UserId == userId).FirstOrDefault();
                            if (userOtherDetails == null || userkycDetails == null)
                            {
                                return userId;
                            }
                            if (string.IsNullOrWhiteSpace(userOtherDetails.aboutMe) || string.IsNullOrWhiteSpace(userOtherDetails.Hobbies)
                            || string.IsNullOrWhiteSpace(userOtherDetails.areaOfInterest)
                            || string.IsNullOrWhiteSpace(userOtherDetails.maritalStatus)
                            || string.IsNullOrWhiteSpace(user.ImageURL)
                        //|| string.IsNullOrWhiteSpace(user.base64Image)
                        )
                            {
                                return userId;
                            }
                        }
                        else
                        {
                            var businessDetails = _businessDetails.Find(x => x.UserId == userId).FirstOrDefault();
                            if (
                                string.IsNullOrWhiteSpace(businessDetails.Logo.logoImageURL)
                                || string.IsNullOrWhiteSpace(businessDetails.BannerDetails.ImageURL)
                                || string.IsNullOrWhiteSpace(businessDetails.CompanyName)
                                || string.IsNullOrWhiteSpace(businessDetails.BusinessAddress.Flat_Wing)
                                || string.IsNullOrWhiteSpace(businessDetails.BusinessAddress.Location)
                                || string.IsNullOrWhiteSpace(businessDetails.BusinessAddress.Locality)
                                || string.IsNullOrWhiteSpace(businessDetails.WebsiteUrl)
                                )
                            {
                                return userId;
                            }
                        }
                    }
                }
            }
            return "";
        }

        internal bool Check_If_Profile_Incomplete()
        {
            var nq = new Notification_Queue();
            var userIds = _users.Find(x => x.isActive && x.Role != "Guest").Project(x => x._id).ToList();

            foreach (var userId in userIds)
            {
                if (_notificationQueue.Find(x => x.userId == userId && x.Event == "Incomplete Profile" && x.status != "new").CountDocuments() != 0 && _notificationQueue.Find(x => x.userId == userId && x.Event == "Incomplete Profile" && x.status == "new").CountDocuments() == 0)
                {
                    var notifyDate = _notificationQueue.Find(x => x.userId == userId && x.Event == "Incomplete Profile" && x.status != "new").SortByDescending(x => x.dateOfNotification).Project(x => x.dateOfNotification).FirstOrDefault();

                    if ((DateTime.Now - notifyDate).TotalDays >= 14)
                    {
                        var user = _users.Find(x => x._id == userId).FirstOrDefault();

                        if (user.Role == "Partner")
                        {
                            var userkycDetails = _userKycDetails.Find(x => x.UserId == userId).FirstOrDefault();
                            var userOtherDetails = _userOtherDetails.Find(x => x.UserId == userId).FirstOrDefault();
                            if (userkycDetails == null || userOtherDetails == null)
                            {

                                nq.Add_To_Queue(userId, "", "", "", "new", "Incomplete Profile", "", "Push", "User", "");
                                nq.Add_To_Queue(userId, "", "", "", "new", "Incomplete Profile", "", "Email", "User", "");
                                return true;
                            }
                            if (string.IsNullOrWhiteSpace(userOtherDetails.aboutMe) || string.IsNullOrWhiteSpace(userOtherDetails.Hobbies)
                            || string.IsNullOrWhiteSpace(userOtherDetails.areaOfInterest)
                            || string.IsNullOrWhiteSpace(userOtherDetails.maritalStatus)
                            || string.IsNullOrWhiteSpace(user.ImageURL)
                            || string.IsNullOrWhiteSpace(user.ImageURL)
                        )
                            {
                                nq.Add_To_Queue(userId, "", "", "", "new", "Incomplete Profile", "", "Push", "User", "");
                                nq.Add_To_Queue(userId, "", "", "", "new", "Incomplete Profile", "", "Email", "User", "");
                                return true;
                            }
                        }
                        else
                        {
                            var businessDetails = _businessDetails.Find(x => x.UserId == userId).FirstOrDefault();
                            if (
                                string.IsNullOrWhiteSpace(businessDetails.Logo.logoImageURL)
                                || string.IsNullOrWhiteSpace(businessDetails.BannerDetails.ImageURL)
                                || string.IsNullOrWhiteSpace(businessDetails.CompanyName)
                                || string.IsNullOrWhiteSpace(businessDetails.BusinessAddress.Flat_Wing)
                                || string.IsNullOrWhiteSpace(businessDetails.BusinessAddress.Location)
                                || string.IsNullOrWhiteSpace(businessDetails.BusinessAddress.Locality)
                                || string.IsNullOrWhiteSpace(businessDetails.WebsiteUrl)
                                )
                            {

                                nq.Add_To_Queue(userId, "", "", "", "new", "Incomplete Profile", "", "Push", "User", "");
                                nq.Add_To_Queue(userId, "", "", "", "new", "Incomplete Profile", "", "Email", "User", "");
                                return true;
                            }
                        }

                    }
                }
                else
                {
                    var userDate = _users.Find(x => x._id == userId).Project(x => x.Created.created_On).FirstOrDefault();

                    if ((DateTime.Now - userDate).TotalDays >= 14)
                    {
                        var user = _users.Find(x => x._id == userId).FirstOrDefault();

                        if (user.Role == "Partner")
                        {
                            var userkycDetails = _userKycDetails.Find(x => x.UserId == userId).FirstOrDefault();
                            var userOtherDetails = _userOtherDetails.Find(x => x.UserId == userId).FirstOrDefault();
                            if (userkycDetails == null || userOtherDetails == null)
                            {

                                nq.Add_To_Queue(userId, "", "", "", "new", "Incomplete Profile", "", "Push", "User", "");
                                nq.Add_To_Queue(userId, "", "", "", "new", "Incomplete Profile", "", "Email", "User", "");
                                return true;
                            }
                            if (string.IsNullOrWhiteSpace(userOtherDetails.aboutMe) || string.IsNullOrWhiteSpace(userOtherDetails.Hobbies)
                                || string.IsNullOrWhiteSpace(userOtherDetails.areaOfInterest)
                                || string.IsNullOrWhiteSpace(userOtherDetails.maritalStatus)
                                || string.IsNullOrWhiteSpace(user.ImageURL)
                            // || string.IsNullOrWhiteSpace(user.base64Image)
                            )
                            {
                                nq.Add_To_Queue(userId, "", "", "", "new", "Incomplete Profile", "", "Push", "User", "");
                                nq.Add_To_Queue(userId, "", "", "", "new", "Incomplete Profile", "", "Email", "User", "");
                                return true;
                            }
                        }
                        else
                        {
                            var businessDetails = _businessDetails.Find(x => x.UserId == userId).FirstOrDefault();
                            if (
                                string.IsNullOrWhiteSpace(businessDetails.Logo.logoImageURL)
                                || string.IsNullOrWhiteSpace(businessDetails.BannerDetails.ImageURL)
                                || string.IsNullOrWhiteSpace(businessDetails.CompanyName)
                                || string.IsNullOrWhiteSpace(businessDetails.BusinessAddress.Flat_Wing)
                                || string.IsNullOrWhiteSpace(businessDetails.BusinessAddress.Location)
                                || string.IsNullOrWhiteSpace(businessDetails.BusinessAddress.Locality)
                                || string.IsNullOrWhiteSpace(businessDetails.WebsiteUrl)
                                )
                            {
                                nq.Add_To_Queue(userId, "", "", "", "new", "Incomplete Profile", "", "Push", "User", "");
                                nq.Add_To_Queue(userId, "", "", "", "new", "Incomplete Profile", "", "Email", "User", "");
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        internal string Get_No_Product_CP()
        {
            var userids = _users.Find(x => x.Role == "Listed Partner" && x.isActive).Project(x => x._id).ToList();

            foreach (var userId in userids)
            {
                var businessId = _businessDetails.Find(x => x.UserId == userId && x.isApproved.Flag == 1 && x.isSubscriptionActive).Project(x => x.Id).FirstOrDefault();

                if (_productsAndService.Find(x => x.bussinessId == businessId).CountDocuments() == 0)
                {
                    return userId;
                }
            }
            return "";
        }

        internal bool Check_If_No_Products()
        {
            var userids = _users.Find(x => x.Role == "Listed Partner" && x.isActive).Project(x => x._id).ToList();

            foreach (var userId in userids)
            {
                var businessId = _businessDetails.Find(x => x.UserId == userId && x.isApproved.Flag == 1 && x.isSubscriptionActive).Project(x => x.Id).FirstOrDefault();

                if (_productsAndService.Find(x => x.bussinessId == businessId).CountDocuments() == 0)
                {
                    var nq = new Notification_Queue();
                    nq.Add_To_Queue(userId, "", "", "", "new", "No Client Partner Products", "", "Push", "User", "");
                    nq.Add_To_Queue(userId, "", "", "", "new", "No Client Partner Products", "", "Email", "User", "");
                    Notification notify_template = new Notification();
                    notify_template = nq.Get_Notification_Template("No Client Partner Products");

                    bool isaalowed = notify_template.Data.Where(x => x.Receiver == "User").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                    if (isaalowed)
                    {
                        nq.Add_To_Queue(userId, "", "", "", "new", "No Client Partner Products", "", "SMS", "User", "");
                    }
                    return true;
                }
            }
            return false;
        }

        private double FeeAmtToBePaid(string UserId, string FeeType)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });
            double FeeAmt = 0;
            double PaidAmt = 0;
            double PendingAmt = 0;
            string SusbscriptionId = "";

            List<SubscriptionDetails> subs = new List<SubscriptionDetails>();
            SubscriptionDetails subscription = new SubscriptionDetails();

            DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
            //DateTime endDate = DateTime.Parse(DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)).ToString("MM/dd/yyyy"));
            //DateTime startDate = DateTime.Parse(DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)).ToString("dd/MM/yyyy"));
            var filter1 = Builders<SubscriptionDetails>.Filter.Gte(x => x.EndDate, CurrentDate);
            filter1 = filter1 & Builders<SubscriptionDetails>.Filter.Lte(x => x.StartDate, CurrentDate);
            filter1 = filter1 & Builders<SubscriptionDetails>.Filter.Eq(x => x.feeType, FeeType);
            filter1 = filter1 & Builders<SubscriptionDetails>.Filter.Eq(x => x.userId, UserId);
            if (_FeeSubscription.Find(filter1).CountDocuments() > 0)
            {
                List<FeePaymentDetails> feePay = new List<FeePaymentDetails>();
                subscription = _FeeSubscription.Find(filter1).FirstOrDefault();
                SusbscriptionId = subscription._id;
                FeeAmt = subscription.Amount;
                DateTime FromDate = subscription.StartDate;
                DateTime EndDate = subscription.EndDate;

                var filter2 = Builders<SubscriptionDetails>.Filter.Eq(x => x.feeType, FeeType);
                filter1 = filter1 & Builders<SubscriptionDetails>.Filter.Eq(x => x.userId, UserId);
                filter1 = filter1 & Builders<SubscriptionDetails>.Filter.Ne(x => x._id, SusbscriptionId);
                if (_FeeSubscription.Find(filter2).CountDocuments() > 0)
                {
                    DateTime CompareDate = _FeeSubscription.Find(filter2).SortByDescending(x => x.Created.created_On).FirstOrDefault().StartDate;
                    feePay = _Feepayment.Find(x => x.userId == UserId && x.feeType == FeeType && x.ConvertedPaymentDate <= FromDate && x.ConvertedPaymentDate >= CompareDate).ToList();
                    if (feePay.ToList().Count() > 0)
                    {
                        PaidAmt = feePay.Sum(x => x.amount);
                    }
                    else
                    {
                        PaidAmt = 0;
                    }
                }
                else
                {
                    feePay = _Feepayment.Find(x => x.userId == UserId && x.feeType == FeeType && x.ConvertedPaymentDate <= FromDate).ToList();
                    if (feePay.ToList().Count() > 0)
                    {
                        PaidAmt = feePay.Sum(x => x.amount);
                    }
                    else
                    {
                        PaidAmt = 0;
                    }
                }

                PendingAmt = FeeAmt - PaidAmt;
            }
            else
            {
                List<FeeStructure> _feeStructure = new List<FeeStructure>();
                FeeStructure fee = new FeeStructure();
                int CountryId1 = _users.Find(x => x._id == UserId).FirstOrDefault().countryId;
                DateTime CurrentDate1 = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));


                var filter = Builders<FeeStructure>.Filter.Gte(x => x.ToDate, CurrentDate1);
                filter = filter & Builders<FeeStructure>.Filter.Lte(x => x.FromDate, CurrentDate1);
                filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.FeeTypeId, FeeType);
                filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.CountryId, CountryId1);
                if (_FeeStructure.Find(filter).CountDocuments() > 0)
                {
                    FeeAmt = _FeeStructure.Find(filter).FirstOrDefault().Amount;

                    List<FeePaymentDetails> feePay = new List<FeePaymentDetails>();
                    feePay = _Feepayment.Find(x => x.userId == UserId && x.feeType == FeeType && x.ConvertedPaymentDate <= CurrentDate1).ToList();
                    if (feePay.Count() > 0)
                    {
                        PaidAmt = feePay.Sum(x => x.amount);
                    }
                    PendingAmt = FeeAmt - PaidAmt;
                }
            }
            return PendingAmt;

        }

        public double Check_TotalPayment_Done(string UserId, string FeeType)
        {
            bool userExist = _Feepayment.Find(x => x.userId == UserId && x.feeType == FeeType).CountDocuments() > 0;
            if (userExist)
            {
                var AmtPaid = _Feepayment.AsQueryable()
                         .Where(x => x.userId == UserId && x.feeType == FeeType)
                         .GroupBy(d => d.userId)
                         .Select(
                          g => new
                          {
                              Value = g.Sum(s => s.amount),
                          }).FirstOrDefault();
                return AmtPaid.Value;
            }
            else
            {
                return 0;
            }
        }


        //internal bool Check_If_Registeration_Pending_48Hrs()
        //{
        //    TimeSpan diff = DateTime.Now - DateTime.UtcNow;
        //    string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

        //    var userids = _users.Find(x => x.Role == "Client Partner" && x.isActive).Project(x => x._id).ToList();

        //    foreach (var userId in userids)
        //    {
        //        double AmtPaid = Check_TotalPayment_Done(userId, "5d5a450d339dce0154441aab");
        //        double FeeAmt = 0; double PendingAmt = 0;
        //        List<FeeStructure> _feeStructure = new List<FeeStructure>();
        //        FeeStructure fee = new FeeStructure();
        //        int CountryId1 = _users.Find(x => x._id == userId).FirstOrDefault().countryId;
        //        DateTime endDate1 = DateTime.Parse(DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)).ToString("MM/dd/yyyy"));
        //        DateTime startDate1 = DateTime.Parse(DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)).ToString("dd/MM/yyyy"));

        //        var filter = Builders<FeeStructure>.Filter.Gte(x => x.EndFecha, endDate1);
        //        filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.FeeTypeId, "5d5a450d339dce0154441aab");
        //        filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.CountryId, CountryId1);
        //        if (_FeeStructure.Find(filter).CountDocuments() > 0)
        //        {
        //            _feeStructure = _FeeStructure.Find(filter).ToList();
        //            FeeAmt = _feeStructure.Where(x => x.StartFecha <= startDate1).FirstOrDefault().Amount;
        //            PendingAmt = FeeAmt - AmtPaid;
        //            if (PendingAmt > 0)
        //            {
        //                var feePayment = _Feepayment.Find(x => x.userId == userId && x.feeType == "5d5a450d339dce0154441aab").SortByDescending(x => x.Created.created_On).FirstOrDefault();
        //                if (feePayment != null)
        //                {
        //                    TimeSpan difference = DateTime.Now - feePayment.Created.created_On;
        //                }

        //                int days = difference.Days;
        //                double hours = difference.TotalHours;
        //                int minutes = difference.Minutes;
        //                if (hours >= 48)
        //                {
        //                    if (_notificationQueue.Find(x => x.userId == userId && x.Event == "Membership Fee Reminder" && x.status != "new").CountDocuments() != 0)
        //                    {
        //                        var notification = _notificationQueue.Find(x => x.userId == userId && x.Event == "Membership Fee Reminder" && x.status != "new").FirstOrDefault();
        //                        var leadDate = feePayment.Created.created_On;
        //                        var notificationDate = DateTime.Parse(notification.dateOfNotification);

        //                        if ((DateTime.Now - notificationDate).TotalHours >= 48 && (DateTime.Now - notificationDate).TotalHours < 49)
        //                        {
        //                            var nq1 = new Notification_Queue();
        //                            nq1.Add_To_Queue(userId, "", "", "", "new", "Membership Fee Reminder", "", "Email", "User", "");
        //                            nq1.Add_To_Queue(userId, "", "", "", "new", "Membership Fee Reminder", "", "SMS", "User", "");
        //                            return true;
        //                        }
        //                        else
        //                        {
        //                            return false;
        //                        }

        //                    }
        //                    else
        //                    {
        //                        var nq1 = new Notification_Queue();
        //                        nq1.Add_To_Queue(userId, "", "", "", "new", "Membership Fee Reminder", "", "Email", "User", "");
        //                        nq1.Add_To_Queue(userId, "", "", "", "new", "Membership Fee Reminder", "", "SMS", "User", "");
        //                        return true;
        //                    }
        //                }
        //            }
        //        }
        //        return false;
        //    }
        //}

        internal bool Check_If_Susbscription_Pending_48Hrs()
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });
            DateTime CompareDate;

            var userids = _users.Find(x => x.Role == "Listed Partner" && x.isActive).Project(x => x._id).ToList();

            foreach (var userId in userids)
            {
                double Amt = FeeAmtToBePaid(userId, "5d5a4534339dce0154441aac");
                if (Amt > 0)
                {
                    if (_Feepayment.Find(x => x.userId == userId && x.feeType == "5d5a4534339dce0154441aac").CountDocuments() > 0)
                    {
                        var feePayment = _Feepayment.Find(x => x.userId == userId && x.feeType == "5d5a4534339dce0154441aac").SortByDescending(x => x.Created.created_On).FirstOrDefault();
                        CompareDate = feePayment.Created.created_On;
                    }
                    else
                    {
                        CompareDate = _businessDetails.Find(x => x.UserId == userId).FirstOrDefault().Created.created_On;
                    }
                    TimeSpan difference = DateTime.Now - CompareDate;
                    int days = difference.Days;
                    double hours = difference.TotalHours;
                    int minutes = difference.Minutes;
                    if (hours >= 48)
                    {
                        if (_notificationQueue.Find(x => x.userId == userId && x.Event == "Membership Fee Reminder" && x.status != "new").CountDocuments() != 0 && _notificationQueue.Find(x => x.userId == userId && x.Event == "Membership Fee Reminder" && x.status == "new").CountDocuments() == 0)
                        {
                            var notification = _notificationQueue.Find(x => x.userId == userId && x.Event == "Membership Fee Reminder" && x.status != "new").FirstOrDefault();

                            var notificationDate = notification.dateOfNotification;

                            if ((DateTime.Now - notificationDate).TotalHours >= 48 && (DateTime.Now - notificationDate).TotalHours < 49)
                            {
                                var nq1 = new Notification_Queue();
                                nq1.Add_To_Queue(userId, "", "", "", "new", "Membership Fee Reminder", "", "Email", "User", "");
                                Notification notify_template = new Notification();
                                notify_template = nq1.Get_Notification_Template("Membership Fee Reminder");

                                bool isaalowed = notify_template.Data.Where(x => x.Receiver == "User").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                                if (isaalowed)
                                {
                                    nq1.Add_To_Queue(userId, "", "", "", "new", "Membership Fee Reminder", "", "SMS", "User", "");
                                }
                                return true;
                            }
                            else
                            {
                                return false;
                            }

                        }
                        else
                        {
                            var nq1 = new Notification_Queue();
                            nq1.Add_To_Queue(userId, "", "", "", "new", "Membership Fee Reminder", "", "Email", "User", "");
                            Notification notify_template = new Notification();
                            notify_template = nq1.Get_Notification_Template("Membership Fee Reminder");

                            bool isaalowed = notify_template.Data.Where(x => x.Receiver == "User").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                            if (isaalowed)
                            {
                                nq1.Add_To_Queue(userId, "", "", "", "new", "Membership Fee Reminder", "", "SMS", "User", "");
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal bool Check_If_Meeting_Pending_48Hrs()
        {
            DateTime CompareDate;
            var userids = _users.Find(x => x.Role == "Listed Partner" && x.isActive).Project(x => x._id).ToList();

            foreach (var userId in userids)
            {
                double Amt = FeeAmtToBePaid(userId, "5d5a4541339dce0154441aad");
                if (Amt > 0)
                {

                    if (_Feepayment.Find(x => x.userId == userId && x.feeType == "5d5a4541339dce0154441aad").CountDocuments() > 0)
                    {
                        var feePayment = _Feepayment.Find(x => x.userId == userId && x.feeType == "5d5a4541339dce0154441aad").SortByDescending(x => x.Created.created_On).FirstOrDefault();
                        CompareDate = feePayment.Created.created_On;
                    }
                    else
                    {
                        CompareDate = _businessDetails.Find(x => x.UserId == userId).FirstOrDefault().Created.created_On;
                    }


                    TimeSpan difference = DateTime.Now - CompareDate;
                    int days = difference.Days;
                    double hours = difference.TotalHours;
                    int minutes = difference.Minutes;
                    if (hours >= 48)
                    {
                        if (_notificationQueue.Find(x => x.userId == userId && x.Event == "Meeting Fee Reminder" && x.status != "new").CountDocuments() != 0 && _notificationQueue.Find(x => x.userId == userId && x.Event == "Meeting Fee Reminder" && x.status == "new").CountDocuments() == 0)
                        {
                            var notification = _notificationQueue.Find(x => x.userId == userId && x.Event == "Meeting Fee Reminder" && x.status != "new").FirstOrDefault();
                            // var leadDate = feePayment.Created.created_On;
                            var notificationDate = notification.dateOfNotification;

                            if ((DateTime.Now - notificationDate).TotalHours >= 48 && (DateTime.Now - notificationDate).TotalHours < 49)
                            {
                                var nq = new Notification_Queue();
                                nq.Add_To_Queue(userId, "", "", "", "new", "Meeting Fee Reminder", "", "Email", "User", "");
                                Notification notify_template = new Notification();
                                notify_template = nq.Get_Notification_Template("Meeting Fee Reminder");

                                bool isaalowed = notify_template.Data.Where(x => x.Receiver == "User").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                                if (isaalowed)
                                {
                                    nq.Add_To_Queue(userId, "", "", "", "new", "Meeting Fee Reminder", "", "SMS", "User", "");
                                }
                                return true;
                            }
                            else
                            {
                                return false;
                            }

                        }
                        else
                        {
                            var nq = new Notification_Queue();
                            nq.Add_To_Queue(userId, "", "", "", "new", "Meeting Fee Reminder", "", "Email", "User", "");
                            Notification notify_template = new Notification();
                            notify_template = nq.Get_Notification_Template("Meeting Fee Reminder");

                            bool isaalowed = notify_template.Data.Where(x => x.Receiver == "User").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                            if (isaalowed)
                            {
                                nq.Add_To_Queue(userId, "", "", "", "new", "Meeting Fee Reminder", "", "SMS", "User", "");
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal List<string> Get_KYC_Pending_Users()
        {
            var partnerIds = _users.Find(x => x.Role == "Partner").Project(x => x._id).ToList();
            var userids = _userKycDetails.Find(x => partnerIds.Contains(x.UserId) && (string.IsNullOrWhiteSpace(x.AdharCard.AdharNumber) || string.IsNullOrWhiteSpace(x.PanCard.PanNumber) || string.IsNullOrWhiteSpace(x.BankDetails.AccountNumber))).Project(x => x.UserId).ToList();
            var idsList = new List<string>();
            foreach (var u in userids)
            {

                if (_notificationQueue.Find(x => x.userId == u && x.Event == "Skip KYC" && x.status != "new").CountDocuments() != 0)
                {
                    var notification = _notificationQueue.Find(x => x.userId == u && x.Event == "Skip KYC" && x.status != "new").FirstOrDefault();
                    var notifyDate = notification.dateOfNotification;
                    if ((DateTime.Now - notifyDate).TotalHours >= 48 && (DateTime.Now - notifyDate).TotalHours < 49)
                    {
                        idsList.Add(u);
                    }
                }
                else
                {
                    var userDate = _users.Find(x => x._id == u).Project(x => x.Created.created_On).FirstOrDefault();

                    if ((DateTime.Now - userDate).TotalHours >= 48)
                    {
                        idsList.Add(u);
                    }
                }
            }
            return idsList;
        }

        internal string Get_User_OTP(string userId)
        {
            return _users.Find(x => x._id == userId).Project(x => x.otpVerification.OTP).FirstOrDefault();
        }

        internal bool Check_if_Referral_Below_72_Hours()
        {
            var nq = new Notification_Queue();
            var leads = _lead.Find(x => x.dealStatus == (int)DealStatusEnum.NotConnected).Project(x => x.Id).ToList();

            foreach (var leadId in leads)
            {
                var leadDate = _lead.Find(x => x.Id == leadId).Project(x => x.Created.created_On).FirstOrDefault();
                if (_notificationQueue.Find(x => x.leadId == leadId && x.Event == "Auto Rejection Reminder" && x.status != "new").CountDocuments() != 0 && _notificationQueue.Find(x => x.leadId == leadId && x.Event == "Auto Rejection Reminder" && x.status == "new").CountDocuments() == 0)
                {
                    var notificationDate = _notificationQueue.Find(x => x.leadId == leadId && x.Event == "Auto Rejection Reminder" && x.status != "new").SortByDescending(x => x.dateOfNotification).Project(x => x.dateOfNotification).FirstOrDefault();


                    if ((DateTime.Now - leadDate).TotalHours >= 48 && (DateTime.Now - notificationDate).TotalHours >= 24)
                    {

                        nq.Add_To_Queue("", leadId, "", "", "new", "Auto Rejection Reminder", "", "Email", "UJBAdmin", "");
                        //nq.Add_To_Queue("", leadId, "", "", "new", "Auto Rejection Reminder", "", "SMS", "UJBAdmin", "");

                        nq.Add_To_Queue("", leadId, "", "", "new", "Auto Rejection Reminder", "", "Email", "Referrer", "");
                        //nq.Add_To_Queue("", leadId, "", "", "new", "Auto Rejection Reminder", "", "SMS", "Referrer", "");
                        nq.Add_To_Queue("", leadId, "", "", "new", "Auto Rejection Reminder", "", "Push", "Referrer", "");


                        nq.Add_To_Queue("", leadId, "", "", "new", "Auto Rejection Reminder", "", "Email", "Business", "");
                        //nq.Add_To_Queue("", leadId, "", "", "new", "Auto Rejection Reminder", "", "SMS", "Business", "");
                        nq.Add_To_Queue("", leadId, "", "", "new", "Auto Rejection Reminder", "", "Push", "Business", "");
                        return true;
                    }
                    else if ((DateTime.Now - leadDate).TotalHours >= 72 && (DateTime.Now - notificationDate).TotalHours >= 48)
                    {
                        nq.Add_To_Queue("", leadId, "", "", "new", "Auto Rejection Reminder", "", "Email", "UJBAdmin", "");
                        //nq.Add_To_Queue("", leadId, "", "", "new", "Auto Rejection Reminder", "", "SMS", "UJBAdmin", "");

                        nq.Add_To_Queue("", leadId, "", "", "new", "Auto Rejection Reminder", "", "Email", "Referrer", "");
                        //nq.Add_To_Queue("", leadId, "", "", "new", "Auto Rejection Reminder", "", "SMS", "Referrer", "");
                        nq.Add_To_Queue("", leadId, "", "", "new", "Auto Rejection Reminder", "", "Push", "Referrer", "");


                        nq.Add_To_Queue("", leadId, "", "", "new", "Auto Rejection Reminder", "", "Email", "Business", "");
                        //nq.Add_To_Queue("", leadId, "", "", "new", "Auto Rejection Reminder", "", "SMS", "Business", "");
                        nq.Add_To_Queue("", leadId, "", "", "new", "Auto Rejection Reminder", "", "Push", "Business", "");
                        return true;
                    }
                }
                else
                {
                    if ((DateTime.Now - leadDate).TotalHours > 72 && _notificationQueue.Find(x => x.leadId == leadId && x.Event == "Auto Rejection Reminder" && x.status == "new").CountDocuments() == 0)
                    {
                        return true;
                    }

                }
            }

            return false;
        }

        internal string Get_Referral_Below_72_Hours()
        {
            var leads = _lead.Find(x => x.dealStatus == (int)DealStatusEnum.NotConnected).ToList();
            foreach (var lead in leads)
            {
                if (_notificationQueue.Find(x => x.leadId == lead.Id && x.Event == "Auto Rejection Reminder" && x.status != "new").CountDocuments() != 0 && _notificationQueue.Find(x => x.leadId == lead.Id && x.Event == "Auto Rejection Reminder" && x.status == "new").CountDocuments() == 0)
                {
                    var notification = _notificationQueue.Find(x => x.leadId == lead.Id && x.Event == "Auto Rejection Reminder" && x.status != "new").FirstOrDefault();
                    var leadDate = lead.Created.created_On;
                    var notificationDate = notification.dateOfNotification;

                    if ((DateTime.Now - leadDate).TotalHours >= 48 && (DateTime.Now - leadDate).TotalHours < 49 && (DateTime.Now - notificationDate).TotalHours >= 24 && (DateTime.Now - notificationDate).TotalHours < 25)
                    {
                        return lead.Id;
                    }
                    else if ((DateTime.Now - leadDate).TotalHours >= 71 && (DateTime.Now - leadDate).TotalHours < 72 && (DateTime.Now - notificationDate).TotalHours >= 23 && (DateTime.Now - notificationDate).TotalHours < 24)
                    {
                        return lead.Id;
                    }
                }
                else
                {
                    if ((DateTime.Now - lead.Created.created_On).TotalHours > 72 && _notificationQueue.Find(x => x.leadId == lead.Id && x.Event == "Auto Rejection Reminder" && x.status == "new").CountDocuments() == 0)
                    {
                        return lead.Id;
                    }

                }
            }
            return "";
        }

        internal void Auto_Reject_Referral(string dealId, string rejectionReason)
        {
            _lead.FindOneAndUpdate(
                Builders<Leads>.Filter.Eq(x => x.Id, dealId),
                Builders<Leads>.Update.Set(x => x.rejectionReason, rejectionReason)
                .Set(x => x.referralStatus, 2)
                );
        }

        internal string Get_Guest_For_Reminder()
        {
            var userIds = _users.Find(x => x.Role == "Guest").Project(x => x._id).ToList();
            foreach (var userId in userIds)
            {
                if (_notificationQueue.Find(x => x.userId == userId && x.Event == "Guest Reminder To Upgrade" && x.status != "new").CountDocuments() != 0 && _notificationQueue.Find(x => x.userId == userId && x.Event == "Guest Reminder To Upgrade" && x.status == "new").CountDocuments() == 0)
                {
                    var notifyDate = _notificationQueue.Find(x => x.userId == userId && x.Event == "Guest Reminder To Upgrade" && x.status != "new").Project(x => x.dateOfNotification).FirstOrDefault();

                    if ((DateTime.Now - notifyDate).TotalDays >= 3 && (DateTime.Now - notifyDate).TotalDays < 4)
                    {
                        return userId;
                    }
                }
                else
                {
                    var userDate = _users.Find(x => x._id == userId).Project(x => x.Created.created_On).FirstOrDefault();

                    if ((DateTime.Now - userDate).TotalDays >= 3 && (DateTime.Now - userDate).TotalDays < 4)
                    {
                        return userId;
                    }
                }
            }
            return "";
            //var users = _users.Find(x => x.Role == "Guest").ToList();

            //var userIds = users.Select(x => x._id).ToList();
            //if (_notificationQueue.Find(x => userIds.Contains(x.userId) && x.Event == "Guest Reminder To Upgrade" && x.status != "new").CountDocuments() != 0)
            //{
            //    var notification = _notificationQueue.Find(x => userIds.Contains(x.userId) && x.Event == "Guest Reminder To Upgrade" && x.status != "new").ToList();
            //    foreach (var n in notification)
            //    {
            //        TimeSpan difference = DateTime.Now - DateTime.Parse(n.dateOfNotification);
            //        int days = difference.Days;
            //        int hours = difference.Hours;
            //        int minutes = difference.Minutes;
            //        if (days == 3 && hours == 0)
            //        {
            //            return n.userId;
            //        }
            //    }
            //}
            //else
            //{
            //    foreach (var u in users)
            //    {

            //        if ((DateTime.Now - u.Created.created_On).Days > 3)
            //        {
            //            return u._id;
            //        }

            //    }
            //}
            //return "";
        }

        internal List<string> Get_Admin_EmailIds()
        {
            return _adminUsers.Find(x => x.isActive && x.allowNotifications).Project(x => x.emailId).ToList();
        }

        internal bool Check_If_Guest_Reminder()
        {
            var userIds = _users.Find(x => x.Role == "Guest").Project(x => x._id).ToList();
            foreach (var userId in userIds)
            {
                if (_notificationQueue.Find(x => x.userId == userId && x.Event == "Guest Reminder To Upgrade" && x.status != "new").CountDocuments() != 0 && _notificationQueue.Find(x => x.userId == userId && x.Event == "Guest Reminder To Upgrade" && x.status == "new").CountDocuments() == 0)
                {
                    var notifyDate = _notificationQueue.Find(x => x.userId == userId && x.Event == "Guest Reminder To Upgrade" && x.status != "new").SortByDescending(x => x.dateOfNotification).Project(x => x.dateOfNotification).FirstOrDefault();

                    if ((DateTime.Now - notifyDate).TotalDays >= 3)
                    {
                        var nq = new Notification_Queue();
                        nq.Add_To_Queue(userId, "", "", "", "new", "Guest Reminder To Upgrade", "", "Push", "User", "");
                        nq.Add_To_Queue(userId, "", "", "", "new", "Guest Reminder To Upgrade", "", "Email", "User", "");
                        return true;
                    }
                }
                else
                {
                    var userDate = _users.Find(x => x._id == userId).Project(x => x.Created.created_On).FirstOrDefault();

                    if ((DateTime.Now - userDate).TotalDays >= 3)
                    {
                        var nq = new Notification_Queue();
                        nq.Add_To_Queue(userId, "", "", "", "new", "Guest Reminder To Upgrade", "", "Push", "User", "");
                        nq.Add_To_Queue(userId, "", "", "", "new", "Guest Reminder To Upgrade", "", "Email", "User", "");
                        return true;
                    }
                }
            }
            return false;
        }

        internal List<string> Get_Admin_MobileNumbers()
        {
            return _adminUsers.Find(x => x.isActive && x.allowNotifications).Project(x => x.countryCode.Substring(1) + x.mobileNumber).ToList();
        }

        internal string Get_Referral_Crossed_72_Hours()
        {
            var leadIds = _lead.Find(x => x.referralStatus == (int)ReferralStatusEnum.Pending && x.dealStatus == 0).Project(x => x.Id).ToList();

            foreach (var leadId in leadIds)
            {
                var lead = _lead.Find(x => x.Id == leadId).FirstOrDefault();
                TimeSpan difference = DateTime.Now - lead.Created.created_On;
                int days = difference.Days;
                double hours = difference.TotalHours;
                int minutes = difference.Minutes;
                if (hours >= 72)
                {
                    return leadId;
                }
            }
            return "";
        }

        internal NotificationQueue Get_ReferralNotification(List<string> ReferralEvents)
        {
            //var notification = _notificationQueue.Find(x => x.status == "new" || (x.status == "faliure" && x.attemptedTrial < x.totalTrial)).FirstOrDefault();

            var filter = Builders<NotificationQueue>.Filter.In(x => x.status, new[] { "new", "failure" });
            filter = filter & Builders<NotificationQueue>.Filter.In(x => x.Event, ReferralEvents);
            var notification = _notificationQueue.Find(filter)
                      .ToEnumerable()
                  //   .Where(x => x.attemptedTrial < x.totalTrial)
                      .FirstOrDefault();
            if (notification != null)
            {
                _notificationQueue.FindOneAndUpdate(Builders<NotificationQueue>.Filter.Eq(x => x._id, notification._id),
                    Builders<NotificationQueue>.Update.Set(x => x.status, "processing"));
            }
            return notification;
        }

        internal List <NotificationQueue> Get_Notification()
        {
            var filter = Builders<NotificationQueue>.Filter.In(x => x.status, new[] { "new", "failure" }) & Builders<NotificationQueue>.Filter.Eq(x => x.Type, "Push");
            //filter = filter & Builders<NotificationQueue>.Filter.Nin(x => x.Event, ReferralEvents);
            var notification = _notificationQueue.Find(filter)
                      .ToEnumerable().ToList();
            //   .Where(x => x.attemptedTrial < x.totalTrial).ToList();

            if (notification != null)
            {
                foreach (var notify in notification)
                {
                    _notificationQueue.FindOneAndUpdate(Builders<NotificationQueue>.Filter.Eq(x => x._id, notify._id),
                Builders<NotificationQueue>.Update.Set(x => x.status, "processing"));

                }
            }
            return notification;
        }

        internal Notification Get_Notification_Template(string template_type)
        {
            return _notification.Find(x => x.Event == template_type).FirstOrDefault();
        }

        internal string Get_User_Password(string receiver, string userId)
        {
            if (receiver.ToLower().Contains("admin"))
            {
                return _adminUsers.Find(x => x._id == userId).Project(x => x.password).FirstOrDefault();
            }
            return _users.Find(x => x._id == userId).Project(x => x.password).FirstOrDefault();
        }

        internal Lead_Email_Details Get_Lead_Mailing_Details(string dealId)
        {
            var res = new Lead_Email_Details();
            var referrerUserId = _lead.Find(x => x.Id == dealId).Project(x => x.referredBy.userId).FirstOrDefault();

            res.referredById = referrerUserId;
            res.referredByName = _users.Find(x => x._id == referrerUserId).Project(x => x.firstName).FirstOrDefault();
            res.referredByemailId = _users.Find(x => x._id == referrerUserId).Project(x => x.emailId).FirstOrDefault();

            res.isForSelf = _lead.Find(x => x.Id == dealId).Project(x => x.isForSelf).FirstOrDefault();
            if (res.isForSelf)
            {
                res.referredToName = res.referredByName;
                res.referredToemailId = res.referredByemailId;
            }
            else
            {
                res.referredToName = _lead.Find(x => x.Id == dealId).Project(x => x.referredTo.name).FirstOrDefault();
                res.referredToemailId = _lead.Find(x => x.Id == dealId).Project(x => x.referredTo.emailId).FirstOrDefault();
            }

            var businessId = _lead.Find(x => x.Id == dealId).Project(x => x.referredBusinessId).FirstOrDefault();

            res.clientPartnerId = _businessDetails.Find(x => x.Id == businessId).Project(x => x.UserId).FirstOrDefault();
            res.clientPartnerName = _businessDetails.Find(x => x.Id == businessId).Project(x => x.CompanyName).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(res.clientPartnerName))
            {
                var userId = _businessDetails.Find(x => x.Id == businessId).Project(x => x.UserId).FirstOrDefault();
                res.clientPartnerName = _users.Find(x => x._id == userId).Project(x => x.firstName + " " + x.lastName).FirstOrDefault();
            }
            res.clientPartneremailId = _businessDetails.Find(x => x.Id == businessId).Project(x => x.BusinessEmail).FirstOrDefault();
            res.productServiceName = _lead.Find(x => x.Id == dealId).Project(x => x.referredProductORServices).FirstOrDefault();
            // res.productServiceName = _productsAndService.Find(x => x.bussinessId == businessId).Project(x => x.name).FirstOrDefault();
            res.dealStatus = _lead.Find(x => x.Id == dealId).Project(x => x.dealStatus.ToString()).FirstOrDefault();

            res.ujbAdminEmailId = _adminUsers.Find(x => x.Role == "Admin").Project(x => x.emailId).FirstOrDefault();

            var statusCode = _lead.Find(x => x.Id == dealId).Project(x => x.dealStatus).FirstOrDefault();
            res.referralCode = _lead.Find(x => x.Id == dealId).Project(x => x.ReferralCode).FirstOrDefault();
            var current_status = "";
            switch (statusCode)
            {
                case 1:
                    current_status = "Not Connected";
                    break;
                case 2:
                    current_status = "Called But No Response";
                    break;
                case 3:
                    current_status = "Deal Lost";
                    break;
                case 4:
                    current_status = "Discussion In Progress";
                    break;
                case 5:
                    current_status = "Deal Won";
                    break;
                case 6:
                    current_status = "Received Part Payment & Transferred to UJustBe";
                    break;
                case 7:
                    current_status = "Work In Progress";
                    break;
                case 8:
                    current_status = "Work Completed";
                    break;
                case 9:
                    current_status = "Received Full & Final Payment";
                    break;
                case 10:
                    current_status = "Agreed Percentage Transferred to UJB";
                    break;
            }
            res.currentStatus = current_status;
            return res;
        }

        internal void Update_Notification_Queue(string queueId, string message, string templateId, string status, string contactInfo, string date)
        {
            //if (status == "faliure")
            //{

            //}
            var newDate = DateTime.Parse(date);
            var currentCount = 1;// _notificationQueue.Find(x => x._id == queueId).Project(x => x.attemptedTrial).FirstOrDefault();

            currentCount += 1;
            //if (TimeZone.CurrentTimeZone.StandardName != "India Standard Time")
            //{
            var INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            var dd = DateTime.Parse(date);
            newDate = TimeZoneInfo.ConvertTimeFromUtc(dd, INDIAN_ZONE);
            //}

            _notificationQueue.FindOneAndUpdate(Builders<NotificationQueue>.Filter.Eq(x => x._id, queueId),
                Builders<NotificationQueue>.Update
                .Set(x => x.Message, message)
                .Set(x => x.notificationId, templateId)
                .Set(x => x.status, status)
                .Set(x => x.ContactInfo, contactInfo)
                .Set(x => x.dateOfNotification, newDate)
               // .Set(x => x.attemptedTrial, currentCount)
                );

            var d = _notificationQueue.Find(x => x._id == queueId).Project(x => x.dateOfNotification).FirstOrDefault();
        }

        internal string Get_Receiver_Mobile_Number(string receiver, string userId)
        {
            if (receiver.ToLower().Contains("admin"))
            {
                return _adminUsers.Find(x => x._id == userId).Project(x => x.countryCode.Substring(1) + x.mobileNumber).FirstOrDefault();
            }
            else
            {
                return _users.Find(x => x._id == userId).Project(x => x.countryCode.Substring(1) + x.mobileNumber).FirstOrDefault();
            }
        }

        internal string Get_Receiver_Email_Id(string receiver, string userId)
        {
            if (receiver.ToLower().Contains("admin"))
            {
                return _adminUsers.Find(x => x._id == userId).Project(x => x.emailId).FirstOrDefault();
            }
            else
            {
                return _users.Find(x => x._id == userId).Project(x => x.emailId).FirstOrDefault();
            }
        }

        internal string Get_Receiver_Name(string userId)
        {
            return _users.Find(x => x._id == userId).Project(x => x.firstName).FirstOrDefault();
        }

        internal string Get_User_Token(string userId)
        {
            return _users.Find(x => x._id == userId).Project(x => x.fcmToken).FirstOrDefault();
        }
    }
}
