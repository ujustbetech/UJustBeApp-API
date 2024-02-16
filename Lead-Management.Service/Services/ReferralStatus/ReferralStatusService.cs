using Lead_Management.Service.Manager.Referral;
using Lead_Management.Service.Models.ReferralStatus;
using Lead_Management.Service.Repositories.ReferralStatus;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using UJBHelper.DataModel;

namespace Lead_Management.Service.Services.ReferralStatus
{
    public class ReferralStatusService : IReferralStatusService
    {
        private readonly IMongoCollection<Categories> _categories;
        private readonly IMongoCollection<Leads> _lead;
        private readonly IMongoCollection<LeadsStatusHistory> _leadStatusHistory;
        private readonly IMongoCollection<DbProductService> _productsAndService;
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<AdminUser> _adminUsers;
        private readonly IMongoCollection<DealDependentStatus> _dealStatus;
        private readonly IMongoCollection<DealStatus> _dStatus;
        private IConfiguration _iconfiguration;
        public ReferralStatusService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _categories = database.GetCollection<Categories>("Categories");
            _lead = database.GetCollection<Leads>("Leads");
            _leadStatusHistory = database.GetCollection<LeadsStatusHistory>("LeadsStatusHistory");
            _productsAndService = database.GetCollection<DbProductService>("ProductsServices");
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
            _users = database.GetCollection<User>("Users");
            _adminUsers = database.GetCollection<AdminUser>("AdminUsers");
            _dealStatus = database.GetCollection<DealDependentStatus>("DealDependentStatus");
            _dStatus = database.GetCollection<DealStatus>("DealStatus");

        }

        public bool Check_If_Status_Exist(int Id)
        {
            return _dealStatus.Find(x => x.StatusId == Id).CountDocuments() != 0;
        }

        public Get_Request Get_Dependent_Status(int StatusId)
        {
            var res = new Get_Request();

            var filter1 = Builders<DealStatus>.Filter.Eq(x => x.StatusId, StatusId);
            DealStatus stats = new DealStatus();
            stats = _dStatus.Find(filter1).FirstOrDefault();

            var filter = Builders<DealDependentStatus>.Filter.Eq(x => x.StatusId, StatusId);
            List<DependentStatus> status = new List<DependentStatus>();
            status = _dealStatus.Find(filter).FirstOrDefault().DependentStatus;
            res.StatusList = status;
            res.StatusList.Add(new DependentStatus { StatusId = stats.StatusId, StatusName = stats.StatusName });
            return res;
        }

        public Get_Request Get_Dependent_Status_Details(int StatusId)
        {
            var res = new Get_Request();

            //var filter1 = Builders<DealStatus>.Filter.Eq(x => x.StatusId, StatusId);
            //DealStatus stats = new DealStatus();
            //stats = _dStatus.Find(filter1).FirstOrDefault();

            var filter = Builders<DealDependentStatus>.Filter.Eq(x => x.StatusId, StatusId);
            List<DependentStatus> status = new List<DependentStatus>();
            status = _dealStatus.Find(filter).FirstOrDefault().DependentStatus;
            res.StatusList = status;
            // res.StatusList.Add(new DependentStatus { StatusId = stats.StatusId, StatusName = stats.StatusName });
            return res;
        }

        public Email_Details Get_Referrer_Email_Id(string dealId)
        {
            var res = new Email_Details();
            var referrerUserId = _lead.Find(x => x.Id == dealId).Project(x => x.referredBy.userId).FirstOrDefault();

            res.referredByName = _users.Find(x => x._id == referrerUserId).Project(x => x.firstName).FirstOrDefault();
            res.referredByemailId = _users.Find(x => x._id == referrerUserId).Project(x => x.emailId).FirstOrDefault();

            res.referredToName = _lead.Find(x => x.Id == dealId).Project(x => x.referredTo.name).FirstOrDefault();//_users.Find(x => x._id == referrerUserId).Project(x => x.firstName).FirstOrDefault();
            res.referredToemailId = _lead.Find(x => x.Id == dealId).Project(x => x.referredTo.emailId).FirstOrDefault();

            var businessId = _lead.Find(x => x.Id == dealId).Project(x => x.referredBusinessId).FirstOrDefault();

            res.clientPartnerName = _businessDetails.Find(x => x.Id == businessId).Project(x => x.CompanyName).FirstOrDefault();
            res.clientPartneremailId = _businessDetails.Find(x => x.Id == businessId).Project(x => x.BusinessEmail).FirstOrDefault();

            res.productServiceName = _productsAndService.Find(x => x.bussinessId == businessId).Project(x => x.name).FirstOrDefault();
            res.dealStatus = _lead.Find(x => x.Id == dealId).Project(x => x.referralStatus.ToString()).FirstOrDefault();

            res.ujbAdminEmailId = _adminUsers.Find(x => x.Role == "UJB Admin").Project(x => x.emailId).FirstOrDefault();

            return res;
        }

        public void Update_Referral_Status(Put_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });
            _lead.FindOneAndUpdate(
                Builders<Leads>.Filter.Eq(x => x.Id, request.leadId),
                Builders<Leads>.Update.Set(x => x.dealStatus, request.statusId)
                .Set(x => x.dealStatusUpdatedOn, DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)))

                );


            var lsh = new LeadsStatusHistory
            {
                leadId = request.leadId,
                statusId = request.statusId,
                Updated = new Updated
                {
                    updated_By = request.updatedBy,
                    updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                }
            };

            _leadStatusHistory.InsertOne(lsh);
        }


        public bool Is_Active_Users(string LeadId)
        {
            var refferal = _lead.Find(x => x.Id == LeadId).Project(x => x.referredBy).FirstOrDefault();
            var bussinessid = _lead.Find(x => x.Id == LeadId).Project(x => x.referredBusinessId).FirstOrDefault();
            var refactive = _users.Find(x => !x.isActive && x._id == refferal.userId).CountDocuments();
            string LPactiveId = _businessDetails.Find(x => x.Id == bussinessid).Project(x => x.UserId).FirstOrDefault();
            var LPactive = _users.Find(x => !x.isActive && x._id == LPactiveId).CountDocuments();
            if (LPactive > 0)
            {
                return false;
            }
            return true;
        }
    }
}
