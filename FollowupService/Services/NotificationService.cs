using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using UJBHelper.Common;
using UJBHelper.Data;
using UJBHelper.DataModel;

namespace FollowupService.Services
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
        private readonly IMongoCollection<DealStatus> _DealStatus;
        public NotificationService()
        {
            var client = new MongoClient(DbHelper.GetConnectionString());
            var database = client.GetDatabase(DbHelper.GetDatabaseName());
            _notification = database.GetCollection<Notification>("Notification");
            _notificationQueue = database.GetCollection<NotificationQueue>("NotificationQueue");
            _lead = database.GetCollection<Leads>("Leads");
            _productsAndService = database.GetCollection<DbProductService>("ProductsServices");
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
            _users = database.GetCollection<User>("Users");
            _userKycDetails = database.GetCollection<UserKYCDetails>("UserKYCDetails");
            _userOtherDetails = database.GetCollection<UserOtherDetails>("UsersOtherDetails");
            _adminUsers = database.GetCollection<AdminUser>("AdminUsers");
            _DealStatus = database.GetCollection<DealStatus>("DealStatus");
        }

        internal bool SendFollowupNotification()
        {
            var filter = Builders<DealStatus>.Filter.Empty;
            var dealstatus = _DealStatus.Find(filter).Project(x =>
                     new DealStatus { _id = x._id, StatusName = x.StatusName, StatusId = x.StatusId }).ToList();

            foreach (var deal in dealstatus)
            {
                var leadIds = _lead.Find(x => (x.dealStatus == deal.StatusId) && x.referralStatus == (int)ReferralStatusEnum.Accepted).Project(x => x.Id).ToList();//|| x.dealStatus == (int)DealStatusEnum.AgreedPercentageTransferredToUJB

                foreach (var leadId in leadIds)
                {
                    SendNotification(leadId, deal.StatusName + " FollowUp");
                }
            }
            return false;
        }

        public Task SendNotification(string LeadId, string Event)
        {
            MessageBody MB = new MessageBody();
            var nq = new Notification_Sender();
            nq.SendNotification(Event, MB, "", LeadId, "");
            return Task.CompletedTask;
        }

        internal bool Check_For_5_Days_Followup()
        {
            var leadIds = _lead.Find(x => (x.dealStatus == (int)DealStatusEnum.DealClosed || x.dealStatus == (int)DealStatusEnum.ReceivedPartPayment || x.dealStatus == (int)DealStatusEnum.WorkInProgress) && x.referralStatus == (int)ReferralStatusEnum.Accepted).Project(x => x.Id).ToList();

            foreach (var leadId in leadIds)
            {
                if (_notificationQueue.Find(x => x.leadId == leadId && (x.Event == "Deal Won FollowUp" || x.Event == "Received Part Payment & Transferred to UJustBe FollowUp" || x.Event == "Work In Progress FollowUp") && x.status != "new").CountDocuments() != 0)
                {
                    var notificationDate = _notificationQueue.Find(x => x.leadId == leadId && (x.Event == "Deal Won FollowUp" || x.Event == "Received Part Payment & Transferred to UJustBe FollowUp" || x.Event == "Work In Progress FollowUp") && x.status != "new").SortByDescending(x => x.dateOfNotification).Project(x => x.dateOfNotification).FirstOrDefault();

                    if ((DateTime.Now - notificationDate).TotalDays >= 5)
                    {
                        return true;
                    }
                }
                else
                {
                    var leadDate = _lead.Find(x => x.Id == leadId).Project(x => x.refStatusUpdatedOn).FirstOrDefault();

                    if ((DateTime.Now - leadDate).TotalDays >= 5)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal ReferralDetails Get_7_Days_Followup()
        {
            var res = new ReferralDetails();
            var leadIds = _lead.Find(x => (x.dealStatus == (int)DealStatusEnum.WorkCompleted || x.dealStatus == (int)DealStatusEnum.ReceivedFullAndFinalPayment) && x.referralStatus == (int)ReferralStatusEnum.Accepted).Project(x => x.Id).ToList();//|| x.dealStatus == (int)DealStatusEnum.AgreedPercentageTransferredToUJB

            foreach (var leadId in leadIds)
            {
                if (_notificationQueue.Find(x => x.leadId == leadId && (x.Event == "Work Completed FollowUp" || x.Event == "Received Full And Final Payment FollowUp") && x.status != "new").CountDocuments() != 0)//|| x.Event == "Agreed Percentage Transferred FollowUp"
                {
                    var notificationDate = _notificationQueue.Find(x => x.leadId == leadId && (x.Event == "Work Completed FollowUp" || x.Event == "Received Full And Final Payment FollowUp") && x.status != "new").SortByDescending(x => x.dateOfNotification).Project(x => x.dateOfNotification).FirstOrDefault();//|| x.Event == "Agreed Percentage Transferred FollowUp"

                    if ((DateTime.Now - notificationDate).TotalDays >= 7)
                    {
                        res.referralId = leadId;
                        var dealStatus = _lead.Find(x => x.Id == leadId).Project(x => x.dealStatus).FirstOrDefault();
                        res.isForSelf = _lead.Find(x => x.Id == leadId).Project(x => x.isForSelf).FirstOrDefault();
                        switch (dealStatus)
                        {
                            case 8:
                                res.referralStatus = "Work Completed FollowUp";
                                break;
                            case 9:
                                res.referralStatus = "Received Full And Final Payment FollowUp";
                                break;
                                //case 10:
                                //    res.referralStatus = "Agreed Percentage Transferred FollowUp";
                                //    break;

                        }
                    }
                }
                else
                {
                    var leadDate = _lead.Find(x => x.Id == leadId).Project(x => x.refStatusUpdatedOn).FirstOrDefault();

                    if ((DateTime.Now - leadDate).TotalDays >= 7)
                    {
                        res.referralId = leadId;
                        var dealStatus = _lead.Find(x => x.Id == leadId).Project(x => x.dealStatus).FirstOrDefault();
                        res.isForSelf = _lead.Find(x => x.Id == leadId).Project(x => x.isForSelf).FirstOrDefault();
                        switch (dealStatus)
                        {
                            case 8:
                                res.referralStatus = "Work Completed FollowUp";
                                break;
                            case 9:
                                res.referralStatus = "Received Full And Final Payment FollowUp";
                                break;
                                //case 10:
                                //    res.referralStatus = "Agreed Percentage Transferred FollowUp";
                                //    break;

                        }
                    }
                }
            }
            return res;
        }

        internal bool Check_For_7_Days_Followup()
        {
            var leadIds = _lead.Find(x => (x.dealStatus == (int)DealStatusEnum.WorkCompleted || x.dealStatus == (int)DealStatusEnum.ReceivedFullAndFinalPayment) && x.referralStatus == (int)ReferralStatusEnum.Accepted).Project(x => x.Id).ToList();//|| x.dealStatus == (int)DealStatusEnum.AgreedPercentageTransferredToUJB

            foreach (var leadId in leadIds)
            {
                if (_notificationQueue.Find(x => x.leadId == leadId && (x.Event == "Work Completed FollowUp" || x.Event == "Received Full And Final Payment FollowUp") && x.status != "new").CountDocuments() != 0)// || x.Event == "Agreed Percentage Transferred FollowUp"
                {
                    var notificationDate = _notificationQueue.Find(x => x.leadId == leadId && (x.Event == "Work Completed FollowUp" || x.Event == "Received Full And Final Payment FollowUp") && x.status != "new").SortByDescending(x => x.dateOfNotification).Project(x => x.dateOfNotification).FirstOrDefault(); //|| x.Event == "Agreed Percentage Transferred FollowUp"

                    if ((DateTime.Now - notificationDate).TotalDays >= 7)
                    {
                        return true;
                    }
                }
                else
                {
                    var leadDate = _lead.Find(x => x.Id == leadId).Project(x => x.refStatusUpdatedOn).FirstOrDefault();

                    if ((DateTime.Now - leadDate).TotalDays >= 7)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal bool Check_For_48_Hours_Followup()
        {
            var leadIds = _lead.Find(x => x.dealStatus == (int)DealStatusEnum.DiscussionInProgress && x.referralStatus == (int)ReferralStatusEnum.Accepted).Project(x => x.Id).ToList();

            foreach (var leadId in leadIds)
            {
                if (_notificationQueue.Find(x => x.leadId == leadId && x.Event == "Discussion In Progress FollowUp" && x.status != "new").CountDocuments() != 0)
                {
                    var notificationDate = _notificationQueue.Find(x => x.leadId == leadId && x.Event == "Discussion In Progress FollowUp" && x.status != "new").SortByDescending(x => x.dateOfNotification).Project(x => x.dateOfNotification).FirstOrDefault();

                    if ((DateTime.Now - notificationDate).TotalHours >= 48)
                    {
                        return true;
                    }
                }
                else
                {
                    var leadDate = _lead.Find(x => x.Id == leadId).Project(x => x.refStatusUpdatedOn).FirstOrDefault();

                    if ((DateTime.Now - leadDate).TotalHours >= 48)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal ReferralDetails Get_5_Days_Followup()
        {
            var res = new ReferralDetails();
            var leadIds = _lead.Find(x => (x.dealStatus == (int)DealStatusEnum.DealClosed || x.dealStatus == (int)DealStatusEnum.ReceivedPartPayment || x.dealStatus == (int)DealStatusEnum.WorkInProgress) && x.referralStatus == (int)ReferralStatusEnum.Accepted).Project(x => x.Id).ToList();

            foreach (var leadId in leadIds)
            {
                if (_notificationQueue.Find(x => x.leadId == leadId && (x.Event == "Deal Won FollowUp" || x.Event == "Received Part Payment & Transferred to UJustBe FollowUp" || x.Event == "Work In Progress FollowUp") && x.status != "new").CountDocuments() != 0)
                {
                    var notificationDate = _notificationQueue.Find(x => x.leadId == leadId && (x.Event == "Deal Won FollowUp" || x.Event == "Received Part Payment & Transferred to UJustBe FollowUp" || x.Event == "Work In Progress FollowUp") && x.status != "new").SortByDescending(x => x.dateOfNotification).Project(x => x.dateOfNotification).FirstOrDefault();

                    if ((DateTime.Now - notificationDate).TotalDays >= 5)
                    {
                        res.referralId = leadId;
                        var dealStatus = _lead.Find(x => x.Id == leadId).Project(x => x.dealStatus).FirstOrDefault();
                        res.isForSelf = _lead.Find(x => x.Id == leadId).Project(x => x.isForSelf).FirstOrDefault();
                        switch (dealStatus)
                        {
                            case 5:
                                res.referralStatus = "Deal Won FollowUp";
                                break;
                            case 6:
                                res.referralStatus = "Received Part Payment & Transferred to UJustBe FollowUp";
                                break;
                            case 7:
                                res.referralStatus = "Work In Progress FollowUp";
                                break;

                        }
                    }
                }
                else
                {
                    var leadDate = _lead.Find(x => x.Id == leadId).Project(x => x.refStatusUpdatedOn).FirstOrDefault();

                    if ((DateTime.Now - leadDate).TotalDays >= 5)
                    {
                        res.referralId = leadId;
                        var dealStatus = _lead.Find(x => x.Id == leadId).Project(x => x.dealStatus).FirstOrDefault();
                        res.isForSelf = _lead.Find(x => x.Id == leadId).Project(x => x.isForSelf).FirstOrDefault();
                        switch (dealStatus)
                        {
                            case 5:
                                res.referralStatus = "Deal Won FollowUp";
                                break;
                            case 6:
                                res.referralStatus = "Received Part Payment & Transferred to UJustBe FollowUp";
                                break;
                            case 7:
                                res.referralStatus = "Work In Progress FollowUp";
                                break;

                        }
                    }
                }
            }
            return res;
        }

        internal ReferralDetails Get_48_Hours_Followup()
        {
            var res = new ReferralDetails();
            var leadIds = _lead.Find(x => x.dealStatus == (int)DealStatusEnum.DiscussionInProgress && x.referralStatus == (int)ReferralStatusEnum.Accepted).Project(x => x.Id).ToList();

            foreach (var leadId in leadIds)
            {
                if (_notificationQueue.Find(x => x.leadId == leadId && x.Event == "Discussion In Progress FollowUp" && x.status != "new").CountDocuments() != 0)
                {
                    var notificationDate = _notificationQueue.Find(x => x.leadId == leadId && x.Event == "Discussion In Progress FollowUp" && x.status != "new").SortByDescending(x => x.dateOfNotification).Project(x => x.dateOfNotification).FirstOrDefault();

                    if ((DateTime.Now - notificationDate).TotalHours >= 48)
                    {
                        res.referralId = leadId;
                        var dealStatus = _lead.Find(x => x.Id == leadId).Project(x => x.dealStatus).FirstOrDefault();
                        res.isForSelf = _lead.Find(x => x.Id == leadId).Project(x => x.isForSelf).FirstOrDefault();
                        switch (dealStatus)
                        {
                            case 4:
                                res.referralStatus = "Discussion In Progress FollowUp";
                                break;

                        }
                    }
                }
                else
                {
                    var leadDate = _lead.Find(x => x.Id == leadId).Project(x => x.refStatusUpdatedOn).FirstOrDefault();

                    if ((DateTime.Now - leadDate).TotalHours >= 48)
                    {
                        res.referralId = leadId;
                        var dealStatus = _lead.Find(x => x.Id == leadId).Project(x => x.dealStatus).FirstOrDefault();
                        res.isForSelf = _lead.Find(x => x.Id == leadId).Project(x => x.isForSelf).FirstOrDefault();
                        switch (dealStatus)
                        {
                            case 4:
                                res.referralStatus = "Discussion In Progress FollowUp";
                                break;
                        }
                    }
                }
            }
            return res;
        }

        internal bool Check_For_24_Hours_Followup()
        {
            var leadIds = _lead.Find(x => (x.dealStatus == (int)DealStatusEnum.NotConnected || x.dealStatus == (int)DealStatusEnum.CalledButNoResponse) && x.referralStatus == (int)ReferralStatusEnum.Accepted).Project(x => x.Id).ToList();

            foreach (var leadId in leadIds)
            {
                if (_notificationQueue.Find(x => x.leadId == leadId && (x.Event == "Not Connected FollowUp" || x.Event == "Called But No Response FollowUp") && x.status != "new").CountDocuments() != 0)
                {
                    var notificationDate = _notificationQueue.Find(x => x.leadId == leadId && (x.Event == "Not Connected FollowUp" || x.Event == "Called But No Response FollowUp") && x.status != "new").SortByDescending(x => x.dateOfNotification).Project(x => x.dateOfNotification).FirstOrDefault();

                    if ((DateTime.Now - notificationDate).TotalHours >= 24)
                    {
                        return true;
                    }
                }
                else
                {
                    var leadDate = _lead.Find(x => x.Id == leadId).Project(x => x.refStatusUpdatedOn).FirstOrDefault();

                    if ((DateTime.Now - leadDate).TotalHours >= 24)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal ReferralDetails Get_24_Hours_Followup()
        {
            var res = new ReferralDetails();
            var leadIds = _lead.Find(x => (x.dealStatus == (int)DealStatusEnum.NotConnected || x.dealStatus == (int)DealStatusEnum.CalledButNoResponse) && x.referralStatus == (int)ReferralStatusEnum.Accepted).Project(x => x.Id).ToList();

            foreach (var leadId in leadIds)
            {
                if (_notificationQueue.Find(x => x.leadId == leadId && (x.Event == "Not Connected FollowUp" || x.Event == "Called But No Response FollowUp") && x.status != "new").CountDocuments() != 0)
                {
                    var notificationDate = _notificationQueue.Find(x => x.leadId == leadId && (x.Event == "Not Connected FollowUp" || x.Event == "Called But No Response FollowUp") && x.status != "new").SortByDescending(x => x.dateOfNotification).Project(x => x.dateOfNotification).FirstOrDefault();

                    if ((DateTime.Now - notificationDate).TotalHours >= 24)
                    {
                        res.referralId = leadId;
                        var dealStatus = _lead.Find(x => x.Id == leadId).Project(x => x.dealStatus).FirstOrDefault();
                        res.isForSelf = _lead.Find(x => x.Id == leadId).Project(x => x.isForSelf).FirstOrDefault();
                        switch (dealStatus)
                        {
                            case 1:
                                res.referralStatus = "Not Connected FollowUp";
                                break;
                            case 2:
                                res.referralStatus = "Called But No Response FollowUp";
                                break;
                        }
                    }
                }
                else
                {
                    var leadDate = _lead.Find(x => x.Id == leadId).Project(x => x.refStatusUpdatedOn).FirstOrDefault();

                    if ((DateTime.Now - leadDate).TotalHours >= 24)
                    {
                        res.referralId = leadId;
                        var dealStatus = _lead.Find(x => x.Id == leadId).Project(x => x.dealStatus).FirstOrDefault();
                        res.isForSelf = _lead.Find(x => x.Id == leadId).Project(x => x.isForSelf).FirstOrDefault();
                        switch (dealStatus)
                        {
                            case 1:
                                res.referralStatus = "Not Connected FollowUp";
                                break;
                            case 2:
                                res.referralStatus = "Called But No Response FollowUp";
                                break;
                        }
                    }
                }
            }
            return res;
        }
    }
}
