
using Lead_Management.Service.Models.Payment;
using Lead_Management.Service.Repositories.Payment;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using UJBHelper.DataModel;
using static UJBHelper.Common.Common;

namespace Lead_Management.Service.Services.Payment
{
    public class PaymentService : IAddPaymentService
    {
        private readonly IMongoCollection<PaymentDetails> _payments;
        private readonly IMongoCollection<FeePaymentDetails> _Feepayment;
        private readonly IMongoCollection<User> _userDetails;
        private readonly IMongoCollection<UserKYCDetails> _userKYCDetails;
        private readonly IMongoCollection<Leads> _leadDetails;
        private readonly IMongoCollection<System_Default> _default;
        private readonly IMongoCollection<FeeStructure> _FeeStructure;
        private readonly IMongoCollection<ReferralAgreedPercentage> _ReferralAgreedPercentage;
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private IConfiguration _iconfiguration;

        public PaymentService(IConfiguration config)
        {
            _iconfiguration = config;
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _payments = database.GetCollection<PaymentDetails>("PaymentDetails");
            _userDetails = database.GetCollection<User>("Users");
            _userKYCDetails = database.GetCollection<UserKYCDetails>("UserKYCDetails");
            _leadDetails = database.GetCollection<Leads>("Leads");
            _default = database.GetCollection<System_Default>("System_Default");
            _Feepayment = database.GetCollection<FeePaymentDetails>("FeePaymentDetails");
            _FeeStructure = database.GetCollection<FeeStructure>("FeeStructure");
            _ReferralAgreedPercentage = database.GetCollection<ReferralAgreedPercentage>("ReferralAgreedPercentage");
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
        }
        public bool Check_If_Payment_Exist(string PaymentId)
        {
            return _payments.Find(x => x._id == PaymentId).CountDocuments() > 0;
        }

        public Get_Request Get_PaymentDetails(string PaymentId)
        {
            var res = new Get_Request();

            var filter = Builders<PaymentDetails>.Filter.Eq(x => x._id, PaymentId);
            res.paymentDetails = _payments.Find(filter).Project(x =>
             new PayDetails
             {
                 _id = x._id,
                 leadId = x.leadId,
                 paymentType = x.paymentType,
                 Description = x.Description,
                 PayType = x.PayType,
                 paymentFrom = x.paymentFrom,
                 paymentTo = x.paymentTo,
                 AdjustedTransactionIds = x.AdjustedTransactionIds,
                 CPReceivedAmt = x.CPReceivedAmt,
                 paymentFor = x.paymentFor,
                 Amount = x.Amount,
                 bankName = x.bankName,
                 branchName = x.branchName,
                 IFSCCode = x.IFSCCode,
                 accountHolderName = x.accountHolderName,
                 chequeDetails = x.chequeDetails,
                 neftDetails = x.neftDetails,
                 paymentDate = x.paymentDate,
                 Created = x.Created,
                 Updated = x.Updated,
                 cashPaidName = x.cashPaidName,
                 mobileNumber = x.mobileNumber,
                 emailId = x.emailId,
                 countryCode = x.countryCode,
                 PaymentTransactionId = x.PaymentTransactionId,
                 percSharedRecvdFrmPramotion = x.percSharedRecvdFrmPramotion,
                 amtRecvdFrmPramotion = x.amtRecvdFrmPramotion,
                 amtRecvdbyUJB = x.amtRecvdbyUJB,
                 adjustedRegiFeefrmPromotion = x.adjustedRegiFeefrmPromotion,
                 sharedId = x.sharedId

                 // AmtPaidbyLP=(x.amtRecvdFrmPramotion * 100)/ x.percSharedRecvdFrmPramotion,




             }).FirstOrDefault();
            res.paymentDetails.PaymentTypeValue = Enum.GetName(typeof(PaymentType), res.paymentDetails.paymentType);
            res.paymentDetails.PayTypeValue = Enum.GetName(typeof(PayType), res.paymentDetails.PayType);
            res.paymentDetails.paymentForValue = Enum.GetName(typeof(PaymentFor), res.paymentDetails.paymentFor);
            double total = 0.0;
            foreach (string transationId in res.paymentDetails.AdjustedTransactionIds)
            {
                double AmtPaid = _payments.Find(x => x.PaymentTransactionId == transationId && x.PayType == 1).Project(x => x.Amount).FirstOrDefault();
                total += AmtPaid;
            }
            res.paymentDetails.AmtPaid = total;
            return res;
        }

        public Get_Request Get_Payment_List(string LeadId)
        {
            var res = new Get_Request();
            var filter = Builders<PaymentDetails>.Filter.Eq(x => x.leadId, LeadId);

            res.PaymentList = _payments.Find(filter).Project(x =>
                    new PaymentList
                    {
                        _id = x._id,
                        leadId = x.leadId,
                        paymentType = x.paymentType,
                        paymentFrom = x.paymentFrom,
                        // paymentTypeValue = Enum.GetName(typeof(PaymentType), x.paymentType),
                        paymentFor = x.paymentFor,
                        // paymentForValue = Enum.GetName(typeof(PaymentFor), x.paymentFor),
                        paymentTo = x.paymentTo,
                        Amount = x.Amount,
                        bankName = x.bankName,
                        branchName = x.branchName,
                        IFSCCode = x.IFSCCode,
                        accountHolderName = x.accountHolderName,
                        chequeDetails = x.chequeDetails,
                        neftDetails = x.neftDetails,
                        paymentDate = x.paymentDate,
                        cashPaidName = x.cashPaidName,
                        mobileNumber = x.mobileNumber,
                        emailId = x.emailId,
                        PaymentTransactionId = x.PaymentTransactionId,
                        AdjustedTransactionIds = x.AdjustedTransactionIds,
                        countryCode = x.countryCode,
                        percSharedRecvdFrmPramotion = x.percSharedRecvdFrmPramotion,
                        amtRecvdFrmPramotion = x.amtRecvdFrmPramotion,
                        amtRecvdbyUJB = x.amtRecvdbyUJB,
                        TotalAmtpaidtoParter = (x.Amount + x.amtRecvdFrmPramotion)


                    }).ToList();

            foreach (var item in res.PaymentList)
            {
                if (item.AdjustedTransactionIds.Count() > 0)
                {
                    if (_Feepayment.Find(x => item.paymentTo.Contains(x.userId) &&
                                     x.feeType == "5d5a450d339dce0154441aab" &&
                                     x.PaidTransactionId == item.PaymentTransactionId).CountDocuments() > 0)
                    {
                        item.RegisterationAmt = _Feepayment.Find(x => item.paymentTo.Contains(x.userId) &&
                                         x.feeType == "5d5a450d339dce0154441aab" &&
                                         x.PaidTransactionId == item.PaymentTransactionId).FirstOrDefault().amount;
                    }
                }

                if (_payments.Find(x => x.leadId == item.leadId && x.AdjustedTransactionIds.Contains(item.PaymentTransactionId)).CountDocuments() > 0)

                {
                    item.AllowEdit = false;
                }
                else
                {
                    item.AllowEdit = true;
                }


                item.paymentToValue = new List<string>();

                foreach (var item1 in item.paymentTo)
                {
                    if (item1 != "UJB")
                    {
                        item.paymentToValue.Add(_userDetails.Find(x => x._id == item1).FirstOrDefault().firstName.ToString() + " " + _userDetails.Find(x => x._id == item1).FirstOrDefault().lastName.ToString());
                    }
                    else
                    {
                        item.paymentToValue.Add("UJB");
                    }
                }
                if (item.paymentFrom != "UJB")
                {
                    item.paymentFromValue = _userDetails.Find(x => x._id == item.paymentFrom).FirstOrDefault().firstName.ToString() + " " + _userDetails.Find(x => x._id == item.paymentFrom).FirstOrDefault().lastName.ToString();
                }
                else
                {
                    item.paymentFromValue = "UJB";
                }
                item.paymentForValue = Enum.GetName(typeof(PaymentFor), item.paymentFor);
                if (item.paymentType != 0)
                {
                    item.paymentTypeValue = Enum.GetName(typeof(PaymentType), item.paymentType).Replace("_", " ");
                }
            }
            res.totalCount = res.PaymentList.Count;
            return res;
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

        public double Get_TotalShare(string leadId, List<string> PaymentTo, List<string> TransactionIds)
        {
            var Filter = Builders<PaymentDetails>.Filter.Eq(x => x.leadId, leadId) & Builders<PaymentDetails>.Filter.AnyIn(x => x.paymentTo, PaymentTo);
            if (TransactionIds.Count > 0)
            {
                Filter = Filter & Builders<PaymentDetails>.Filter.Nin(x => x.PaymentTransactionId, TransactionIds);
            }
            var AmtPaid = _payments.AsQueryable()
              .Where(x => x.leadId == leadId && x.paymentTo.Contains("UJB") && !TransactionIds.Contains(x.PaymentTransactionId))
              .GroupBy(d => d.leadId)
              .Select(
               g => new
               {
                   Value = g.Sum(s => s.Amount),
               }).FirstOrDefault();
            if (AmtPaid != null)
            {
                return AmtPaid.Value;
            }
            else
            {
                return 0;
            }
        }

        public double Get_TotalPayment_Done_ForLead(string LeadId, string PaymentFrom, string UserId, int sharedId)
        {
            var AmtPaid = _payments.AsQueryable()
             .Where(x => x.leadId == LeadId && x.paymentFrom == PaymentFrom && x.paymentTo.Contains(UserId) && x.sharedId == sharedId)
             .GroupBy(d => d.leadId)
             .Select(
              g => new
              {
                  Value = g.Sum(s => s.Amount),
              }).FirstOrDefault();

            if (AmtPaid != null)
            {

                return double.Parse(AmtPaid.Value.ToString());
            }
            else
            {

                return 0.00;
            }
        }

        public double GetRegisterationAmt(string UserId)
        {
            int CountryId = _userDetails.Find(x => x._id == UserId).FirstOrDefault().countryId;
            DateTime ApproveOn = _userKYCDetails.Find(x => x.UserId == UserId).FirstOrDefault().IsApproved.ApprovedOn;
            var filter = Builders<FeeStructure>.Filter.Lte(x => x.FromDate, DateTime.Parse(ApproveOn.ToString("yyyy-MM-dd")));
            filter = filter & Builders<FeeStructure>.Filter.Gte(x => x.ToDate, DateTime.Parse(ApproveOn.ToString("yyyy-MM-dd")));
            filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.CountryId, CountryId);
            filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.FeeTypeId, "5d5a450d339dce0154441aab");
            return _FeeStructure.Find(filter).FirstOrDefault().Amount;
        }

        public GetBalance Calculate_Balance(Put_Request request)
        {
            var res = new GetBalance();
            double sharedamt = 0.00;
            double PendingAmt = 0.00;
            double PaidRegisterationAmt = 0.00;
            double RegisterationAmt = 0.00;
            double BalRegisterationAmt = 0.00;

            if (!request.IsUJB)
            {
                var paymentTo = request.PaymentTo.Split('-');
                if (_payments.Find(x => x.leadId == request.leadId && x.paymentTo.Contains("UJB")).CountDocuments() > 0)
                {
                    List<string> Transaction = new List<string>();
                    List<string> PaymentTo = new List<string>();
                    PaymentTo.Add("UJB");

                    var TransactionId = _payments.Find(x => x.leadId == request.leadId && x.paymentTo.Contains(paymentTo[0]) && x.sharedId == int.Parse(paymentTo[1]) && !x.paymentTo.Contains("UJB")).Project(u => u.AdjustedTransactionIds).ToList();
                    ///x.sharedId == int.Parse(paymentTo[1]) &&
                    foreach (var item in TransactionId)
                    {
                        foreach (var item1 in item)
                        {
                            Transaction.Add(item1);
                        }
                    }
                    //var TransactionId = _Feepayment.Find(x => x.userId == paymentTo[0] && x.feeType == "5d5a450d339dce0154441aab")
                    //                            .Project(u => u.PaymentTransactionId).ToList();

                    //foreach (var item in TransactionId)
                    //{
                    //    foreach (var item1 in item)
                    //    {
                    //        Transaction.Add(item1);
                    //    }
                    //}

                    var Filter = Builders<PaymentDetails>.Filter.Eq(x => x.leadId, request.leadId) & Builders<PaymentDetails>.Filter.AnyIn(x => x.paymentTo, PaymentTo);
                    // & Builders<PaymentDetails>.Filter.Eq(x => x.sharedId, int.Parse(paymentTo[1]));
                    if (Transaction.Count > 0)
                    {
                        Filter = Filter & Builders<PaymentDetails>.Filter.Nin(x => x.PaymentTransactionId, Transaction);
                    }


                    List<ShareRecievedByPartners> sharedList = _payments.Find(Filter).Project(u => u.ShareRecvdByPartners).ToList();
                    res.TransactionIds = _payments.Find(Filter).Project(u => u.PaymentTransactionId).ToList();
                    res.TotalShareAmt = Get_TotalShare(request.leadId, PaymentTo, Transaction);
                    if (sharedList.Count > 0 && res.TransactionIds.Count > 0)
                    {
                        foreach (var item in sharedList)
                        {
                            if (item.partnerID == paymentTo[0] && int.Parse(paymentTo[1]) == 2)
                            {
                                sharedamt = sharedamt + item.RecievedByReferral;
                            }
                            else if (item.mentorID == paymentTo[0] && int.Parse(paymentTo[1]) == 3)
                            {
                                sharedamt = sharedamt + item.RecievedByMentor;
                            }
                            else if (item.LPmentorID == paymentTo[0] && int.Parse(paymentTo[1]) == 4)
                            {
                                sharedamt = sharedamt + item.RecievedByLPMentor;
                            }
                        }

                        // get the Registeration Amt of the user
                        RegisterationAmt = GetRegisterationAmt(paymentTo[0]);

                        //get the registeration paid amt of the user
                        PaidRegisterationAmt = Check_TotalPayment_Done(paymentTo[0], "5d5a450d339dce0154441aab");
                        res.BalanceRegisterationAmt = RegisterationAmt - PaidRegisterationAmt;
                        res.BalRegisterationAmt = res.BalanceRegisterationAmt;

                        // Get the payment done to user against a lead from UJB
                        res.PaidAmt = Get_TotalPayment_Done_ForLead(request.leadId, request.paymentFrom, PaymentTo[0], int.Parse(paymentTo[1]));
                        res.ActualAmt = sharedamt;

                        if (res.ActualAmt < res.BalanceRegisterationAmt)
                        {
                            res.BalanceRegisterationAmt = res.ActualAmt;
                        }

                        if (paymentTo[0] != "UJB")
                        {
                            if (res.BalanceRegisterationAmt > 0)
                            {
                                res.PendingAmt = res.ActualAmt - res.BalanceRegisterationAmt;
                            }
                            else
                            {
                                res.PendingAmt = res.ActualAmt;
                            }
                        }

                        // res.PendingAmt = (res.ActualAmt - res.PaidAmt) - res.BalanceRegisterationAmt;





                    }
                }
                else
                {
                    return res;
                }
            }
            else
            {
                var ReceivedByUJB = _leadDetails.Find(x => x.Id == request.leadId)
                                     .FirstOrDefault().shareReceivedByUJB;
                double DealValue = _leadDetails.Find(x => x.Id == request.leadId)
                                     .FirstOrDefault().dealValue;

                var AmtPaid = _payments.AsQueryable()
                     .Where(x => x.leadId == request.leadId && x.paymentFrom == request.paymentFrom && x.paymentTo.Contains(request.PaymentTo))
                     .GroupBy(d => d.leadId)
                     .Select(
                      g => new
                      {
                          Value = g.Sum(s => s.Amount),
                      }).FirstOrDefault();
                if (ReceivedByUJB.percOrAmount == 1)
                {
                    sharedamt = ((DealValue * ReceivedByUJB.value) / 100);

                }
                else
                {
                    sharedamt = ReceivedByUJB.value;
                }
                if (AmtPaid != null)
                {
                    PendingAmt = double.Parse(sharedamt.ToString()) - double.Parse(AmtPaid.Value.ToString());
                    res.PaidAmt = double.Parse(AmtPaid.Value.ToString());
                }
                else
                {
                    PendingAmt = double.Parse(sharedamt.ToString());
                    res.PaidAmt = 0.0;
                }
                res.ActualAmt = sharedamt;
                res.PendingAmt = PendingAmt;
            }
            return res;
        }

        public bool Check_If_Lead_Exist(string LeadId)
        {
            return _leadDetails.Find(x => x.Id == LeadId).CountDocuments() > 0;
        }

        public string GeneratePaymentTransactionCode()
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            int PTNoLength = 0;
            int CurrYear = 0;
            int CurrentYear = 0;
            var NextYear = 0;
            var PTNoCounter = "";
            var PtCode = "";
            int CurrentMonth = 0;
            int CurrentDay = 0;
         
            List<System_Default> systemDefault = new List<System_Default>();
            System_Default system = new System_Default();

            DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
            // CurrentDate = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day);

            // DateTime endDate = DateTime.Parse(DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)).ToString("MM/dd/yyyy"));
            // DateTime startDate = DateTime.Parse(DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)).ToString("dd/MM/yyyy"));

            var filter = Builders<System_Default>.Filter.Gte(x => x.endDate, CurrentDate);
            filter = filter & Builders<System_Default>.Filter.Lte(x => x.startDate, CurrentDate);
            filter = filter & Builders<System_Default>.Filter.Eq(x => x.Default_Name, "PaymentTransactionCode");
            system = _default.Find(filter).FirstOrDefault();
            //system = systemDefault.Where(x => x.StartFecha <= startDate).FirstOrDefault();

            ////var filter = Builders<System_Default>.Filter.Lte(x => x.StartFecha, DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")));
            ////filter = filter & Builders<System_Default>.Filter.Gte(x => x.EndFecha, DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")));


            //CurrentYear = DateTime.Now.Year;
            //DateTime dt = system.startDate;
            //CurrYear = dt.Year;
            //NextYear = CurrYear + 1;

            CurrentYear = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)).Year;
            CurrentMonth = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)).Month;
            CurrentDay = DateTime.Now.Day;
            DateTime dt = _default.Find(filter).FirstOrDefault().startDate;

            if (CurrentMonth > 3 && CurrentDay >= 1)
            {
                CurrYear = CurrentYear;
                NextYear = CurrYear + 1;
            }
            else
            {
                CurrYear = CurrentYear - 1;
                NextYear = CurrentYear;
            }

            PTNoCounter = system.Default_Value;
            PTNoLength = PTNoCounter.Length;
            PtCode = "PT/" + CurrYear.ToString().Substring(CurrYear.ToString().Length - 2) + "-" + NextYear.ToString().Substring(NextYear.ToString().Length - 2) + "/";

            if (PTNoLength == 1)
            {
                PtCode = PtCode + "0000000" + int.Parse(PTNoCounter);
            }
            else if (PTNoLength == 2)
            {
                PtCode = PtCode + "000000" + int.Parse(PTNoCounter);
            }
            else if (PTNoLength == 3)
            {
                PtCode = PtCode + "00000" + int.Parse(PTNoCounter);
            }
            else if (PTNoLength == 4)
            {
                PtCode = PtCode + "0000" + int.Parse(PTNoCounter);
            }
            else if (PTNoLength == 5)
            {
                PtCode = PtCode + "000" + int.Parse(PTNoCounter);
            }
            else if (PTNoLength == 6)
            {
                PtCode = PtCode + "00" + int.Parse(PTNoCounter);
            }
            else if (PTNoLength == 7)
            {
                PtCode = PtCode + "0" + int.Parse(PTNoCounter);
            }
            else
            {
                PtCode = PtCode + int.Parse(PTNoCounter);
            }
            return PtCode;
        }

        public void Update_System_Default(string Default_Name)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            List<System_Default> systemDefault = new List<System_Default>();
            System_Default system = new System_Default();

            DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
            //CurrentDate = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day);

            //DateTime endDate = DateTime.Parse(DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)).ToString("MM/dd/yyyy"));
            //DateTime startDate = DateTime.Parse(DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)).ToString("dd/MM/yyyy"));

            var filter = Builders<System_Default>.Filter.Gte(x => x.endDate, CurrentDate);
            filter = filter & Builders<System_Default>.Filter.Lte(x => x.startDate, CurrentDate);
            filter = filter & Builders<System_Default>.Filter.Eq(x => x.Default_Name, "PaymentTransactionCode");
            system = _default.Find(filter).FirstOrDefault();

            //system = systemDefault.Where(x => x.StartFecha <= startDate).FirstOrDefault();
            //var filter = Builders<System_Default>.Filter.Lte(x => x.StartFecha, DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")));
            //filter = filter & Builders<System_Default>.Filter.Gte(x => x.EndFecha, DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")));
            //filter = filter & Builders<System_Default>.Filter.Eq(x => x.Default_Name, Default_Name);

            int value = int.Parse(system.Default_Value);
            value = value + 1;
            _default.FindOneAndUpdate(
                Builders<System_Default>.Filter.Eq(x => x.Default_Name, Default_Name),
               Builders<System_Default>.Update
               .Set(x => x.Default_Value, value.ToString())
               );
        }

        public string Insert_New_Payment(Post_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });
            double sharewithujb = 0.0;
            string PTCode = GeneratePaymentTransactionCode();
            if (request.paymentFrom != "UJB")
            {
                ///  double TotalShareAmt = Get_TotalShare(request.leadId, paymentTo[0], request.AdjustedTransactionIds);
                var ujbperc = _ReferralAgreedPercentage.Find(x => x.transferTo == 1).Project(x => x.Percentage).FirstOrDefault();

                sharewithujb = (request.amount * (Double.Parse(ujbperc.ToString()) / 100.00));

            }
            else
            {
                sharewithujb = Convert.ToDouble("-" + request.amtRecvdFrmPramotion.ToString()) + Convert.ToDouble("-" + request.adjustedRegiFeefrmPromotion.ToString());
            }
            //ShareRecievedByPartners shareReceived = new ShareRecievedByPartners();
            //shareReceived = _leadDetails.Find(x => x.Id == request.leadId).FirstOrDefault().shareRecievedByPartners;
            //request.ShareRecvdByPartners.partnerID = shareReceived.partnerID;
            //request.ShareRecvdByPartners.mentorID = shareReceived.mentorID;
            var userId = _leadDetails.Find(x => x.Id == request.leadId).Project(x => x.referredBy.userId).FirstOrDefault();

            var userDetails = _userDetails.Find(x => x._id == userId).FirstOrDefault();
            request.ShareRecvdByPartners.partnerID = userId;
            //= userDetails.mentorCode;

            // string MentorCode = _users.Find(x => x._id == ReferredById).FirstOrDefault().mentorCode.ToString();
            request.ShareRecvdByPartners.mentorID = _userDetails.Find(x => x.myMentorCode == userDetails.mentorCode).FirstOrDefault()._id.ToString();
            var referredBusinessId = _leadDetails.Find(x => x.Id == request.leadId).Project(x => x.referredBusinessId).FirstOrDefault();
            var bussinesuser_id = _businessDetails.Find(x => x.Id == referredBusinessId).Project(x => x.UserId).FirstOrDefault();
            var LPmentorCode = _userDetails.Find(x => x._id == bussinesuser_id).Project(x => x.mentorCode).FirstOrDefault();
            request.ShareRecvdByPartners.LPmentorID = _userDetails.Find(x => x.myMentorCode == LPmentorCode).Project(x => x._id).FirstOrDefault();
            List<string> payment_to = new List<string>();
            payment_to.Add(request.PaymentTo);
            var p = new PaymentDetails
            {
                paymentType = request.paymentType,
                leadId = request.leadId,
                Description = request.Description,
                Amount = request.amount,
                bankName = request.bankName,
                branchName = request.branchName,
                IFSCCode = request.IFSCCode,
                accountHolderName = request.accountHolderName,
                paymentFor = request.paymentFor,
                paymentFrom = request.paymentFrom,
                paymentTo = payment_to,
                cashPaidName = request.cashPaidName,
                countryCode = request.countryCode,
                mobileNumber = request.mobileNumber,
                PaymentTransactionId = PTCode,
                emailId = request.emailId,
                AdjustedTransactionIds = request.AdjustedTransactionIds,
                CPReceivedAmt = request.CPReceivedAmt,
                amtRecvdFrmPramotion = request.amtRecvdFrmPramotion,
                percSharedRecvdFrmPramotion = request.percSharedRecvdFrmPramotion,
                adjustedRegiFeefrmPromotion = request.adjustedRegiFeefrmPromotion,
                amtRecvdbyUJB = sharewithujb,
                sharedId = request.sharedId,
                chequeDetails = new ChequeDetails
                {
                    chequeNo = request.chequeNo
                },
                neftDetails = new NeftDetails
                {
                    ReferrenceNo = request.ReferrenceNo,
                    TransactionDate = request.TransactionDate
                },
                PayType = request.PayType,
                paymentDate = request.paymentDate,
                ShareRecvdByPartners = request.ShareRecvdByPartners,
                Created = new Created
                {
                    created_By = request.created_By,
                    created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                },
                Updated = new Updated()
            };
            _payments.InsertOne(p);
            return PTCode;
        }

        public void Update_Payment_Details(Post_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            ShareRecievedByPartners shareReceived = new ShareRecievedByPartners();
            double sharewithujb = 0.0;

            if (request.paymentFrom != "UJB")
            {
                //  double TotalShareAmt = Get_TotalShare(request.leadId, request.PaymentTo, request.AdjustedTransactionIds);
                var ujbperc = _ReferralAgreedPercentage.Find(x => x.transferTo == 1).Project(x => x.Percentage).FirstOrDefault();

                sharewithujb = Convert.ToDouble("-" + request.amtRecvdFrmPramotion.ToString()) + Convert.ToDouble("-" + request.adjustedRegiFeefrmPromotion.ToString());

            }
            else
            {
                sharewithujb = Convert.ToDouble("-" + request.amtRecvdFrmPramotion.ToString());
            }
            //shareReceived = _leadDetails.Find(x => x.Id == request.leadId).FirstOrDefault().shareRecievedByPartners;
            // request.ShareRecvdByPartners.partnerID = shareReceived.partnerID;
            // request.ShareRecvdByPartners.mentorID = shareReceived.mentorID;

            var userId = _leadDetails.Find(x => x.Id == request.leadId).Project(x => x.referredBy.userId).FirstOrDefault();

            var userDetails = _userDetails.Find(x => x._id == userId).FirstOrDefault();
            request.ShareRecvdByPartners.partnerID = userId;
            //= userDetails.mentorCode;

            // string MentorCode = _users.Find(x => x._id == ReferredById).FirstOrDefault().mentorCode.ToString();
            request.ShareRecvdByPartners.mentorID = _userDetails.Find(x => x.myMentorCode == userDetails.mentorCode).FirstOrDefault()._id.ToString();
            var referredBusinessId = _leadDetails.Find(x => x.Id == request.leadId).Project(x => x.referredBusinessId).FirstOrDefault();
            var bussinesuser_id = _businessDetails.Find(x => x.Id == referredBusinessId).Project(x => x.UserId).FirstOrDefault();
            var LPmentorCode = _userDetails.Find(x => x._id == bussinesuser_id).Project(x => x.mentorCode).FirstOrDefault();
            List<string> payment_to = new List<string>();
            payment_to.Add(request.PaymentTo);
            request.ShareRecvdByPartners.LPmentorID = _userDetails.Find(x => x.myMentorCode == LPmentorCode).Project(x => x._id).FirstOrDefault();
            _payments.FindOneAndUpdate(
                Builders<PaymentDetails>.Filter.Eq(x => x._id, request.PaymentId),
                Builders<PaymentDetails>.Update
                .Set(x => x.paymentType, request.paymentType)
                .Set(x => x.leadId, request.leadId)
                .Set(x => x.Description, request.Description)
                .Set(x => x.Amount, request.amount)
                .Set(x => x.bankName, request.bankName)
                .Set(x => x.branchName, request.branchName)
                .Set(x => x.IFSCCode, request.IFSCCode)
                .Set(x => x.paymentFor, request.paymentFor)
                .Set(x => x.paymentFrom, request.paymentFrom)
                .Set(x => x.paymentTo, payment_to)
                .Set(x => x.cashPaidName, request.cashPaidName)
                .Set(x => x.countryCode, request.countryCode)
                .Set(x => x.mobileNumber, request.mobileNumber)
                .Set(x => x.emailId, request.emailId)
                .Set(x => x.AdjustedTransactionIds, request.AdjustedTransactionIds)
                .Set(x => x.CPReceivedAmt, request.CPReceivedAmt)
                .Set(x => x.accountHolderName, request.accountHolderName)
                 .Set(x => x.amtRecvdFrmPramotion, request.amtRecvdFrmPramotion)
                .Set(x => x.percSharedRecvdFrmPramotion, request.percSharedRecvdFrmPramotion)
                  .Set(x => x.adjustedRegiFeefrmPromotion, request.adjustedRegiFeefrmPromotion)
                   .Set(x => x.amtRecvdbyUJB, sharewithujb)
                    .Set(x => x.sharedId, request.sharedId)

                .Set(x => x.chequeDetails, new ChequeDetails
                {
                    chequeNo = request.chequeNo
                })
                .Set(x => x.neftDetails, new NeftDetails
                {
                    ReferrenceNo = request.ReferrenceNo,
                    TransactionDate = request.TransactionDate
                })
                .Set(x => x.PayType, request.PayType)
                .Set(x => x.paymentDate, request.paymentDate)
                .Set(x => x.ShareRecvdByPartners, request.ShareRecvdByPartners)
                .Set(x => x.Updated, new Updated
                {
                    updated_By = request.created_By,
                    updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                })
                );
        }

        public double GetSharedPercentage(string leadId, int sharedwithid)
        {

            DateTime referraldate = _leadDetails.Find(x => x.Id == leadId).FirstOrDefault().referralDate ?? DateTime.Now;


            // var filter = Builders<ReferralAgreedPercentage>.Filter.Eq(x => x.isActive, true); 
            //
            var filter = Builders<ReferralAgreedPercentage>.Filter.Gte(x => x.EffectiveEndDate, DateTime.Parse(referraldate.ToString("yyyy-MM-dd")));
            Builders<ReferralAgreedPercentage>.Filter.Lte(x => x.EffectiveStartDate, DateTime.Parse(referraldate.ToString("yyyy-MM-dd")));
            filter = filter & Builders<ReferralAgreedPercentage>.Filter.Eq(x => x.transferTo, sharedwithid);

            return _ReferralAgreedPercentage.Find(filter).FirstOrDefault().Percentage;
        }
    }
}
