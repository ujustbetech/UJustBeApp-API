using Lead_Management.Service.Manager.Referral;
using Lead_Management.Service.Models.Referral;
using Lead_Management.Service.Repositories.Referral;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using UJBHelper.DataModel;

namespace Lead_Management.Service.Services.Referral
{
    public class ReferralService : IReferralService
    {
        private readonly IMongoCollection<Categories> _categories;
        private readonly IMongoCollection<Leads> _lead;
        private readonly IMongoCollection<LeadsStatusHistory> _leadStatusHistory;
        private readonly IMongoCollection<DbProductService> _productsAndService;
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<AdminUser> _adminUsers;
        private readonly IMongoCollection<System_Default> _default;
        private readonly IMongoCollection<DealStatus> _dealStatus;
        private readonly IMongoCollection<PaymentDetails> _payments;
        private readonly IMongoCollection<ReferralAgreedPercentage> _ReferralAgreedPercentage;
        private readonly IMongoCollection<ProductServiceDetails> _productsDetails;
        private readonly IMongoCollection<LeadProductServiceDetails> _leadProductsDetails;
        private IConfiguration _iconfiguration;
        public ReferralService(IConfiguration config)
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
            _default = database.GetCollection<System_Default>("System_Default");
            _dealStatus = database.GetCollection<DealStatus>("DealStatus");
            _payments = database.GetCollection<PaymentDetails>("PaymentDetails");
            _productsDetails = database.GetCollection<ProductServiceDetails>("ProductsServicesDetails");
            _leadProductsDetails = database.GetCollection<LeadProductServiceDetails>("LeadProductServiceDetails");
            _ReferralAgreedPercentage = database.GetCollection<ReferralAgreedPercentage>("ReferralAgreedPercentage");
        }

        public string GenerateReferralCode()
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            int ReferralNoLength = 0;
            int CurrYear = 0;
            int CurrentYear = 0;
            int CurrentMonth = 0;
            int CurrentDay = 0;
            var NextYear = 0;
            var ReferralNoCounter = "";
            var ReferralCode = "";

            DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
            //CurrentDate = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day);

            var filter = Builders<System_Default>.Filter.Lte(x => x.startDate, CurrentDate);
            filter = filter & Builders<System_Default>.Filter.Gte(x => x.endDate, CurrentDate);
            filter = filter & Builders<System_Default>.Filter.Eq(x => x.Default_Name, "ReferralCode");

            CurrentYear = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)).Year;
            CurrentMonth = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)).Month;
            CurrentDay = DateTime.Now.Day;
            DateTime dt = _default.Find(filter).FirstOrDefault().startDate;

            if ((CurrentMonth > 3) && (CurrentDay > 0))
            {
                CurrYear = CurrentYear;
                NextYear = CurrYear + 1;
            }
            else
            {
                CurrYear = CurrentYear - 1;
                NextYear = CurrentYear;
            }
            ReferralNoCounter = _default.Find(filter).FirstOrDefault().Default_Value;
            ReferralNoLength = ReferralNoCounter.Length;
            ReferralCode = "Ref/" + CurrYear.ToString().Substring(CurrYear.ToString().Length - 2) + "-" + NextYear.ToString().Substring(NextYear.ToString().Length - 2) + "/";

            if (ReferralNoLength == 1)
            {
                ReferralCode = ReferralCode + "0000000" + int.Parse(ReferralNoCounter);
            }
            else if (ReferralNoLength == 2)
            {
                ReferralCode = ReferralCode + "000000" + int.Parse(ReferralNoCounter);
            }
            else if (ReferralNoLength == 3)
            {
                ReferralCode = ReferralCode + "00000" + int.Parse(ReferralNoCounter);
            }
            else if (ReferralNoLength == 4)
            {
                ReferralCode = ReferralCode + "0000" + int.Parse(ReferralNoCounter);
            }
            else if (ReferralNoLength == 5)
            {
                ReferralCode = ReferralCode + "000" + int.Parse(ReferralNoCounter);
            }
            else if (ReferralNoLength == 6)
            {
                ReferralCode = ReferralCode + "00" + int.Parse(ReferralNoCounter);
            }
            else if (ReferralNoLength == 7)
            {
                ReferralCode = ReferralCode + "0" + int.Parse(ReferralNoCounter);
            }
            else
            {
                ReferralCode = ReferralCode + int.Parse(ReferralNoCounter);
            }
            return ReferralCode;
        }

        public bool Check_If_Lead_Exists(string referralId)
        {
            return _lead.Find(x => x.Id == referralId).CountDocuments() != 0;
        }

        public bool Is_Active_Users(string referredById, string businessId)
        {
            var refactive = _users.Find(x => !x.isActive && x._id == referredById).CountDocuments();
            string LPactiveId = _businessDetails.Find(x => x.Id == businessId).Project(x => x.UserId).FirstOrDefault();
            var LPactive = _users.Find(x => !x.isActive && x._id == LPactiveId).CountDocuments();
            if (refactive > 0 || LPactive > 0)
            {
                return false;
            }
            return true;
        }
        public void Update_System_Default(string Default_Name)
        {

            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
            // CurrentDate = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day);

            var filter = Builders<System_Default>.Filter.Lte(x => x.startDate, CurrentDate);
            filter = filter & Builders<System_Default>.Filter.Gte(x => x.endDate, CurrentDate);
            filter = filter & Builders<System_Default>.Filter.Eq(x => x.Default_Name, Default_Name);

            int value = int.Parse(_default.Find(filter).FirstOrDefault().Default_Value);
            value = value + 1;
            _default.FindOneAndUpdate(
                Builders<System_Default>.Filter.Eq(x => x.Default_Name, Default_Name),
               Builders<System_Default>.Update
               .Set(x => x.Default_Value, value.ToString())
               );
        }

        public DealValue_Get Update_DealValue(DealValue_Put request)
        {
            var response = new DealValue_Get();
            string ReferredById = _lead.Find(x => x.Id == request.leadId).FirstOrDefault().referredBy.userId;
            request.shareReceivedByPartner.partnerID = ReferredById;
            string MentorCode = _users.Find(x => x._id == ReferredById).FirstOrDefault().mentorCode.ToString();
            request.shareReceivedByPartner.mentorID = _users.Find(x => x.myMentorCode == MentorCode).FirstOrDefault()._id.ToString();
            var referredBusinessId = _lead.Find(x => x.Id == request.leadId).Project(x => x.referredBusinessId).FirstOrDefault();
            var bussinesuser_id = _businessDetails.Find(x => x.Id == referredBusinessId).Project(x => x.UserId).FirstOrDefault();
            var LPmentorCode = _users.Find(x => x._id == bussinesuser_id).Project(x => x.mentorCode).FirstOrDefault();
            request.shareReceivedByPartner.LPmentorID = _users.Find(x => x.myMentorCode == LPmentorCode).Project(x => x._id).FirstOrDefault();
            request.PercentOrAmt = _leadProductsDetails.Find(x => x.prodservdetailId == request.ProductId && x.LeadId == request.leadId).Project(x => x.type).FirstOrDefault();
            request.Value = _leadProductsDetails.Find(x => x.prodservdetailId == request.ProductId && x.LeadId == request.leadId).Project(x => x.value).FirstOrDefault();
            _lead.FindOneAndUpdate(
               Builders<Leads>.Filter.Eq(x => x.Id, request.leadId),
               Builders<Leads>.Update
               .Set(x => x.dealValue, request.dealValue)
               .Set(x => x.refMultisSlabProdId, request.ProductId)
               .Set(x => x.shareRecievedByPartners, new ShareRecievedByPartners
               {
                   partnerID = request.shareReceivedByPartner.partnerID,
                   RecievedByReferral = request.shareReceivedByPartner.RecievedByReferral,
                   mentorID = request.shareReceivedByPartner.mentorID,
                   RecievedByMentor = request.shareReceivedByPartner.RecievedByMentor,
                   RecievedByUJB = request.shareReceivedByPartner.RecievedByUJB,
                   LPmentorID = request.shareReceivedByPartner.LPmentorID,
                   RecievedByLPMentor = request.shareReceivedByPartner.RecievedByLPMentor
               })
               .Set(x => x.shareReceivedByUJB, new ShareReceivedByUJB
               {
                   percOrAmount = request.PercentOrAmt,
                   value = request.Value
               })
               );
            response.ShareDetails = request.shareReceivedByPartner;
            return response;
        }

        public string Create_New_Referral(Post_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            string referralCode = GenerateReferralCode();
            var refactive = _users.Find(x => x.isActive && x._id == request.referredById).CountDocuments();
            string LPactiveId = _businessDetails.Find(x => x.Id == request.businessId).Project(x => x.UserId).FirstOrDefault();
            var LPactive = _users.Find(x => x.isActive && x._id == LPactiveId).CountDocuments();
            if (refactive > 0 && LPactive > 0)
            {
                var shareType = _productsAndService.Find(x => x.Id == request.selectedProductId).FirstOrDefault().shareType;
                var _typeof = _productsAndService.Find(x => x.Id == request.selectedProductId).FirstOrDefault().typeOf;
                var lead = new Leads
                {
                    referralStatus = 0,
                    dealStatus = 0,
                    isForSelf = request.forSelf,
                    //  isAccepted = false,
                    referralDate = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)),
                    referralDescription = request.shortDescription,
                    referredBusinessId = request.businessId,
                    referredBy = new ReferredBy
                    {
                        name = request.referredByName,
                        userId = request.referredById
                    },
                    referredProductORServices = request.selectedProduct,
                    referredProductORServicesId = request.selectedProductId,
                    refMultisSlabProdId = request.productServiceSlabId,
                    referredTo = new ReferredTo
                    {
                        name = request.referredToName,
                        countryCode = request.countryCode,
                        emailId = request.emailId,
                        mobileNumber = request.mobileNumber
                    },
                    ReferralCode = referralCode,
                    shareReceivedByUJB = new ShareReceivedByUJB(),
                    shareRecievedByPartners = new ShareRecievedByPartners(),
                    shareType = shareType,
                    typeOf = _typeof,
                    //dealValue = request.dealValue,
                    //shareReceivedByUJB = new ShareReceivedByUJB
                    //{
                    //    percOrAmount = request.percentOrAmount,
                    //    value = request.ujbsharevalue
                    //},
                    //shareRecievedByPartners = request.shareReceivedByPartners,
                    Created = new Created
                    {
                        created_By = request.referredById,
                        created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                    },
                    Updated = new Updated(),
                    refStatusUpdatedOn = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)),
                    dealStatusUpdatedOn = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                    //referralStatus=0,
                };
                _lead.InsertOne(lead);

                var productsOrServices = _productsDetails.Find(x => x.prodservId == request.selectedProductId && x.isActive).Project(x => new ProductServiceDetails
                {

                    productName = x.productName,
                    Id = x.Id,
                    from = x.from,
                    to = x.to,
                    isActive = x.isActive,
                    type = x.type,
                    value = x.value,

                }).ToList();
                foreach (var productsde in productsOrServices)
                {
                    var ps = new LeadProductServiceDetails
                    {
                        prodservdetailId = productsde.Id,
                        LeadId = lead.Id,
                        from = productsde.from,
                        to = productsde.to,
                        prodservId = request.selectedProductId,
                        productName = productsde.productName,
                        type = (int)productsde.type,
                        value = (int)productsde.value,
                        isActive = true,
                        created = new Created
                        {
                            created_By = request.referredById,
                            created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                        },
                        updated = new Updated()
                    };

                    _leadProductsDetails.InsertOne(ps);
                }

                return lead.Id;
            }
            else
            {
                return "Inactive User";
            }
        }

        public Get_Request Get_Referral_Details(string referralId)
        {
            var response = new Get_Request();
            var productid = _lead.Find(x => x.Id == referralId).FirstOrDefault().referredProductORServicesId;

            var businessId = _productsAndService.Find(x => x.Id == productid).FirstOrDefault().bussinessId;

            var categoriIds = _businessDetails.Find(x => x.Id == businessId).FirstOrDefault().Categories;

            var categoryNames = new List<string>();
            if (categoriIds != null)
            {
                categoryNames = _categories.Find(x => categoriIds.Contains(x.Id)).Project(x => x.categoryName).ToList();
            }

            response = _lead.Find(x => x.Id == referralId).Project(x => new Get_Request
            {
                referralId = x.Id,
                categories = categoryNames,
                dateCreated = x.referralDate.Value.ToString("dd/MM/yyyy"),
                isForSelf = x.isForSelf,
                productId = x.referredProductORServicesId,
                productName = x.referredProductORServices,
                referralDescription = x.referralDescription,
                refMultisSlabProdId = x.refMultisSlabProdId,
                //    isAccepted = x.isAccepted,
                rejectedReason = x.rejectionReason,
                dealValue = x.dealValue.ToString(),
                businessId = x.referredBusinessId,
                referredToDetails = x.referredTo,
                refStatus = (int)x.referralStatus,
                dealStatus = (int)x.dealStatus,
                shareReceivedByUJB = x.shareReceivedByUJB,
                ProjectiveshareReceivedByPartner = x.shareRecievedByPartners,
                ReferralCode = x.ReferralCode,
                referralStatusUpdatedby = x.referralStatusUpdatedby,
                referralStatusUpdatedOn = x.dealStatusUpdatedOn.ToString("dd/MM/yyyy"),
                referralcreatedDate = x.referralDate ?? DateTime.Now
            }).FirstOrDefault();

            var userId = _lead.Find(x => x.Id == referralId).Project(x => x.referredBy.userId).FirstOrDefault();

            var userDetails = _users.Find(x => x._id == userId).FirstOrDefault();


            // string MentorCode = _users.Find(x => x._id == ReferredById).FirstOrDefault().mentorCode.ToString();
            var mentorID = _users.Find(x => x.myMentorCode == userDetails.mentorCode).FirstOrDefault()._id.ToString();
            var user_id = _businessDetails.Find(x => x.Id == response.businessId).Project(x => x.UserId).FirstOrDefault();
            var LPmentorCode = _users.Find(x => x._id == user_id).Project(x => x.mentorCode).FirstOrDefault();

            var LPMentorID = _users.Find(x => x.myMentorCode == LPmentorCode).Project(x => x._id).FirstOrDefault(); ;
            if (_payments.Find(x => x.leadId == referralId).CountDocuments() > 0)
            {
                var AmtReceived = _payments.AsQueryable()
                            .Where(x => x.leadId == referralId && x.PayType == 1)
                            .GroupBy(d => d.leadId)
                            .Select(
                             g => new
                             {
                                 Value = g.Sum(s => s.CPReceivedAmt),
                                 Value1 = g.Sum(x => x.Amount)
                             }).FirstOrDefault();
                if (AmtReceived != null)
                {
                    response.TotalCPAmtReceived = AmtReceived.Value;
                    response.TotalUJBAmtReceived = AmtReceived.Value1;
                }

                response.PartnerId = userId;
                response.MentorId = mentorID;
                response.LPMentorId = LPMentorID;
                response.shareReceivedByPartner.mentorID = mentorID;
                response.shareReceivedByPartner.partnerID = userId;
                response.shareReceivedByPartner.LPmentorID = LPMentorID;
                List<ShareRecievedByPartners> share = new List<ShareRecievedByPartners>();
                share = _payments.Find(x => x.leadId == referralId && x.paymentTo.Contains("UJB")).Project(x => x.ShareRecvdByPartners).ToList();


                response.shareReceivedByPartner.RecievedByReferral = share.Sum(x => x.RecievedByReferral);
                response.shareReceivedByPartner.RecievedByMentor = share.Sum(x => x.RecievedByMentor);
                response.shareReceivedByPartner.RecievedByUJB = share.Sum(x => x.RecievedByUJB);
                response.shareReceivedByPartner.RecievedByLPMentor = share.Sum(x => x.RecievedByLPMentor);
                var CPAmtPaid = _payments.AsQueryable()
                            .Where(x => x.leadId == referralId && x.PayType == 2 && x.paymentTo.Contains(response.PartnerId))
                            .GroupBy(d => d.leadId)
                            .Select(
                             g => new
                             {
                                 Value = g.Sum(s => s.Amount),

                             }).FirstOrDefault();
                if (CPAmtPaid != null)
                {
                    response.TotalPartnerAmtPaid = CPAmtPaid.Value;
                }

                var MentorAmtPaid = _payments.AsQueryable()
                            .Where(x => x.leadId == referralId && x.PayType == 2 && x.paymentTo.Contains(response.MentorId))
                            .GroupBy(d => d.leadId)
                            .Select(
                             g => new
                             {
                                 Value = g.Sum(s => s.Amount),
                             }).FirstOrDefault();
                if (MentorAmtPaid != null)
                {
                    response.TotalMentorAmtPaid = MentorAmtPaid.Value;
                }

                var LPMentorAmtPaid = _payments.AsQueryable()
                            .Where(x => x.leadId == referralId && x.PayType == 2 && x.paymentTo.Contains(response.LPMentorId))
                            .GroupBy(d => d.leadId)
                            .Select(
                             g => new
                             {
                                 Value = g.Sum(s => s.Amount),
                             }).FirstOrDefault();
                if (LPMentorAmtPaid != null)
                {
                    response.TotalLPMentorAmtPaid = LPMentorAmtPaid.Value;
                }
            }

            response.referralStatusValue = Enum.GetName(typeof(ReferralStatusEnum), response.refStatus);
            var dealstatusvalue = _dealStatus.Find(x => x.StatusId == response.dealStatus).Project(x => x.StatusName).FirstOrDefault();
            response.dealStatusValue = dealstatusvalue;
            //Calculate Share value if type is percentage

            if (response != null)
            {
                if (response.shareReceivedByUJB != null)
                {
                    if (response.shareReceivedByUJB.percOrAmount == 1)
                    {
                        response.CalcDealValue = ((double.Parse(response.dealValue) * response.shareReceivedByUJB.value) / 100);
                    }
                }
            }


            response.referredByDetails = new ReferredByDetails
            {
                referredByName = userDetails.firstName + " " + userDetails.lastName,
                referredByEmailId = userDetails.emailId,
                referredByMobileNo = userDetails.mobileNumber,
                referredByCountryCode = userDetails.countryCode

            };
            if (response.isForSelf == true)
            {
                response.referredToDetails = new ReferredTo
                {
                    name = userDetails.firstName + " " + userDetails.lastName,
                    emailId = userDetails.emailId,
                    mobileNumber = userDetails.mobileNumber,
                    countryCode = userDetails.countryCode
                };
            }


            var mobileNumber = _users.Find(y => y._id == user_id).Project(y => y.mobileNumber).FirstOrDefault();
            var countryCode = _users.Find(y => y._id == user_id).Project(y => y.countryCode).FirstOrDefault();
            var UserName = _users.Find(y => y._id == user_id).Project(y => y.firstName + " " + y.lastName).FirstOrDefault();
            response.clientPartnerDetails = _businessDetails.Find(x => x.Id == response.businessId).Project(x => new ClientPartnerDetails
            {
                name = UserName,
                tagline = x.Tagline,
                emailId = x.BusinessEmail,
                mobileNumber = mobileNumber,
                CountryCode = countryCode,
                UserId = user_id,
                companyName = x.CompanyName
            }).FirstOrDefault();

            response.StatusHistories = _leadStatusHistory.Find(x => x.leadId == response.referralId).Project(x => new ReferralStatusHistory
            {
                //date =DateTime.Parse(x.Updated.updated_On.Value.ToString("dd/MM/yyyy hh:mm")),
                date = x.Updated.updated_On.Value,
                statusCode = x.statusId,
                // status = ((ReferralStatusEnum)x.statusId).ToString()
            }).ToList();
            foreach (var item in response.StatusHistories)
            {
                item.status = _dealStatus.Find(x => x.StatusId == item.statusCode).FirstOrDefault().StatusName;
            }

            var refferedById = _lead.Find(x => x.Id == referralId).Project(x => x.referredBy.userId).FirstOrDefault();

            var mentorCode = _users.Find(x => x._id == refferedById).Project(x => x.mentorCode).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(mentorCode))
            {
                response.mentorDetails = new MentorDetails();
            }
            else
            {
                response.mentorDetails = _users.Find(x => x.myMentorCode == mentorCode).Project(x => new MentorDetails
                {
                    id = x._id,
                    name = x.firstName + " " + x.lastName,
                    email = x.emailId,
                    mobile = x.mobileNumber,
                    CountryCode = x.countryCode
                }).FirstOrDefault();
            }


            MentorDetails LPmentorDetails = new MentorDetails();
            LPmentorDetails = _users.Find(x => x.myMentorCode == LPmentorCode).Project(x => new MentorDetails
            {
                id = x._id,
                name = x.firstName + " " + x.lastName,
                email = x.emailId,
                mobile = x.mobileNumber,
                CountryCode = x.countryCode
            }).FirstOrDefault();
            response.LPMentorDetails = LPmentorDetails;
            response.productDetails = _productsAndService.Find(x => x.Id == response.productId).Project(x => new ProductDetails
            {
                id = x.Id,
                name = x.name,
                price = x.productPrice,
                shareType = x.shareType
            }).FirstOrDefault();



            var filter = Builders<ReferralAgreedPercentage>.Filter.Lte(x => x.EffectiveStartDate, DateTime.Parse(response.referralcreatedDate.ToShortDateString()));
            filter = filter & Builders<ReferralAgreedPercentage>.Filter.Gte(x => x.EffectiveEndDate, DateTime.Parse(response.referralcreatedDate.ToShortDateString()));
            // DateTime.Parse(response.referralcreatedDate.ToString())
            filter = filter & Builders<ReferralAgreedPercentage>.Filter.Where(x => x.isActive);
            //var filter = Builders<ReferralAgreedPercentage>.Filter.Where(x => x.isActive);
            //DateTime.Parse(response.referralcreatedDate.ToShortDateString())                                                                                                                    //filter = Builders<ReferralAgreedPercentage>.Filter.Where(x => x.isActive);

            List<ReferralAgreedPercentage> referralperList = _ReferralAgreedPercentage.Find(filter).ToList();
            foreach (var ReferralAgreedPercentage in referralperList)
            {
                SharedPercentageDetails SharedPercentageDetail = new SharedPercentageDetails();
                if (ReferralAgreedPercentage.transferTo == 1)
                {
                    SharedPercentageDetail.ID = "UJB";
                    SharedPercentageDetail.Name = "UJB";

                }
                else if (ReferralAgreedPercentage.transferTo == 2)
                {
                    SharedPercentageDetail.ID = refferedById;
                    SharedPercentageDetail.Name = response.referredByDetails.referredByName + " (Partner)";

                }
                else if (ReferralAgreedPercentage.transferTo == 3)
                {
                    SharedPercentageDetail.ID = response.mentorDetails.id;
                    SharedPercentageDetail.Name = response.mentorDetails.name + " (Partner Mentor)";

                }
                else
                {
                    SharedPercentageDetail.ID = LPmentorDetails.id;
                    SharedPercentageDetail.Name = LPmentorDetails.name + " (LP Mentor)"; ;

                }
                SharedPercentageDetail.percentage = ReferralAgreedPercentage.Percentage;
                SharedPercentageDetail.isActive = ReferralAgreedPercentage.isActive;
                SharedPercentageDetail.sharedId = ReferralAgreedPercentage.transferTo;
                response.sharedPercentageDetails.Add(SharedPercentageDetail);

            }

            var LPMentorPAmtPaid = _payments.AsQueryable()
                       .Where(x => x.leadId == referralId && x.PayType == 2 && x.paymentTo.Contains(response.LPMentorId))
                       .GroupBy(d => d.leadId)
                       .Select(
                        g => new
                        {
                            Value = g.Sum(s => s.amtRecvdFrmPramotion),
                        }).FirstOrDefault();

            var MentorPAmtPaid = _payments.AsQueryable()
                      .Where(x => x.leadId == referralId && x.PayType == 2 && x.paymentTo.Contains(response.MentorId))
                      .GroupBy(d => d.leadId)
                      .Select(
                       g => new
                       {
                           Value = g.Sum(s => s.amtRecvdFrmPramotion),
                       }).FirstOrDefault();

            var PartenrPAmtPaid = _payments.AsQueryable()
                      .Where(x => x.leadId == referralId && x.PayType == 2 && x.paymentTo.Contains(response.PartnerId))
                      .GroupBy(d => d.leadId)
                      .Select(
                       g => new
                       {
                           Value = g.Sum(s => s.amtRecvdFrmPramotion),
                       }).FirstOrDefault();
            response.PramotionalshareRecievedByPartners = new ShareRecievedByPartners();
            response.PramotionalshareRecievedByPartners.partnerID = response.PartnerId;
            if (PartenrPAmtPaid != null)
            {
                response.PramotionalshareRecievedByPartners.RecievedByReferral = PartenrPAmtPaid.Value;
            }
            else
            {
                response.PramotionalshareRecievedByPartners.RecievedByReferral = 0.0;
            }
            response.PramotionalshareRecievedByPartners.mentorID = response.PartnerId;
            response.PramotionalshareRecievedByPartners.LPmentorID = response.LPMentorId;
            if (MentorPAmtPaid != null)
            {
                response.PramotionalshareRecievedByPartners.RecievedByMentor = MentorPAmtPaid.Value;
            }
            else
            {
                response.PramotionalshareRecievedByPartners.RecievedByMentor = 0.0;
            }
            if (LPMentorPAmtPaid != null)
            {
                response.PramotionalshareRecievedByPartners.RecievedByLPMentor = LPMentorPAmtPaid.Value;
            }
            else
            {
                response.PramotionalshareRecievedByPartners.RecievedByLPMentor = 0.0;
            }

            var UJBPaid = _payments.AsQueryable()
                    .Where(x => x.leadId == referralId)
                    .GroupBy(d => d.leadId)
                    .Select(
                     g => new
                     {
                         Value = g.Sum(s => s.amtRecvdbyUJB),
                     }).FirstOrDefault();

            response.TotalshareRecievedByPartners = new ShareRecievedByPartners();
            response.TotalshareRecievedByPartners.partnerID = response.PartnerId;
            response.TotalshareRecievedByPartners.RecievedByReferral = response.TotalPartnerAmtPaid + response.PramotionalshareRecievedByPartners.RecievedByReferral;
            response.TotalshareRecievedByPartners.mentorID = response.PartnerId;
            response.TotalshareRecievedByPartners.LPmentorID = response.LPMentorId;
            response.TotalshareRecievedByPartners.RecievedByMentor = response.TotalMentorAmtPaid + response.PramotionalshareRecievedByPartners.RecievedByMentor;
            response.TotalshareRecievedByPartners.RecievedByLPMentor = response.TotalLPMentorAmtPaid + response.PramotionalshareRecievedByPartners.RecievedByLPMentor;
            if (UJBPaid != null)
            {
                response.TotalshareRecievedByPartners.RecievedByUJB = UJBPaid.Value;
            }
            else
            {
                response.TotalshareRecievedByPartners.RecievedByUJB = 0.0;
            }


            var refactive = _users.Find(x => !x.isActive && x._id == userId).CountDocuments();
            var refMactive = _users.Find(x => !x.isActive && x._id == response.MentorId).CountDocuments();
            var LPactive = _users.Find(x => !x.isActive && x._id == user_id).CountDocuments();
            var LPMactive = _users.Find(x => !x.isActive && x._id == response.LPMentorId).CountDocuments();
            if (refactive > 0 || LPactive > 0 || refMactive > 0 || LPMactive > 0)
            {
                response.inActiveUsers = "One or more partner(s) involved in this referral are inactive now.";
            }



            return response;

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

        public List<Get_Request> Search_For_Referrals(Lookup_Request request)
        {
            var categoryNames = new List<string>();

            var response = new List<Get_Request>();

            request.query = request.query.ToLower();

            var res = Builders<Leads>.Filter.In(x => x.referralStatus, request.status)
               //& (Builders<Leads>.Filter.In(x => x.referredProductORServicesId, productIds)
               /// | Builders<Leads>.Filter.Eq(x => x.referredBy.name, request.query)
               // | Builders<Leads>.Filter.Eq(x => x.referredTo.name, request.query)
               ;
            //var res=Builders<Leads>.Filter.Eq(x => x.referredBy.userId, request.userId);

            if (request.referred == "byMe")
            {
                //if (request.status.Count == 0)
                //{
                //    request.status.Add(11);
                //}

                //if (request.status.Contains(11))
                //{
                //    res = Builders<Leads>.Filter.Eq(x => x.referredBy.userId, request.userId);
                //}
                if (request.status.Count == 0)
                {
                    res = Builders<Leads>.Filter.Eq(x => x.referredBy.userId, request.userId);
                }
                else
                {
                    res = Builders<Leads>.Filter.In(x => x.dealStatus, request.status)
                    & Builders<Leads>.Filter.Eq(x => x.referredBy.userId, request.userId);
                }
                //& Builders<Leads>.Filter.In(x => x.referredProductORServicesId, productIds)
                //& (Builders<Leads>.Filter.Eq(x => x.referredBy.name, request.query)
                //| Builders<Leads>.Filter.Eq(x => x.referredTo.name, request.query))

            }
            else if (request.referred == "toMe")
            {
                //if (request.status.Count == 0)
                //{
                //    request.status.Add(11);
                //}
                //if (request.status.Contains(11))
                //{
                //    /// res = Builders<Leads>.Filter.Eq(x => x.referredBy.userId, request.userId);
                //    var businessIdList = _businessDetails.Find(x => x.UserId == request.userId).Project(x => x.Id).ToList();
                //    res = Builders<Leads>.Filter.In(x => x.referredBusinessId, businessIdList);
                //}
                var businessIdList = _businessDetails.Find(x => x.UserId == request.userId).Project(x => x.Id).ToList();
                if (request.status.Count == 0)
                {
                    /// res = Builders<Leads>.Filter.Eq(x => x.referredBy.userId, request.userId);

                    res = Builders<Leads>.Filter.In(x => x.referredBusinessId, businessIdList);
                }
                else
                {
                    //var businessIdList = _businessDetails.Find(x => x.UserId == request.userId).Project(x => x.Id).ToList();
                    res = Builders<Leads>.Filter.In(x => x.dealStatus, request.status)
                    & Builders<Leads>.Filter.In(x => x.referredBusinessId, businessIdList);
                }
                //& (Builders<Leads>.Filter.Eq(x => x.referredBy.name, request.query)
                //& Builders<Leads>.Filter.In(x => x.referredProductORServicesId, productIds)
                //| Builders<Leads>.Filter.Eq(x => x.referredTo.name, request.query)
                //)


            }

            //else if(request.referred == "all")
            //{
            //    response = _lead.Find(x => x.referredBy.userId == request.userId || );
            //}

            response.AddRange(_lead.Find(res).SortBy(x => x.referralStatus).ThenByDescending(x => x.Created.created_On).Project(x => new Get_Request
            {
                referralId = x.Id,
                categories = categoryNames,
                dateCreated = x.referralDate.Value.ToString("dd/MM/yyyy"),
                isForSelf = x.isForSelf,
                productId = x.referredProductORServicesId,
                productName = x.referredProductORServices,
                referralDescription = x.referralDescription,
                businessId = x.referredBusinessId,
                referredToDetails = x.referredTo,
                refStatus = (int)x.referralStatus,
                dealStatus = (int)x.dealStatus,
                referralStatusUpdatedOn = x.refStatusUpdatedOn.ToString("dd/MM/yyyy"),
                DealStatusUpdatedOn = x.dealStatusUpdatedOn.ToString("dd/MM/yyyy"),
                ReferralCode = x.ReferralCode,
                //.referralStatusUpdatedOn
                // referralStatusValue = (string)Enum.GetName(typeof(ReferralStatusEnum), x.referralStatus),
                //   dealStatusValue= Enum.GetName(typeof(ReferralStatusEnum), x.referralStatus),
                // isAccepted = x.referralStatus
            }).ToList());



            foreach (var r in response)
            {

                r.referralStatusValue = Enum.GetName(typeof(ReferralStatusEnum), r.refStatus);
                if (r.dealStatus != 0)
                {
                    var dealstatusvalue = _dealStatus.Find(x => x.StatusId == r.dealStatus).Project(x => x.StatusName).FirstOrDefault();

                    r.dealStatusValue = dealstatusvalue;
                }
                var userId = _lead.Find(x => x.Id == r.referralId).Project(x => x.referredBy.userId).FirstOrDefault();
                var userDetails = _users.Find(x => x._id == userId).FirstOrDefault();
                r.referredByDetails = new ReferredByDetails
                {
                    referredByName = userDetails.firstName + " " + userDetails.lastName,
                    referredByEmailId = userDetails.emailId,
                    referredByMobileNo = userDetails.mobileNumber,
                    referredByCountryCode = userDetails.countryCode,
                    referredByUserId = userDetails._id,
                    referredByRole = userDetails.Role

                };

                if (r.isForSelf)
                {
                    r.referredToDetails = new ReferredTo
                    {
                        name = userDetails.firstName + " " + userDetails.lastName,
                        emailId = userDetails.emailId,
                        mobileNumber = userDetails.mobileNumber,
                        countryCode = userDetails.countryCode

                    };
                }
                //else
                //{
                //    r.referredToDetails = r.referredToDetails;
                //}

                var user_id = _businessDetails.Find(x => x.Id == r.businessId).Project(x => x.UserId).FirstOrDefault();
                var mobileNumber = _users.Find(y => y._id == user_id).Project(y => y.mobileNumber).FirstOrDefault();
                var countryCode = _users.Find(y => y._id == user_id).Project(y => y.countryCode).FirstOrDefault();
                var UserName = _users.Find(y => y._id == user_id).Project(y => y.firstName + " " + y.lastName).FirstOrDefault();
                if (_businessDetails.Find(x => x.Id == r.businessId).CountDocuments() != 0)
                {
                    r.clientPartnerDetails = _businessDetails.Find(x => x.Id == r.businessId).Project(x => new ClientPartnerDetails
                    {
                        name = x.CompanyName,
                        tagline = x.Tagline,
                        emailId = x.BusinessEmail,
                        mobileNumber = mobileNumber,
                        CountryCode = countryCode,
                        UserId = user_id
                    }).FirstOrDefault();
                    if (r.clientPartnerDetails.name == "")
                    {
                        r.clientPartnerDetails.name = UserName;
                    }
                }

                //r.StatusHistories = _leadStatusHistory.Find(x => x.leadId == r.referralId).SortByDescending(x => x.Updated.updated_On).Project(x => new ReferralStatusHistory
                //{
                //    // date = DateTime.Parse(x.Updated.updated_On.Value.ToString("dd/MM/yyyy hh:mm")),
                //    date = x.Updated.updated_On.Value,
                //    statusCode = x.statusId,
                //    status = ((DealStatusEnum)x.statusId).ToString()
                //}).ToList();

                //foreach (var rs in r.StatusHistories)
                //{
                //    rs.status = _dealStatus.Find(x => x.StatusId == rs.statusCode).Project(x => x.StatusName).FirstOrDefault();
                //}
                //if (r.StatusHistories.Count > 0)
                //{
                //    r.referralStatusUpdatedOn = r.StatusHistories.Select(x => x.date).FirstOrDefault().ToString("dd/MM/yyyy");
                //}

                if (r.dealStatus > 0)
                {
                    r.referralStatusUpdatedOn = r.DealStatusUpdatedOn;
                }
                var businessId = _productsAndService.Find(x => x.Id == r.productId).FirstOrDefault().bussinessId;

                var categoriIds = _businessDetails.Find(x => x.Id == businessId).FirstOrDefault().Categories;

                categoryNames = new List<string>();
                if (categoriIds != null)
                {
                    categoryNames = _categories.Find(x => categoriIds.Contains(x.Id)).Project(x => x.categoryName).ToList();
                }
                r.categories = categoriIds;
            }

            var list = new List<Get_Request>();

            if (request.categoryIds != null && request.categoryIds.Count != 0)
            {
                foreach (var r in response)
                {
                    foreach (var c in request.categoryIds)
                    {
                        if (r.categories.Contains(c) && !string.IsNullOrWhiteSpace(c))
                        {
                            int index = list.FindIndex(f => f.businessId == r.businessId);
                            if (index < 0)
                            {
                                list.Add(r);
                            }
                        }
                    }
                }
            }
            else
            {
                list.AddRange(response);
            }

            if (!string.IsNullOrWhiteSpace(request.query))
            {
                response = list.Where(x =>
                x.referredByDetails.referredByName.ToLower().Contains(request.query)
                || x.referredToDetails.name.ToLower().Contains(request.query)
                || x.clientPartnerDetails.name != null ? x.clientPartnerDetails.name.ToLower().Contains(request.query) : false).ToList();
            }
            else
            {
                response = list;
            }

            foreach (var r in response)
            {
                categoryNames = _categories.Find(x => r.categories.Contains(x.Id)).Project(x => x.categoryName).ToList();
                r.categories = categoryNames;
                var user_id = _businessDetails.Find(x => x.Id == r.businessId).Project(x => x.UserId).FirstOrDefault();
                var UserName = _users.Find(y => y._id == user_id).Project(y => y.firstName + " " + y.lastName).FirstOrDefault();
                if (UserName != null && (string.IsNullOrWhiteSpace(r.clientPartnerDetails.name) || r.clientPartnerDetails.name == ""))
                {
                    r.clientPartnerDetails.name = UserName;
                }
            }

            return response;
        }
        public double GetSharedPercentage(string leadId, int sharedwithid)
        {

            DateTime referraldate = _lead.Find(x => x.Id == leadId).FirstOrDefault().referralDate ?? DateTime.Now;


            //  var filter = Builders<ReferralAgreedPercentage>.Filter.Eq(x => x.isActive, true); //Builders<ReferralAgreedPercentage>.Filter.Lte(x => x.EffectiveStartDate, DateTime.Parse(referraldate.ToString("yyyy-MM-dd")));
            var filter = Builders<ReferralAgreedPercentage>.Filter.Lte(x => x.EffectiveStartDate, DateTime.Parse(referraldate.ToShortDateString()));
            filter = filter & Builders<ReferralAgreedPercentage>.Filter.Gte(x => x.EffectiveEndDate, DateTime.Parse(referraldate.ToShortDateString()));
            // filter = filter & Builders<ReferralAgreedPercentage>.Filter.Gte(x => x.EffectiveEndDate, DateTime.Parse(referraldate.ToString("yyyy-MM-dd")));
            filter = filter & Builders<ReferralAgreedPercentage>.Filter.Eq(x => x.transferTo, sharedwithid);

            return _ReferralAgreedPercentage.Find(filter).FirstOrDefault().Percentage;
        }
        public void Update_Referral_Rejection_Status(Put_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            _lead.FindOneAndUpdate(
                Builders<Leads>.Filter.Eq(x => x.Id, request.dealId),
                Builders<Leads>.Update
                .Set(x => x.referralStatus, request.referralStatus)
                .Set(x => x.rejectionReason, request.rejectionReason)
                .Set(x => x.Updated, new Updated
                {
                    updated_By = request.userId,
                    updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                })
                .Set(x => x.refStatusUpdatedOn, DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)))
                .Set(x => x.dealStatusUpdatedOn, DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)))
                .Set(x => x.referralStatusUpdatedby, request.userId)
                )
                ;

            if (request.referralStatus == 1)
            {
                var dealStatusId = _dealStatus.Find(x => x.StatusName == "Not Connected").Project(x => x.StatusId).FirstOrDefault();
                _lead.FindOneAndUpdate(
                    Builders<Leads>.Filter.Eq(x => x.Id, request.dealId),
                    Builders<Leads>.Update
                    .Set(x => x.dealStatus, dealStatusId)
                    .Set(x => x.dealStatusUpdatedOn, DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)))
                    );

                var lsh = new LeadsStatusHistory
                {
                    leadId = request.dealId,
                    statusId = dealStatusId,
                    Updated = new Updated
                    {
                        updated_By = request.userId,
                        updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                    }
                };

                _leadStatusHistory.InsertOne(lsh);
            }
        }

        public LeadProductInfo Get_Product_Service_Details(string LeadId)
        {
            var res = new LeadProductInfo();

            var lead_detailss = _lead.Find(x => x.Id == LeadId).Project(x => new Leads
            {
                shareType = x.shareType,
                typeOf = x.typeOf
            }).FirstOrDefault();
            res.typeOf = lead_detailss.typeOf;
            res.shareType = lead_detailss.shareType;
            res.productsOrServices = _leadProductsDetails.Find(x => x.LeadId == LeadId && x.isActive).Project(x => new LeadProductServiceDetails
            {
                LeadId = x.LeadId,
                prodservId = x.prodservId,
                productName = x.productName,
                prodservdetailId = x.prodservdetailId,
                from = x.from,
                to = x.to,
                isActive = x.isActive,
                type = x.type,
                value = x.value,

            }).ToList();

            return res;
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
