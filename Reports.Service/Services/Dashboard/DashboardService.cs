using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Reports.Service.Models.Dashboard;
using Reports.Service.Repositories.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using UJBHelper.DataModel;

namespace Reports.Service.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        //private readonly IMongoCollection<LeadsStatusDetails> _leadStatusDetails;
        private readonly IMongoCollection<Leads> _leads;
        private readonly IMongoCollection<User> _users;
        private IConfiguration _iconfiguration;
        private readonly IMongoCollection<PaymentDetails> _payment;
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        public DashboardService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _users = database.GetCollection<User>("Users");
            //_leadStatusDetails = database.GetCollection<LeadsStatusDetails>("LeadsStatusDetails");
            _leads = database.GetCollection<Leads>("Leads");
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
            _payment = database.GetCollection<PaymentDetails>("PaymentDetails");
        }

        public bool Check_If_User_Exists(string userId)
        {
            return _users.Find(x => x._id == userId).CountDocuments() > 0;
        }

        public string Get_Amount_Earned_By_UJB()
        {
            double total = 0;
            var data = _leads.Find(x => x.dealValue > 0 && x.shareReceivedByUJB != null).ToList();

            foreach (var d in data)
            {
                switch (d.shareReceivedByUJB.percOrAmount)
                {
                    case 1:
                        var val = (d.dealValue / 100) * d.shareReceivedByUJB.value;

                        var actualvalue = (val / 100) * 20;

                        total += actualvalue;
                        break;
                    case 2:
                        var actualval = (d.shareReceivedByUJB.value / 100) * 20;
                        total += actualval;
                        break;
                }
            }
            //return FormatNumber(total);
            return Math.Round(total).ToString();
        }

        public Post_Request Get_Client_Partner_Stats(string userId)
        {
            var res = new Post_Request();
            var myMentorCode = _users.Find(x => x._id == userId).Project(x => x.myMentorCode).FirstOrDefault();
            res.dealsClosed = _leads.Find(x => x.referredBy.userId == userId && x.dealStatus >= (int)DealStatusEnum.DealClosed).CountDocuments().ToString();

            var leadListforActiveIncome = _leads.Find(x => x.referredBy.userId == userId && x.dealStatus >= (int)DealStatusEnum.DealClosed).ToList();

            double total = 0; double RefTotal = 0;
            foreach (var lead in leadListforActiveIncome)
            {
                //List<ShareRecievedByPartners> shareRecieved = _payment.Find(x => x.PayType == 1 && x.leadId == lead.Id).Project(x => x.ShareRecvdByPartners).ToList();
                //if (shareRecieved.Count() > 0)
                //{
                //    //double SharedAmt = shareRecieved.Where(x => x.partnerID == userId).Sum(x => x.RecievedByReferral);
                //    //total = total + SharedAmt;
                //    //RefTotal = RefTotal + SharedAmt;
                //    double SharedAmt = Double.Parse(_payment.Find(x => x.PayType == 2 && x.leadId == lead.Id && x.paymentTo.Contains(userId)).Project(x => x.Amount).FirstOrDefault().ToString());
                //    //double amtRecvdFrmPramotion = Double.Parse(_payment.Find(x => x.PayType == 2 && x.leadId == lead.Id && x.paymentTo.Contains(userId)).Project(x => x.amtRecvdFrmPramotion).FirstOrDefault().ToString());
                //    //if (shareRecieved.Count() > 0)
                //    //{
                //    //    double SharedAmt = shareRecieved.Where(x => x.mentorID == userId).Sum(x => x.RecievedByMentor);

                //    total = total + SharedAmt;
                //    RefTotal = RefTotal + SharedAmt;

                //    res.refsEarned.activeIncome =Math.Round(total,2).ToString(); // FormatNumber(total);
                //}      

                var shareRecieved = _payment.Find(x => x.PayType == 2 && x.leadId == lead.Id && x.paymentTo.Contains(userId)).Project(x => x.Amount).ToList();

                foreach (var share in shareRecieved)
                {
                    double SharedAmt = share;
                    total = total + SharedAmt;
                    RefTotal = RefTotal + SharedAmt;
                    res.refsEarned.activeIncome = total.ToString();
                }
            }

            if (string.IsNullOrEmpty(res.refsEarned.activeIncome))
            {
                res.refsEarned.activeIncome = "0";
            }

            else
            {
                res.refsEarned.activeIncome = Math.Round(total).ToString();
            }
            //pending
            var passiveUserList = _users.Find(x => x.mentorCode == myMentorCode).Project(x => x._id).ToList();

            var passiveleadIds = _leads.Find(x => passiveUserList.Contains(x.referredBy.userId) && x.dealStatus == (int)DealStatusEnum.DealClosed).Project(x => x.Id).ToList();

            var passiveBuleadIds = _businessDetails.Find(x => passiveUserList.Contains(x.UserId)).Project(x => x.Id).ToList();

            var passiveBleadIds = _leads.Find(x => passiveBuleadIds.Contains(x.referredBusinessId)).ToList();

            var leadListforPassiveIncome = _leads.Find(x => passiveUserList.Contains(x.referredBy.userId) && x.dealStatus >= (int)DealStatusEnum.DealClosed).ToList();

            total = 0;
            foreach (var lead in leadListforPassiveIncome)
            {
                //List<ShareRecievedByPartners> shareRecieved = _payment.Find(x => x.PayType == 1 && x.leadId == lead.Id).Project(x => x.ShareRecvdByPartners).ToList();
                //if (shareRecieved.Count() > 0)
                //{
                //    //double SharedAmt = shareRecieved.Where(x => x.mentorID == userId).Sum(x => x.RecievedByMentor);
                //    //double SharedLPAmt = shareRecieved.Where(x => x.LPmentorID == userId).Sum(x => x.RecievedByLPMentor);
                //    //total = total + SharedAmt + SharedLPAmt;
                //    //RefTotal = RefTotal + SharedAmt + SharedLPAmt;
                //    //res.refsEarned.passiveIncome = Math.Round(total,2).ToString();// FormatNumber(total);                    

                //    double SharedAmt = Double.Parse(_payment.Find(x => x.PayType == 2 && x.leadId == lead.Id && x.paymentTo.Contains(userId)).Project(x => x.Amount).FirstOrDefault().ToString());
                //    double amtRecvdFrmPramotion = Double.Parse(_payment.Find(x => x.PayType == 2 && x.leadId == lead.Id && x.paymentTo.Contains(userId)).Project(x => x.amtRecvdFrmPramotion).FirstOrDefault().ToString());
                //    //if (shareRecieved.Count() > 0)
                //    //{
                //    //    double SharedAmt = shareRecieved.Where(x => x.mentorID == userId).Sum(x => x.RecievedByMentor);

                //    total = total + SharedAmt + amtRecvdFrmPramotion;
                //    RefTotal = RefTotal + SharedAmt + amtRecvdFrmPramotion;
                //    res.refsEarned.passiveIncome = total.ToString(); // FormatNumber(total);   
                //}    
                var shareRecieved = _payment.Find(x => x.PayType == 2 && x.leadId == lead.Id && x.paymentTo.Contains(userId)).Project(x => x.Amount).ToList();

                foreach (var share in shareRecieved)
                {
                    double SharedAmt = share;
                    total = total + SharedAmt;
                    RefTotal = RefTotal + SharedAmt;
                    res.refsEarned.passiveIncome = total.ToString();
                }

                var amtRecvdPramotion = _payment.Find(x => x.PayType == 2 && x.leadId == lead.Id && x.paymentTo.Contains(userId)).Project(x => x.amtRecvdFrmPramotion).ToList();
                //if (shareRecieved.Count() > 0)
                //{
                //    double SharedAmt = shareRecieved.Where(x => x.mentorID == userId).Sum(x => x.RecievedByMentor);
                foreach (var share in amtRecvdPramotion)
                {
                    double amtRecvdFrmPramotion = share;
                    total = total + amtRecvdFrmPramotion;
                    RefTotal = RefTotal + amtRecvdFrmPramotion;
                    res.refsEarned.passiveIncome = total.ToString();
                }
            }
            //foreach (var lead in passiveBleadIds)
            //{
            //    List<ShareRecievedByPartners> shareRecieved = _payment.Find(x => x.PayType == 1 && x.leadId == lead.Id).Project(x => x.ShareRecvdByPartners).ToList();
            //    if (shareRecieved.Count() > 0)
            //    {
            //        //double SharedAmt = shareRecieved.Where(x => x.mentorID == userId).Sum(x => x.RecievedByMentor);
            //        double SharedLPAmt = shareRecieved.Where(x => x.LPmentorID == userId).Sum(x => x.RecievedByLPMentor);
            //        total = total  + SharedLPAmt;
            //        RefTotal = RefTotal  + SharedLPAmt;
            //        res.refsEarned.passiveIncome = res.refsEarned.passiveIncome+ Math.Round(total, 2).ToString();// FormatNumber(total);                    
            //    }
            //}
            if (string.IsNullOrEmpty(res.refsEarned.passiveIncome))
            {
                res.refsEarned.passiveIncome = "0";
            }
            else
            {
                res.refsEarned.passiveIncome = Math.Round(total).ToString();
            }

            res.refsGiven = _leads.Find(x => x.referredBy.userId == userId).CountDocuments().ToString();
            var bussinessid = _businessDetails.Find(x => x.UserId == userId).Project(x => x.Id).FirstOrDefault();
            res.businessStats.totalDealsClosed = _leads.Find(x => x.referredBusinessId == bussinessid && x.dealStatus >= (int)DealStatusEnum.DealClosed).CountDocuments().ToString();
            //check
            // res.businessStats.totalBusinessClosed = FormatNumber(_leads.Find(x => x.referredBusinessId == bussinessid && x.dealStatus >= (int)DealStatusEnum.DealClosed).Project(x => x.dealValue).ToList().Sum()); //.ToString();
            res.businessStats.totalBusinessClosed = Get_Total_Business_Closed_User(userId);
            if (RefTotal != 0)
            {
                res.refsEarnedTotal = Math.Round(RefTotal).ToString(); // FormatNumber(RefTotal);
            }
            else
            {
                res.refsEarnedTotal = "0";
            }
            return res;
        }

        public Post_Request Get_Partner_Stats(string userId)
        {
            var res = new Post_Request();

            var myMentorCode = _users.Find(x => x._id == userId).Project(x => x.myMentorCode).FirstOrDefault();

            var leadIds = _leads.Find(x => x.referredBy.userId == userId && x.dealStatus == (int)DealStatusEnum.DealClosed).Project(x => x.Id).ToList();

            res.dealsClosed = _leads.Find(x => x.referredBy.userId == userId && x.dealStatus >= (int)DealStatusEnum.DealClosed).CountDocuments().ToString();

            var leadListforActiveIncome = _leads.Find(x => x.referredBy.userId == userId && x.dealStatus >= (int)DealStatusEnum.DealClosed).ToList();

            double total = 0; double RefTotal = 0;
            foreach (var lead in leadListforActiveIncome)
            {
                //List<ShareRecievedByPartners> shareRecieved = _payment.Find(x => x.PayType == 1 && x.leadId == lead.Id).Project(x => x.ShareRecvdByPartners).ToList();
                //if (shareRecieved.Count() > 0)
                //{
                //    double SharedAmt = shareRecieved.Where(x => x.partnerID == userId).Sum(x => x.RecievedByReferral);
                //    total = total + SharedAmt;
                //    RefTotal = RefTotal + SharedAmt;
                //    //pending
                //    res.refsEarned.activeIncome = Math.Round(total,2).ToString(); // FormatNumber(total);

                //}     

                var shareRecieved = _payment.Find(x => x.PayType == 2 && x.leadId == lead.Id && x.paymentTo.Contains(userId)).Project(x => x.Amount).ToList();

                foreach (var share in shareRecieved)
                {
                    double SharedAmt = share;
                    total = total + SharedAmt;
                    RefTotal = RefTotal + SharedAmt;
                    res.refsEarned.activeIncome = total.ToString();
                }
                // double amtRecvdFrmPramotion = Double.Parse(_payment.Find(x => x.PayType == 2 && x.leadId == lead.Id && x.paymentTo.Contains(userId)).Project(x => x.amtRecvdFrmPramotion).FirstOrDefault().ToString());
                //if (shareRecieved.Count() > 0)
                //{
                //    double SharedAmt = shareRecieved.Where(x => x.mentorID == userId).Sum(x => x.RecievedByMentor);

                //    total = total + SharedAmt ;
                //RefTotal = RefTotal + SharedAmt ;
                //res.refsEarned.activeIncome = total.ToString(); // FormatNumber(total);
            }

            if (string.IsNullOrEmpty(res.refsEarned.activeIncome))
            {
                res.refsEarned.activeIncome = "0";
            }
            else
            {
                res.refsEarned.activeIncome = Math.Round(total).ToString();
            }

            //pending
            var passiveUserList = _users.Find(x => x.mentorCode == myMentorCode).Project(x => x._id).ToList();

            var passiveleadIds = _leads.Find(x => passiveUserList.Contains(x.referredBy.userId) && x.dealStatus == (int)DealStatusEnum.DealClosed).Project(x => x.Id).ToList();

            var leadListforPassiveIncome = _leads.Find(x => passiveUserList.Contains(x.referredBy.userId) && x.dealStatus >= (int)DealStatusEnum.DealClosed).ToList();
            var passiveBuleadIds = _businessDetails.Find(x => passiveUserList.Contains(x.UserId)).Project(x => x.Id).ToList();

            var passiveBleadIds = _leads.Find(x => passiveBuleadIds.Contains(x.referredBusinessId)).ToList();
            total = 0;
            foreach (var lead in leadListforPassiveIncome)
            {
                //List<ShareRecievedByPartners> shareRecieved = _payment.Find(x => x.PayType == 1 && x.leadId == lead.Id).Project(x => x.ShareRecvdByPartners).ToList();
                //if (shareRecieved.Count() > 0)
                //{
                //    double SharedAmt = shareRecieved.Where(x => x.mentorID == userId).Sum(x => x.RecievedByMentor);

                //    total = total + SharedAmt;
                //    RefTotal = RefTotal + SharedAmt;
                //    res.refsEarned.passiveIncome = total.ToString(); // FormatNumber(total);                    
                //}

                // double SharedAmt =Double.Parse( _payment.Find(x => x.PayType == 2 &&  x.leadId == lead.Id && x.paymentTo.Contains(userId)).Project(x => x.Amount).FirstOrDefault().ToString());

                var shareRecieved = _payment.Find(x => x.PayType == 2 && x.leadId == lead.Id && x.paymentTo.Contains(userId)).Project(x => x.Amount).ToList();

                foreach (var share in shareRecieved)
                {
                    double SharedAmt = share;
                    total = total + SharedAmt;
                    RefTotal = RefTotal + SharedAmt;
                    res.refsEarned.passiveIncome = total.ToString();
                }

                var amtRecvdPramotion = _payment.Find(x => x.PayType == 2 && x.leadId == lead.Id && x.paymentTo.Contains(userId)).Project(x => x.amtRecvdFrmPramotion).ToList();
                //if (shareRecieved.Count() > 0)
                //{
                //    double SharedAmt = shareRecieved.Where(x => x.mentorID == userId).Sum(x => x.RecievedByMentor);
                foreach (var share in amtRecvdPramotion)
                {
                    double amtRecvdFrmPramotion = share;
                    total = total + amtRecvdFrmPramotion;
                    RefTotal = RefTotal + amtRecvdFrmPramotion;
                    res.refsEarned.passiveIncome = total.ToString();
                }
                //total = total + SharedAmt + amtRecvdFrmPramotion;
                //    RefTotal = RefTotal + SharedAmt + amtRecvdFrmPramotion;
                //    res.refsEarned.passiveIncome = total.ToString(); // FormatNumber(total);                    
                // }
            }
            if (string.IsNullOrEmpty(res.refsEarned.passiveIncome))
            {
                res.refsEarned.passiveIncome = "0";
            }
            else
            {
                res.refsEarned.passiveIncome = Math.Round(total).ToString();
            }
            //foreach (var lead in passiveBleadIds)
            //{
            //    List<ShareRecievedByPartners> shareRecieved = _payment.Find(x => x.PayType == 1 && x.leadId == lead.Id).Project(x => x.ShareRecvdByPartners).ToList();
            //    if (shareRecieved.Count() > 0)
            //    {
            //        //double SharedAmt = shareRecieved.Where(x => x.mentorID == userId).Sum(x => x.RecievedByMentor);
            //        double SharedLPAmt = shareRecieved.Where(x => x.LPmentorID == userId).Sum(x => x.RecievedByLPMentor);
            //        total = total + SharedLPAmt;
            //        RefTotal = RefTotal + SharedLPAmt;
            //        res.refsEarned.passiveIncome = res.refsEarned.passiveIncome + Math.Round(total, 2).ToString();// FormatNumber(total);                    
            //    }
            //}
            //if (string.IsNullOrEmpty(res.refsEarned.passiveIncome))
            //{
            //    res.refsEarned.passiveIncome = "0";
            //}

            res.refsGiven = _leads.Find(x => x.referredBy.userId == userId).CountDocuments().ToString();
            if (RefTotal != 0)
            {
                res.refsEarnedTotal = Math.Round(RefTotal).ToString(); // FormatNumber(RefTotal);
            }
            else
            {
                res.refsEarnedTotal = "0";
            }

            // ref given ,, referred by id 
            // deal closed == ref given total
            //res.businessStats.totalDealsClosed = _leadStatusDetails.Find(x => leadIds.Contains(x.leadId)).CountDocuments().ToString();
            //res.businessStats.totalBusinessClosed = _leadStatusDetails.Find(x => leadIds.Contains(x.leadId)).Project(x => x.dealValue).ToList().Sum().ToString();

            return res;
        }

        public string Get_Total_Business_Closed()
        {
            //var AmtPaid = _payment.AsQueryable()
            //             .Where(x => x.PayType==1)
            //             .GroupBy(d => d.PayType)
            //             .Select(
            //              g => new
            //              {
            //                  Value = g.Sum(s => s.CPReceivedAmt),
            //              }).FirstOrDefault();

            var data = _leads.Find(x => x.dealValue > 0 && x.dealStatus > 4).Project(x=>x.dealValue).ToList();
            var AmtPaid = data.AsQueryable().Sum();

            if (AmtPaid != null)
            {
                return Math.Round((AmtPaid)).ToString(); // FormatNumber(AmtPaid.Value).ToString();
            }
            else
            {
                return "0";
            }
            //return _leads.Find(x => x.dealStatus == (int)DealStatusEnum.DealClosed).CountDocuments().ToString();
        }

        public string Get_Total_Business_Closed_User(string UserId)
        {
            var AmtPaid = _payment.AsQueryable()
                         .Where(x => x.PayType == 1 && x.paymentFrom == UserId)
                         .GroupBy(d => d.PayType)
                         .Select(
                          g => new
                          {
                              Value = g.Sum(s => s.CPReceivedAmt),
                          }).FirstOrDefault();
            if (AmtPaid != null)
            {
                return Math.Round((AmtPaid.Value)).ToString(); //FormatNumber(AmtPaid.Value).ToString();
            }
            else
            {
                return "0";
            }
            //return _leads.Find(x => x.dealStatus == (int)DealStatusEnum.DealClosed).CountDocuments().ToString();
        }

        public string Get_Total_Guests_Count()
        {
            return _users.Find(x => x.Role == "Guest").CountDocuments().ToString();
        }

        public string Get_Total_Client_Partners()
        {
            string Count = "0";
            if (_users.Find(x => x.Role.Equals("Listed Partner") && x.isMembershipAgreementAccepted == true).CountDocuments() > 0)
            {
                var UserIds = _users.Find(x => x.Role.Equals("Listed Partner") && x.isMembershipAgreementAccepted == true).Project(x => x._id).ToList();
                Count = _businessDetails.Find(x => UserIds.Contains(x.UserId) && x.isSubscriptionActive == true && x.isApproved.Flag == 1).CountDocuments().ToString();

            }
            return Count;
            //return _users.Find(x => x.Role.Equals("Listed Partner")).CountDocuments().ToString();
        }

        public string Get_Total_Partners()
        {
            // return _users.Find(x => x.Role.Contains("Partner") && x.isPartnerAgreementAccepted==true).CountDocuments().ToString();
            return _users.Find(x => x.Role.Contains("Partner")).CountDocuments().ToString();
        }

        public string Get_Total_Referral_Earned()
        {
            double total = 0;
            var data = _leads.Find(x => x.dealValue > 0 && x.shareReceivedByUJB != null).ToList();

            foreach (var d in data)
            {
                switch (d.shareReceivedByUJB.percOrAmount)
                {
                    case 1:
                        var val = (d.dealValue / 100) * d.shareReceivedByUJB.value;

                        var actualvalue = (val / 100) * 80;

                        total += actualvalue;
                        break;
                    case 2:
                        var actualval = (d.shareReceivedByUJB.value / 100) * 80;
                        total += actualval;
                        break;
                }
            }

            return Math.Round(total).ToString(); // FormatNumber(total);
        }

        public string Get_Total_Referral_Passed()
        {
            // return _leads.Find(x => x.dealStatus == 2).CountDocuments().ToString();
            return _leads.Find(x => x.Id != null).CountDocuments().ToString();
        }

        //static string FormatNumber(double num)
        //{
        //    if (num >= 100000)
        //        return FormatNumber(num / 1000) + "K";
        //    if (num >= 10000)
        //    {
        //        return (num / 1000D).ToString("0.#") + "K";
        //    }
        //    return num.ToString("#,0");
        //}
    }
}
