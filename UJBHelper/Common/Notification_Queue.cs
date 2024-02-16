using MongoDB.Driver;
using System;
using UJBHelper.Data;
using UJBHelper.DataModel;
using System.Linq;
using System.Reflection;

using log4net.Config;
using System.Timers;
using UJBHelper.Common;
using System.Collections.Generic;


namespace UJBHelper.Common
{
    public class Notification_Queue
    {
        private readonly IMongoCollection<NotificationQueue> _notificationQueue;
        private readonly IMongoCollection<NotificationList> _notificationList;
        private readonly IMongoCollection<System_Default> _System_Default;

        private List<NotificationQueue> notifications;
        private NotificationQueue notification;

        private Notification notify_template;
        private Lead_Email_Details email_Details = new Lead_Email_Details();

        private string template_type;

        private string email_Id;

        private string user_name;
        private string user_id = "";
        private string new_password = "";
        private string new_otp = "";
        private readonly IMongoCollection<Notification> _notification;

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
        public Notification_Queue()
        {
            var client = new MongoClient(DbHelper.GetConnectionString());
            var database = client.GetDatabase(DbHelper.GetDatabaseName());

            _notificationQueue = database.GetCollection<NotificationQueue>("NotificationQueue");
            _notificationList = database.GetCollection<NotificationList>("NotificationList");
            _System_Default = database.GetCollection<System_Default>("System_Default");
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

        public Notification_Queue(string connection, string dbName)
        {
            //var client = new MongoClient(connection);
            //var database = client.GetDatabase(dbName);

            var client = new MongoClient(DbHelper.GetConnectionString());
            var database = client.GetDatabase(DbHelper.GetDatabaseName());
            _notificationQueue = database.GetCollection<NotificationQueue>("NotificationQueue");
            _notificationList = database.GetCollection<NotificationList>("NotificationList");
            _System_Default = database.GetCollection<System_Default>("System_Default");
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

        public Notification Get_Notification_Template(string template_type)
        {
            return _notification.Find(x => x.Event == template_type).FirstOrDefault();
        }
        public string Add_To_Queue(string userId, string leadId, string notificationId, string date, string status, string eventName, string contactInfo, string type, string receiver, string message, string _messagebody)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });
            var nq = new NotificationQueue
            {
                userId = userId,
                leadId = leadId,
                notificationId = notificationId,
                dateOfNotification = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)),
                status = status,
                Event = eventName,
                ContactInfo = contactInfo,
                Type = type,
                Receiver = receiver,
                Message = message,
                MessageBody = _messagebody,
                //attemptedTrial = 0,
                //totalTrial = 1,
                Created = new Created { created_By = userId, created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)) },
            };

            _notificationQueue.InsertOne(nq);
            if (receiver != "UJBAdmin" && receiver != "Referred")
            {
                Message_To_Add_Notification_List(eventName, receiver, userId, leadId, type);
            }
            return nq._id;

        }

        public void Add_To_Queue(string userId, string leadId, string notificationId, string date, string status, string eventName, string contactInfo, string type, string receiver, string message)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });
            var nq = new NotificationQueue
            {
                userId = userId,
                leadId = leadId,
                notificationId = notificationId,
                //dateOfNotification = DateTime.Parse(date),
                status = status,
                Event = eventName,
                ContactInfo = contactInfo,
                Type = type,
                Receiver = receiver,
                Message = message,

                //attemptedTrial = 0,
                //totalTrial = 1,
                //Created = new Created { created_By = userId, created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)) },
            };

            _notificationQueue.InsertOne(nq);
            if (receiver != "UJBAdmin" && receiver != "Referred")
                Message_To_Add_Notification_List(eventName, receiver, userId, leadId, type);

        }

        internal long Get_Notification_List_cnt(string template_type, string userId, string LeadId)
        {

            return _notificationList.Find(x => x.type == template_type && (x.userId == userId || x.leadId == LeadId)).CountDocuments();
        }

        public void Message_To_Add_Notification_List(string eventName, string receiver, string user_id, string LeadId, string Type)
        {
            try
            {// && Get_Notification_List_cnt(eventName,user_id,LeadId)<1
                if (NotifyList.NotificationListHolder.Contains(eventName) && Type == "Email")
                {

                    notify_template = Get_Notification_Template(eventName);

                    var notificationData = notify_template.Data.Where(x => x.Receiver == receiver).FirstOrDefault();
                    bool isReferredByme = false;

                    switch (eventName)
                    {
                        case "Lead Created":
                        case "Lead Acceptance":
                        case "Lead Rejection":
                        case "Lead Status Changed":
                        case "Lead Auto Rejection":
                        case "Auto Rejection Reminder":
                        case "Not Connected FollowUp":
                        case "Called But No Response FollowUp":
                        case "Discussion In Progress FollowUp":
                        case "Deal Won FollowUp":
                        case "Received Part Payment & Transferred to UJustBe FollowUp":
                        case "Work In Progress FollowUp":
                        case "Work Completed FollowUp":
                        case "Received Full And Final Payment FollowUp":
                        case "Agreed Percentage Transferred FollowUp":
                            email_Details = Get_Lead_Mailing_Details(LeadId);
                            switch (receiver)
                            {

                                case "Referrer":
                                    user_id = email_Details.referredById;

                                    isReferredByme = true;
                                    break;
                                case "Referred":
                                    user_id = email_Details.referredById;

                                    break;
                                case "Business":
                                    user_id = email_Details.clientPartnerId;
                                    break;
                            }
                            break;
                        case "Registeration":
                            user_name = Get_Receiver_Name(user_id);
                            new_password = SecurePasswordHasherHelper.Decrypt(Get_User_Password(receiver, user_id));

                            break;
                        case "Forgot Password":
                        case "Change Password":
                            user_name = Get_Receiver_Name(user_id);
                            new_password = SecurePasswordHasherHelper.Decrypt(Get_User_Password(receiver, user_id));
                            break;
                        case "KYC Approval Under Process":
                        case "OTP Verification":
                        case "Approve Partner":
                        case "Reject Partner":
                        case "Partner Profile Updated":
                        case "Partner Business Profile Updated":
                        case "Guest Reminder To Upgrade":
                            user_name = Get_Receiver_Name(user_id);
                            new_otp = Get_User_OTP(user_id);

                            break;
                        case "Email Changed":
                        case "Mobile Number Changed":
                        case "Skip KYC":
                            user_name = Get_Receiver_Name(user_id);

                            break;
                        case "Incomplete Profile":
                        case "No Client Partner Products":
                            user_name = Get_Receiver_Name(user_id);

                            break;

                    }


                    var message = notificationData.Push.MessageBody
                  .Replace("@user", user_name)
                  .Replace("@new_password", new_password)
                  .Replace("@business", email_Details.clientPartnerName)
                    .Replace("@productservice", email_Details.productServiceName)
                    .Replace("@referredperson", email_Details.referredToName)
                    .Replace("@referrer", email_Details.referredByName)
                    .Replace("@status", email_Details.currentStatus)
                    .Replace("@ref_code", email_Details.referralCode);


                    if (user_id != "" && user_id != null)
                    { Add_To_Notification_List(user_id, DateTime.Now, message, eventName, false, notify_template.isSystemGenerated, LeadId, isReferredByme); }



                }



            }
            catch (Exception ex)
            {

                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }
        internal string Get_User_OTP(string userId)
        {
            return _users.Find(x => x._id == userId).Project(x => x.otpVerification.OTP).FirstOrDefault();
        }
        internal string Get_Receiver_Name(string userId)
        {
            return _users.Find(x => x._id == userId).Project(x => x.firstName).FirstOrDefault();
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
            var categories = _businessDetails.Find(x => x.Id == businessId).Project(x => x.Categories).FirstOrDefault();

            //  categories = _categories.Find(x => catids.Contains(x.Id)).Project(x => new Category_Info
            //{
            //    id = x.Id,
            //    name = x.categoryName
            //}).ToList();
            res.clientPartnerId = _businessDetails.Find(x => x.Id == businessId).Project(x => x.UserId).FirstOrDefault();
            res.clientPartnerName = _businessDetails.Find(x => x.Id == businessId).Project(x => x.CompanyName).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(res.clientPartnerName))
            {
                var userId = _businessDetails.Find(x => x.Id == businessId).Project(x => x.UserId).FirstOrDefault();
                res.clientPartnerName = _users.Find(x => x._id == userId).Project(x => x.firstName + " " + x.lastName).FirstOrDefault();
            }
            res.clientPartneremailId = _businessDetails.Find(x => x.Id == businessId).Project(x => x.BusinessEmail).FirstOrDefault();
            res.productServiceName = _lead.Find(x => x.Id == dealId).Project(x => x.referredProductORServices).FirstOrDefault();
            //res.productServiceName = _productsAndService.Find(x => x.bussinessId == businessId).Project(x => x.name).FirstOrDefault();
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




        public void Add_To_Notification_List(string userId, DateTime date, string message, string type, bool isRead, bool isSystemGenerated, string leadId, bool isReferredByMe)
        {
            var ns = new NotificationList
            {
                userId = userId,
                date = date,
                messageText = message,
                isRead = isRead,
                type = type,
                isSystemGenerated = isSystemGenerated,
                leadId = leadId,
                isReferredByMe = isReferredByMe

            };

            _notificationList.InsertOne(ns);

        }

        public string checkSMSSendFlag()
        {
            var filter = Builders<System_Default>.Filter.Eq(x => x.Default_Name, "SMS");
            return _System_Default.Find(filter).FirstOrDefault().Default_Value;

        }

        public System_Default checkSMSSendDetails()
        {
            var filter = Builders<System_Default>.Filter.Eq(x => x.Default_Name, "SMS");
            //  return _System_Default.Find(filter).FirstOrDefault();
            var res = new System_Default();
            res = _System_Default.Find(filter).Project(x => new System_Default
            {
                Default_Name = x.Default_Name,
                Default_Value = x.Default_Value,
                userName = x.userName,
                senderId = x.senderId,
                password = x.password
            }).FirstOrDefault();
            return res;
        }

        public System_Default checkEmailSendDetails()
        {
            var filter = Builders<System_Default>.Filter.Eq(x => x.Default_Name, "EMail");
            // return _System_Default.Find(filter).FirstOrDefault();

            var res = new System_Default();
            res = _System_Default.Find(filter).Project(x => new System_Default
            {
                Default_Name = x.Default_Name,
                Default_Value = x.Default_Value,
                userName = x.userName,
                fromEmailID = x.fromEmailID,
                password = x.password,
                host = x.host,
                port = x.port


            }).FirstOrDefault();
            return res;

        }
    }

    public class Lead_Email_Details
    {
        public string ujbAdminEmailId { get; set; }

        public string referredById { get; set; }
        public string referredByName { get; set; }
        public string referredByemailId { get; set; }

        public string referredToName { get; set; }
        public string referredToemailId { get; set; }

        public string clientPartnerId { get; set; }
        public string clientPartnerName { get; set; }
        public string clientPartneremailId { get; set; }

        public string productServiceName { get; set; }
        public string dealStatus { get; set; }
        public string currentStatus { get; set; }
        public string referralCode { get; set; }

        public bool isForSelf { get; set; }
    }

}
