using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Susbscription.Service.Models.FeePayment;
using Susbscription.Service.Models.UserDetails;
using Susbscription.Service.Repositories.FeePayment;
using System;
using System.Collections.Generic;
using System.Linq;
using UJBHelper.Common;
using UJBHelper.DataModel;
using static UJBHelper.Common.Common;

namespace Susbscription.Service.Services.FeePayment
{
    public class AddPaymentService : IFeePaymentService
    {
        private readonly IMongoCollection<FeePaymentDetails> _Feepayment;
        private readonly IMongoCollection<UserKYCDetails> _userKYCDetails;
        private readonly IMongoCollection<FeeStructure> _FeeStructure;
        private readonly IMongoCollection<SubscriptionDetails> _FeeSubscription;
        private readonly IMongoCollection<FeeType> _FeeType;
        private readonly IMongoCollection<Leads> _leadDetails;
        private readonly IMongoCollection<User> _userDetails;
        private readonly IMongoCollection<BusinessDetails> _bsnsDetails;
        private readonly IMongoCollection<PaymentDetails> _payment;
        private IConfiguration _iconfiguration;
        public List<Message_Info> _messages = null;

        public AddPaymentService(IConfiguration config)
        {
            _iconfiguration = config;
            _messages = new List<Message_Info>();
            //var client = new MongoClient(DbHelper.GetConnectionString());
            //var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _Feepayment = database.GetCollection<FeePaymentDetails>("FeePaymentDetails");
            _userKYCDetails = database.GetCollection<UserKYCDetails>("UserKYCDetails");
            _FeeStructure = database.GetCollection<FeeStructure>("FeeStructure");
            _FeeType = database.GetCollection<FeeType>("FeeType");
            _userDetails = database.GetCollection<User>("Users");
            _leadDetails = database.GetCollection<Leads>("Leads");
            _bsnsDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
            _FeeSubscription = database.GetCollection<SubscriptionDetails>("SubscriptionDetails");
            _payment = database.GetCollection<PaymentDetails>("PaymentDetails");
        }

        public List<Get_Suggestion> Get_Users_Suggestion(string query)
        {
            var res = new List<Get_Suggestion>();
            if (string.IsNullOrWhiteSpace(query))
            {
                query = "";
            }
            else
            {
                query = query.ToLower();
            }
            res = _userDetails.Find(x => (x.firstName.ToLower().Contains(query) | x.lastName.ToLower().Contains(query) | x.myMentorCode.ToLower().Contains(query)) && (x.Role != "Guest"))
                .Project(x => new Get_Suggestion
                {
                    UserId = x._id,
                    UserName = x.firstName + " " + x.lastName + " (" + x.myMentorCode + ")",
                    Role = x.Role
                }).SortBy(x => x.firstName).ToList();

            var blist1 = new List<Get_Suggestion>();
            blist1.AddRange(res);
            res.Clear();
            foreach (var b in blist1)
            {
                if (_userKYCDetails.Find(x => x.UserId == b.UserId && x.IsApproved != null && x.IsApproved.Flag ==true).CountDocuments() != 0)
                {
                    res.Add(b);
                }
            }
            
            return res;
        }

        public GetUserOtherDetails Get_Users_OtherDetails(string UserId)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var res = new GetUserOtherDetails();
            var filter = Builders<User>.Filter.Eq(x => x._id, UserId);
            res = _userDetails.Find(filter).Project(x =>
                     new GetUserOtherDetails
                     {
                         Name = x.firstName + " " + x.lastName,
                         Role = x.Role,
                         mobileNumber = x.mobileNumber,
                         Email = x.emailId,
                         RegisterationDate = x.Created.created_On
                     }).FirstOrDefault();

            if (_bsnsDetails.Find(x => x.UserId == UserId).CountDocuments() > 0)
            {
                res.leadId = _bsnsDetails.Find(x => x.UserId == UserId).FirstOrDefault().Id;
                res.BusinessRegisterationDate = _bsnsDetails.Find(x => x.UserId == UserId).FirstOrDefault().Created.created_On;
            }

            if(_FeeSubscription.Find(x => x.userId == UserId && x.feeType== "5d5a4534339dce0154441aac").CountDocuments() > 0)
            {
                DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
                var filter1 = Builders<SubscriptionDetails>.Filter.Gte(x => x.EndDate, CurrentDate);
                filter1 = filter1 & Builders<SubscriptionDetails>.Filter.Lte(x => x.StartDate, CurrentDate);
                filter1 = filter1 & Builders<SubscriptionDetails>.Filter.Eq(x => x.feeType, "5d5a4534339dce0154441aac");
                filter1 = filter1 & Builders<SubscriptionDetails>.Filter.Eq(x => x.userId, UserId);
                if (_FeeSubscription.Find(filter1).CountDocuments() > 0)
                {
                    DateTime EndDate = _FeeSubscription.Find(filter1).FirstOrDefault().EndDate;
                    res.RenewalDate = EndDate.AddDays(1);
                }
                else
                {
                    DateTime EndDate = _FeeSubscription.Find(x => x.userId == UserId).SortByDescending(x => x.EndDate).FirstOrDefault().EndDate;
                    res.RenewalDate = EndDate.AddDays(1);
                }
            }
            else
            {
                res.RenewalDate = null;
            }

            
            
            return res;
        }

        public Get_FeeBreakup Get_FeeBreakup(string UserId, string FeeType)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var res = new Get_FeeBreakup();
            List<FeePaymentDetails> feePaymentList = new List<FeePaymentDetails>();
                 
            feePaymentList = _Feepayment.Find(x => x.userId == UserId && x.feeType == FeeType).ToList();
            DateTime ApproveOn;
            int CountryId = _userDetails.Find(x => x._id == UserId).FirstOrDefault().countryId;
            if (FeeType == "5d5a450d339dce0154441aab")
            {
                ApproveOn = _userKYCDetails.Find(x => x.UserId == UserId).FirstOrDefault().IsApproved.ApprovedOn;
                var filter = Builders<FeeStructure>.Filter.Lte(x => x.FromDate,DateTime.Parse(ApproveOn.ToShortDateString()));
                filter = filter & Builders<FeeStructure>.Filter.Gte(x => x.ToDate, DateTime.Parse(ApproveOn.ToShortDateString()));
                filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.CountryId, CountryId);
                filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.FeeTypeId, FeeType);
                if (_FeeStructure.Find(filter).CountDocuments() > 0)
                {
                    res.FeeAmount = _FeeStructure.Find(filter).FirstOrDefault().Amount;
                }

                if (res.FeeAmount > 0)
                {
                    res.PaidAmount = Check_TotalPayment_Done(UserId, FeeType);
                    res.BalanceAmount = res.FeeAmount - res.PaidAmount;
                    foreach (var item in feePaymentList)
                    {
                        string ReferralNo = "NA - Adjusted offline";
                        bool isPartner = false;
                        bool isMentor = false;
                        if (item.referralId != "" && item.referralId != null)
                        {
                            isPartner = _leadDetails.Find(x => x.Id == item.referralId && x.shareRecievedByPartners.partnerID == UserId).CountDocuments() > 0;
                            isMentor = _leadDetails.Find(x => x.Id == item.referralId && x.shareRecievedByPartners.mentorID == UserId).CountDocuments() > 0;
                            ReferralNo = _leadDetails.Find(x => x.Id == item.referralId).FirstOrDefault().ReferralCode;
                        }
                        RegisterFeeDetails fee = new RegisterFeeDetails();
                        fee.ReferralNo = ReferralNo;
                        
                        fee.AdjustedDate = item.ConvertedPaymentDate;
                        fee.AdjustedAmt = item.amount;
                        foreach (var item1 in item.PaymentTransactionId)
                        {
                            fee.PaymentTransactionId = item1;

                            if (isPartner)
                            {
                                //fee.EarnedAmt = _leadDetails.Find(x => x.Id == item.referralId).FirstOrDefault().shareRecievedByPartners.RecievedByReferral;
                                fee.EarnedAmt = _payment.Find(x => x.leadId == item.referralId && x.PaymentTransactionId == item1).FirstOrDefault().ShareRecvdByPartners.RecievedByReferral;
                            }
                            else if (isMentor)
                            {
                                // fee.EarnedAmt = _leadDetails.Find(x => x.Id == item.referralId).FirstOrDefault().shareRecievedByPartners.RecievedByMentor;
                                fee.EarnedAmt = _payment.Find(x => x.leadId == item.referralId && x.PaymentTransactionId == item1).FirstOrDefault().ShareRecvdByPartners.RecievedByMentor;
                            }
                          
                        }
                        res.feeBreakUp.Add(fee);
                    }

                }
                else
                {
                    res._messages.Add(new Message_Info { Message = "Fee amount not found", Type = Message_Type.SUCCESS.ToString() });
                }

            }
            else
            {
                string SusbscriptionId = "";
                var Approveed = _bsnsDetails.Find(x => x.UserId == UserId).FirstOrDefault().isApproved;
                if (Approveed != null)
                {
                    ApproveOn = _bsnsDetails.Find(x => x.UserId == UserId).FirstOrDefault().isApproved.ApprovedOn;
                }
                SubscriptionDetails subscription = new SubscriptionDetails();
                List<SubscriptionDetails> subs = new List<SubscriptionDetails>();

                DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
                //CurrentDate = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day);

                var filter1 = Builders<SubscriptionDetails>.Filter.Gte(x => x.EndDate, CurrentDate);
                filter1 = filter1 & Builders<SubscriptionDetails>.Filter.Lte(x => x.StartDate, CurrentDate);
                filter1 = filter1 & Builders<SubscriptionDetails>.Filter.Eq(x => x.feeType, FeeType);                
                filter1 = filter1 & Builders<SubscriptionDetails>.Filter.Eq(x => x.userId, UserId);

                if (_FeeSubscription.Find(filter1).CountDocuments() > 0)
                {
                    subscription = _FeeSubscription.Find(filter1).FirstOrDefault();
                    SusbscriptionId = subscription._id;
                    res.FeeAmount = subscription.Amount;
                    DateTime FromDate = subscription.StartDate;
                    DateTime EndDate = subscription.EndDate;

                    var filter2 = Builders<SubscriptionDetails>.Filter.Eq(x => x.feeType, FeeType);
                    filter2 = filter2 & Builders<SubscriptionDetails>.Filter.Eq(x => x.userId, UserId);
                    filter2 = filter2 & Builders<SubscriptionDetails>.Filter.Ne(x => x._id, SusbscriptionId);

                    List<FeePaymentDetails> feePay = new List<FeePaymentDetails>();
                    if (_FeeSubscription.Find(filter2).CountDocuments() > 0)
                    {
                        DateTime CompareDate = _FeeSubscription.Find(filter2).SortByDescending(x => x.Created.created_On).FirstOrDefault().StartDate;

                        // List<FeePaymentDetails> feePay = new List<FeePaymentDetails>();
                        feePay = _Feepayment.Find(x => x.userId == UserId && x.feeType == FeeType && x.ConvertedPaymentDate <= FromDate).ToList();
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
                        feePay = _Feepayment.Find(x => x.userId == UserId && x.feeType == FeeType && x.ConvertedPaymentDate <= FromDate).ToList();
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

                    foreach (var item in feePaymentList)
                    {
                        List<SubscriptionDetails> subs1 = new List<SubscriptionDetails>();
                        SubscriptionDetails subscription1 = new SubscriptionDetails();
                        DateTime Date = item.ConvertedPaymentDate;
                        DateTime End = Date.AddYears(1).AddDays(-1);
                        var filter3 = Builders<SubscriptionDetails>.Filter.Gte(x => x.StartDate, Date);
                        filter3 = filter3 & Builders<SubscriptionDetails>.Filter.Lte(x => x.StartDate, End);
                        filter3 = filter3 & Builders<SubscriptionDetails>.Filter.Eq(x => x.feeType, FeeType);
                        filter3 = filter3 & Builders<SubscriptionDetails>.Filter.Eq(x => x.userId, UserId);

                        if (_FeeSubscription.Find(filter3).CountDocuments() > 0)
                        {
                            subscription1 = _FeeSubscription.Find(filter3).FirstOrDefault();

                            FeeDetails fee = new FeeDetails();
                            fee.Amount = item.amount;
                            fee.TransactionID = item.referenceNo== BsonNull.Value ? "NA" : item.referenceNo;
                            fee.TransactionDate = (item.transactionDate == BsonNull.Value ? null : item.transactionDate);
                            fee.PaymentDate = item.ConvertedPaymentDate;
                            fee.StartDate = subscription1.StartDate;
                            fee.EndDate = subscription1.EndDate;
                            fee.PaymentMode = Enum.GetName(typeof(PaymentType), int.Parse(item.paymentType));

                            res.feeBreakUp1.Add(fee);
                        }
                    }
                }

                else if (_FeeSubscription.Find(x => x.userId == UserId && x.feeType == FeeType).CountDocuments() > 0)
                {
                    FeeStructure fee = new FeeStructure();
                    int CountryId1 = _userDetails.Find(x => x._id == UserId).FirstOrDefault().countryId;
                    DateTime CurrentDate1 = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));

                    var filter = Builders<FeeStructure>.Filter.Gte(x => x.ToDate, CurrentDate1);
                    filter = filter & Builders<FeeStructure>.Filter.Lte(x => x.FromDate, CurrentDate1);
                    filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.FeeTypeId, FeeType);
                    filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.CountryId, CountryId1);
                    if (_FeeStructure.Find(filter).CountDocuments() > 0)
                    {
                        res.FeeAmount = _FeeStructure.Find(filter).FirstOrDefault().Amount;
                    }
                    
                    List<SubscriptionDetails> SubsList = new List<SubscriptionDetails>();
                    string SubId = ""; DateTime LastStartDate = DateTime.MinValue; DateTime LastEndDate = DateTime.MinValue;
                    SubsList = _FeeSubscription.Find(x => x.userId == UserId && x.feeType == FeeType).ToList();
                    foreach (var item in SubsList)
                    {
                        List<FeePaymentDetails> feePay = new List<FeePaymentDetails>();
                        int index = SubsList.IndexOf(item);
                        if (index == 0)
                        {
                            feePay = _Feepayment.Find(x => x.userId == UserId && x.feeType == FeeType && x.ConvertedPaymentDate <= item.StartDate).ToList();
                            foreach (var item1 in feePay)
                            {
                                FeeDetails fee1 = new FeeDetails();
                                fee1.Amount = item1.amount;
                                fee1.TransactionID = item1.referenceNo;
                                fee1.TransactionDate = (item1.transactionDate == BsonNull.Value ? null : item1.transactionDate);
                                fee1.PaymentDate = item1.ConvertedPaymentDate;
                                fee1.StartDate = item.StartDate;
                                fee1.EndDate = item.EndDate;
                                fee1.PaymentMode = Enum.GetName(typeof(PaymentType), int.Parse(item1.paymentType));

                                res.feeBreakUp1.Add(fee1);
                            }
                            LastStartDate = item.StartDate;
                            LastEndDate = item.EndDate;
                        }
                        else
                        {
                            feePay = _Feepayment.Find(x => x.userId == UserId && x.feeType == FeeType && x.ConvertedPaymentDate <= item.StartDate && x.ConvertedPaymentDate >= LastStartDate).ToList();

                            foreach (var item1 in feePay)
                            {
                                FeeDetails fee1 = new FeeDetails();
                                fee1.Amount = item1.amount;
                                fee1.TransactionID = item1.referenceNo;
                                fee1.TransactionDate = (item1.transactionDate == BsonNull.Value ? null : item1.transactionDate);
                                fee1.PaymentDate = item1.ConvertedPaymentDate;
                                fee1.StartDate = item.StartDate;
                                fee1.EndDate = item.EndDate;
                                fee1.PaymentMode = Enum.GetName(typeof(PaymentType), int.Parse(item1.paymentType));

                                res.feeBreakUp1.Add(fee1);
                            }
                            LastStartDate = item.StartDate;
                            LastEndDate = item.EndDate;

                        }
                    }

                    List<FeePaymentDetails> feeDetails = new List<FeePaymentDetails>();
                     feeDetails = _Feepayment.Find(x => x.userId == UserId && x.feeType == FeeType && x.ConvertedPaymentDate <= CurrentDate1).ToList();
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

                        res.feeBreakUp1.Add(fefe1);
                    }

                }

                else
                {
                    List<FeeStructure> _feeStructure = new List<FeeStructure>();
                    FeeStructure fee = new FeeStructure();
                    int CountryId1 = _userDetails.Find(x => x._id == UserId).FirstOrDefault().countryId;
                    DateTime CurrentDate1 = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
                    //CurrentDate1 = new DateTime(CurrentDate1.Year, CurrentDate1.Month, CurrentDate1.Day);

                    var filter = Builders<FeeStructure>.Filter.Gte(x => x.ToDate, CurrentDate1);
                    filter = filter & Builders<FeeStructure>.Filter.Lte(x => x.FromDate, CurrentDate1);
                    filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.FeeTypeId, FeeType);
                    filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.CountryId, CountryId1);
                    if (_FeeStructure.Find(filter).CountDocuments() > 0)
                    {
                        res.FeeAmount = _FeeStructure.Find(filter).FirstOrDefault().Amount;

                        List<FeePaymentDetails> feePay = new List<FeePaymentDetails>();
                        feePay = _Feepayment.Find(x => x.userId == UserId && x.feeType == FeeType && x.ConvertedPaymentDate <= CurrentDate1).ToList();
                        if (feePay.Count() > 0)
                        {
                            res.PaidAmount = feePay.Sum(x => x.amount);
                        }
                        res.BalanceAmount = res.FeeAmount - res.PaidAmount;

                        foreach (var item in feePaymentList)
                        {
                            FeeDetails fee1 = new FeeDetails();
                            fee1.Amount = item.amount;
                            fee1.TransactionID = item.referenceNo;
                            fee1.TransactionDate = (item.transactionDate == BsonNull.Value ? null : item.transactionDate);
                            fee1.PaymentDate = item.ConvertedPaymentDate;
                            fee1.StartDate = null;
                            fee1.EndDate = null;
                            fee1.PaymentMode = Enum.GetName(typeof(PaymentType), int.Parse(item.paymentType)).Replace("_"," ");

                            res.feeBreakUp1.Add(fee1);
                        }
                    }

                    else
                    {
                        res._messages.Add(new Message_Info { Message = "Fee not found", Type = Message_Type.SUCCESS.ToString() });
                    }
                }
            }
            return res;
        }
            
        
        public void Adjust_Partner_RegisterationFee(Post_RegisterationFee request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            ShareRecievedByPartners shared = _leadDetails.Find(x => x.Id == request.leadId).FirstOrDefault().shareRecievedByPartners;
            double Deductionamt = 0.0;
            // get the partner earned amount           
            double PartnerAmt = shared.RecievedByReferral;
            //get CountryId for the partner user
            int CountryId = _userDetails.Find(x => x._id == shared.partnerID).FirstOrDefault().countryId;
            // Get the registeration fee to be paid by partner
            DateTime ApproveOn = _userKYCDetails.Find(x => x.UserId == shared.partnerID).FirstOrDefault().IsApproved.ApprovedOn;
            var filter = Builders<FeeStructure>.Filter.Lte(x => x.FromDate, DateTime.Parse(ApproveOn.ToString("yyyy-MM-dd")));
            filter = filter & Builders<FeeStructure>.Filter.Gte(x => x.ToDate, DateTime.Parse(ApproveOn.ToString("yyyy-MM-dd")));
            filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.CountryId, CountryId);
            filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.FeeTypeId, request.feeType);
            double ActualAmt = _FeeStructure.Find(filter).FirstOrDefault().Amount;
            double PaidAmt = Check_TotalPayment_Done(shared.partnerID, request.feeType);
            double PendingAmt = ActualAmt - PaidAmt;

            if (PendingAmt > 0)
            {
                if (PartnerAmt > PendingAmt)
                {
                    Deductionamt = PendingAmt;
                }
                else
                {
                    Deductionamt = PartnerAmt;
                }

                Post_Request request1 = new Post_Request();
                request1.userId = shared.partnerID;
                request1.referralId = request.leadId;
                request1.feeType = request.feeType;
                request1.feeAmount = ActualAmt;
                request1.amount = Deductionamt;
                request1.Created_By = request.created_By;
                request1.transactionDate = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
                request1.PaymentDate = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
                request1.Description = "";
                Add_FeePayment_Details(request1);
            }

            Adjust_Mentor_RegisterationFee(shared, request);

        } 

        public void Adjust_Mentor_RegisterationFee(ShareRecievedByPartners shared,Post_RegisterationFee request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            double Deductionamt = 0.0;
            double MentorAmt = shared.RecievedByMentor;
            //get CountryId for the partner mentor
            int MentorCountryId = _userDetails.Find(x => x._id == shared.mentorID).FirstOrDefault().countryId;
            // Get the registeration fee to be paid by mentor
            DateTime MentorApproveOn = _userKYCDetails.Find(x => x.UserId == shared.mentorID).FirstOrDefault().IsApproved.ApprovedOn;
            var Mentorfilter = Builders<FeeStructure>.Filter.Lte(x => x.FromDate, DateTime.Parse(MentorApproveOn.ToString("yyyy-MM-dd")));
            Mentorfilter = Mentorfilter & Builders<FeeStructure>.Filter.Gte(x => x.ToDate, DateTime.Parse(MentorApproveOn.ToString("yyyy-MM-dd")));
            Mentorfilter = Mentorfilter & Builders<FeeStructure>.Filter.Eq(x => x.FeeTypeId, request.feeType);
            Mentorfilter = Mentorfilter & Builders<FeeStructure>.Filter.Eq(x => x.CountryId, MentorCountryId);
            double MentorActualAmt = _FeeStructure.Find(Mentorfilter).FirstOrDefault().Amount;
            double PaidAmt = Check_TotalPayment_Done(shared.mentorID, request.feeType);
            double PendingAmt = MentorActualAmt - PaidAmt;

            if (PendingAmt > 0)
            {
                if (MentorAmt > PendingAmt)
                {
                    Deductionamt = PendingAmt;
                }
                else
                {
                    Deductionamt = MentorAmt;
                }


                Post_Request request1 = new Post_Request();
                request1.userId = shared.mentorID;
                request1.referralId = request.leadId;
                request1.feeType = request.feeType;
                request1.feeAmount = MentorActualAmt;
                request1.amount = Deductionamt;
                request1.Created_By = request.created_By;
                request1.transactionDate = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
                request1.PaymentDate = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
                request1.Description = "";
                Add_FeePayment_Details(request1);
            }
        }

        public Get_FeeType Get_FeeTypes(string UserId)
        {
            var res = new Get_FeeType();
            var filter = Builders<FeeType>.Filter.Empty;

            if (UserId != string.Empty && UserId != "null" && UserId != null)
            {
                if (_userDetails.Find(x => x._id == UserId).FirstOrDefault().Role == "Partner")
                {
                    filter = filter & Builders<FeeType>.Filter.Eq(x => x.feeType, "Registration");
                }
            }

            res.FeeTypeList = _FeeType.Find(filter).Project(x =>
                    new FeeType
                    {
                        _id = x._id,
                        feeType = x.feeType

                    }).ToList();

            return res;
        }

        public void Add_FeePayment_Details(Post_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            if (request.transactionDate.HasValue)
            {
                DateTime date1 = ((DateTime)request.transactionDate).Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));                
                request.transactionDate = date1;
            }

            List<FeeStructure> _feeStructure = new List<FeeStructure>();
            FeeStructure fee = new FeeStructure();
            int CountryId = _userDetails.Find(x => x._id == request.userId).FirstOrDefault().countryId;

            DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
            //CurrentDate = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day);

            var filter = Builders<FeeStructure>.Filter.Gte(x => x.ToDate, CurrentDate);
            filter = filter & Builders<FeeStructure>.Filter.Lte(x => x.FromDate, CurrentDate);
            filter = filter &  Builders<FeeStructure>.Filter.Eq(x => x.FeeTypeId, request.feeType);
            filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.CountryId, CountryId);
            if (_FeeStructure.Find(filter).CountDocuments() > 0)
            {
                request.feeAmount = _FeeStructure.Find(filter).FirstOrDefault().Amount;
                var p = new FeePaymentDetails
                {
                    CashPaidName = request.CashPaidName,
                    userId = request.userId,
                    countryId = request.countryId,
                    emailId = request.emailId,
                    mobileNumber = request.mobileNumber,
                    referralId = request.referralId,
                    paymentType = request.paymentType,
                    ConvertedPaymentDate = request.PaymentDate.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)),
                    Description = request.Description,
                    feeType = request.feeType,
                    feeAmount = request.feeAmount,
                    amount = request.amount,
                    bankName = request.bankName,
                    branchName = request.branchName,
                    IFSCCode = request.IFSCCode,
                    accountHolderName = request.accountHolderName,
                    chequeDetails = new ChequeDetails
                    {
                        chequeNo = request.chequeNo
                    },
                    referenceNo = request.referenceNo,
                    PaymentTransactionId = request.PaymentTransactionId,
                    PaidTransactionId = request.PaidTransactionId,
                    transactionDate = request.transactionDate,
                    //ConvertedTransactionDate = (request.transactionDate == DateTime.MinValue ? DateTime.MinValue : request.transactionDate),
                    Created = new Created
                    {
                        created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)),
                        created_By = request.Created_By
                    }
                };
                _Feepayment.InsertOne(p);
                if (request.feeType != "5d5a450d339dce0154441aab")
                { 
                    // For Renewal of Membership Amt
                    Approved bsnsApprove = new Approved();
                DateTime SubscriptionStartDate;
                double yearlyAmtPaid = 0.0;
                bsnsApprove = _bsnsDetails.Find(x => x.UserId == request.userId).FirstOrDefault().isApproved;
                if (bsnsApprove != null)
                {
                    if (bsnsApprove.Flag == 1)
                    {
                        DateTime StartDate = _FeeSubscription.Find(x => x.userId == request.userId && x.feeType == request.feeType)
                                                             .SortByDescending(x => x.Created.created_On).FirstOrDefault().StartDate;
                        DateTime EndDate = _FeeSubscription.Find(x => x.userId == request.userId && x.feeType == request.feeType)
                                                             .SortByDescending(x => x.Created.created_On).FirstOrDefault().EndDate;

                        var AmtPaid = _Feepayment.AsQueryable()
                                .Where(x => x.userId == request.userId && x.feeType == request.feeType && x.ConvertedPaymentDate >= StartDate)
                                .GroupBy(d => d.userId)
                                .Select(
                                 g => new
                                 {
                                     Value = g.Sum(s => s.amount),
                                 }).FirstOrDefault();
                        if (AmtPaid != null)
                        {
                            yearlyAmtPaid = AmtPaid.Value;
                        }
                        else
                        {
                            yearlyAmtPaid = 0;
                        }

                        if (yearlyAmtPaid >= request.feeAmount)
                        {
                            if (EndDate > DateTime.Today)
                            {
                                SubscriptionStartDate = EndDate.AddDays(1).Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
                            }
                            else
                            {
                                SubscriptionStartDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
                            }
                            var subs = new SubscriptionDetails
                            {
                                userId = request.userId,
                                StartDate = SubscriptionStartDate,
                                EndDate = SubscriptionStartDate.AddYears(1).AddDays(-1),
                                Amount = request.feeAmount,
                                feeType = request.feeType,
                                Created = new Created
                                {
                                    created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                                }
                            };
                            _FeeSubscription.InsertOne(subs);
                        }
                    }
                }
            }
            }
        }

        public double Check_TotalPayment_Done(string UserId,string FeeType)
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
    }
}
