using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UJBHelper.Common;
using UJBHelper.Data;
using UJBHelper.DataModel;

namespace UJBNotification.Services
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
                    var sendNotification = SendNotification("", "Lead Auto Rejection", leadId);
                    return true;
                    //var nq = new Notification_Queue();
                    //nq.Add_To_Queue("", leadId, "", "", "new", "Lead Auto Rejection", "", "Email", "UJBAdmin", "");
                    ////nq.Add_To_Queue("", leadId, "", "", "new", "Lead Auto Rejection", "", "SMS", "UJBAdmin", "");

                    //nq.Add_To_Queue("", leadId, "", "", "new", "Lead Auto Rejection", "", "Push", "Referrer", "");
                    //nq.Add_To_Queue("", leadId, "", "", "new", "Lead Auto Rejection", "", "Email", "Referrer", "");
                    ////nq.Add_To_Queue("", leadId, "", "", "new", "Lead Auto Rejection", "", "SMS", "Referrer", "");

                    //nq.Add_To_Queue("", leadId, "", "", "new", "Lead Auto Rejection", "", "Push", "Business", "");
                    //nq.Add_To_Queue("", leadId, "", "", "new", "Lead Auto Rejection", "", "Email", "Business", "");
                    ////nq.Add_To_Queue("", leadId, "", "", "new", "Lead Auto Rejection", "", "SMS", "Business", "");
                    //return true;
                }
            }
            return false;
        }
        internal bool Check_If_KYC_Pending()
        {
            var nq = new Notification_Queue();
            // var userids = _userKycDetails.Find(x => string.IsNullOrWhiteSpace(x.AdharCard.AdharNumber) || string.IsNullOrWhiteSpace(x.PanCard.PanNumber) || string.IsNullOrWhiteSpace(x.BankDetails.AccountNumber)).Project(x => x.UserId).ToList();
            var _kycdetails = _userKycDetails.Find(x => x.IsApproved.Flag == false).ToList();
            foreach (UserKYCDetails u in _kycdetails)
            {
                if (string.IsNullOrEmpty(u.PanCard.PanNumber) || string.IsNullOrEmpty(u.PanCard.ImageURL)
                                   || string.IsNullOrEmpty(u.AdharCard.AdharNumber)
                                   || string.IsNullOrEmpty(u.AdharCard.FrontImageURL)
                                   || string.IsNullOrEmpty(u.AdharCard.BackImageURL))
                {
                    var sendNotification = SendNotification(u.UserId, "Skip KYC", "");
                    return true;
                }
            }
            return false;
        }
        internal bool Check_If_Profile_Incomplete()
        {
            var nq = new Notification_Queue();
            var userIds = _users.Find(x => x.isActive && x.Role != "Guest").Project(x => x._id).ToList();

            foreach (var userId in userIds)
            {

                var user = _users.Find(x => x._id == userId).FirstOrDefault();

                if (user.Role == "Partner")
                {
                    var userkycDetails = _userKycDetails.Find(x => x.UserId == userId).FirstOrDefault();
                    var userOtherDetails = _userOtherDetails.Find(x => x.UserId == userId).FirstOrDefault();
                    if (userkycDetails == null || userOtherDetails == null)
                    {

                        var sendNotifiaction = SendNotification(userId, "Incomplete Profile", "");
                        return true;
                    }
                    if (string.IsNullOrWhiteSpace(userOtherDetails.aboutMe) || string.IsNullOrWhiteSpace(userOtherDetails.Hobbies)
                    || string.IsNullOrWhiteSpace(userOtherDetails.areaOfInterest)
                    || string.IsNullOrWhiteSpace(userOtherDetails.maritalStatus)
                    || string.IsNullOrWhiteSpace(user.ImageURL)
                    || string.IsNullOrWhiteSpace(user.ImageURL)
                )
                    {
                        var sendNotifiaction = SendNotification(userId, "Incomplete Profile", "");
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
                        var sendNotifiaction = SendNotification(userId, "Incomplete Profile", "");

                        return true;
                    }
                }



            }
            return false;
        }
        internal bool Check_If_No_Products()
        {
            var userids = _users.Find(x => x.Role == "Listed Partner" && x.isActive).Project(x => x._id).ToList();

            foreach (var userId in userids)
            {
                var businessId = _businessDetails.Find(x => x.UserId == userId && x.isApproved.Flag == 1 && x.isSubscriptionActive).Project(x => x.Id).FirstOrDefault();
                if (businessId != null)
                {
                    if (_productsAndService.Find(x => x.bussinessId == businessId).CountDocuments() == 0)
                    {
                        var sendNotification = SendNotification(userId, "No Client Partner Products", "");

                        return true;
                    }
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

        internal bool Check_if_Referral_Below_72_Hours()
        {

            var leads = _lead.Find(x => x.referralStatus == 0).Project(x => x.Id).ToList();

            foreach (var leadId in leads)
            {
             
                var sendNotification = SendNotification("", "Auto Rejection Reminder", leadId);
              
            }

            return false;
        }
        internal void Auto_Reject_Referral(string dealId, string rejectionReason)
        {
            _lead.FindOneAndUpdate(
                Builders<Leads>.Filter.Eq(x => x.Id, dealId),
                Builders<Leads>.Update.Set(x => x.rejectionReason, rejectionReason)
                .Set(x => x.referralStatus, 2)
                );
        }

        internal bool Check_If_Guest_Reminder()
        {
            var userIds = _users.Find(x => x.Role == "Guest").Project(x => x._id).ToList();
            foreach (var userId in userIds)
            {
                var sendNotification = SendNotification(userId, "Guest Reminder To Upgrade", "");
                return true;

            }
            return false;
        }

        public Task SendNotification(string UserId, string Event, string LeadID)
        {
            MessageBody MB = new MessageBody();
            var nq = new Notification_Sender();
            nq.SendNotification(Event, MB, UserId, LeadID, "");
            return Task.CompletedTask;

        }
    }
}
