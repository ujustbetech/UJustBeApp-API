using Business.Service.Models.ApproveBusinessKYC;
using Business.Service.Repositories.ApproveBusinessKYC;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UJBHelper.Common;
using UJBHelper.DataModel;
using Wkhtmltopdf.NetCore;
using static UJBHelper.Common.Common;

namespace Business.Service.Services.ApproveBusinessKYC
{
    public class ApproveBusinessKYCService : IApproveBusinessKYCService
    {
        private IConfiguration _iconfiguration;
        readonly IGeneratePdf _generatePdf;
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private readonly IMongoCollection<FeeStructure> _feeStructure;
        private readonly IMongoCollection<User> _userDetails;
        private readonly IMongoCollection<FeePaymentDetails> _Feepayment;
        private readonly IMongoCollection<SubscriptionDetails> _subscriptionDetails;
        private readonly IMongoCollection<Agreement> _agreement;
        private readonly IMongoCollection<AgreementDetails> _agreementDetails;

        public ApproveBusinessKYCService(IConfiguration config, IGeneratePdf generatePdf)
        {
            _iconfiguration = config;
            _generatePdf = generatePdf;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
            _userDetails = database.GetCollection<User>("Users");
            _subscriptionDetails = database.GetCollection<SubscriptionDetails>("SubscriptionDetails");
            _Feepayment = database.GetCollection<FeePaymentDetails>("FeePaymentDetails");
            _feeStructure = database.GetCollection<FeeStructure>("FeeStructure");
            _agreement = database.GetCollection<Agreement>("Agreement");
            _agreementDetails = database.GetCollection<AgreementDetails>("AgreementDetails");

        }

        public string GeneratePDF(BusinessDetails bsns)
        {
            // Get UserDetails
            User userdetails = _userDetails.Find(x => x._id == bsns.UserId).FirstOrDefault();
           
            string HTMlString = "";
           

            if (bsns.UserType == (int)UserType.Individual_Proprietor)
            {
                HTMlString =  _agreement.Find(x => x.Type == "Freelancer Listed Partner Agreement").FirstOrDefault().AgreementContent;
                HTMlString = HTMlString.Replace("@UserName", userdetails.firstName + " " + userdetails.lastName);
                HTMlString = HTMlString.Replace("@BusinessAddress", bsns.BusinessAddress.Flat_Wing.Trim()  + ", " + bsns.BusinessAddress.Location);
            }

            else if (bsns.UserType == (int)UserType.LLP)
            {
                HTMlString =  _agreement.Find(x => x.Type == "LLP Listed Partner Agreement").FirstOrDefault().AgreementContent;
                HTMlString = HTMlString.Replace("@CompanyName", bsns.CompanyName);
                HTMlString = HTMlString.Replace("@NameofPartner", bsns.NameOfPartner);
                HTMlString = HTMlString.Replace("@BusinessAddress", bsns.BusinessAddress.Flat_Wing.Trim() + ", " + bsns.BusinessAddress.Location.Trim());
            }
            else if (bsns.UserType == (int)UserType.PartnerShipFirm)
            {
                HTMlString =  _agreement.Find(x => x.Type == "PartnerShipFirm Listed Partner Agreement").FirstOrDefault().AgreementContent;
                HTMlString = HTMlString.Replace("@CompanyName", bsns.CompanyName);
                HTMlString = HTMlString.Replace("@NameofPartner", bsns.NameOfPartner);
                HTMlString = HTMlString.Replace("@BusinessAddress", bsns.BusinessAddress.Flat_Wing.Trim() + ", " + bsns.BusinessAddress.Location.Trim());
            }
            else if (bsns.UserType == (int)UserType.Company)
            {
                HTMlString = _agreement.Find(x => x.Type == "Company Listed Partner Agreement").FirstOrDefault().AgreementContent;
                HTMlString = HTMlString.Replace("@CompanyName", bsns.CompanyName);                
                HTMlString = HTMlString.Replace("@BusinessAddress", bsns.BusinessAddress.Flat_Wing.Trim() + ", " + bsns.BusinessAddress.Location.Trim());
            }

            String FileURL = "";
            string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
            FileDestination = FileDestination + _iconfiguration["ListedPartnerAgreementImagePath"];
            FileURL = _iconfiguration["ListedPartnerAgreementImageURL"];
            Byte[] bytes = _generatePdf.GetPDF(HTMlString);
            string fileUniqueName = Utility.UploadFilebytes(bytes, "ListedPartnerAgreement.pdf", FileDestination);
            FileURL = FileURL + fileUniqueName;
            return FileURL;
        }

        public void SaveAgreementDetails(string BusinessId, string UpdatedBy)
        {

            BusinessDetails bsns = _businessDetails.Find(x => x.Id == BusinessId).FirstOrDefault();

            string FileURL = GeneratePDF(bsns);

            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var p = new AgreementDetails
            {
                UserId = bsns.UserId,
                BusinessId = bsns.Id,
                type = "Listed Partner Agreement",
                Version = 1.0,
                PdfURL = FileURL,
                created = new Created
                {
                    created_By = UpdatedBy,
                    created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                },
                accepted = new Accepted()
            };
            _agreementDetails.InsertOne(p);
        }

        public bool Check_If_Business_Exists(string businessId)
        {
            return _businessDetails.Find(x => x.Id == businessId).CountDocuments() != 0;
        }

        public bool Check_If_SusbscriptionPaymentDone(string businessId)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            string UserId = _businessDetails.Find(x => x.Id == businessId).FirstOrDefault().UserId;
            int CountryId = _userDetails.Find(x => x._id == UserId).FirstOrDefault().countryId;

            var AmtPaid = Check_TotalPayment_Done(UserId, "5d5a4534339dce0154441aac");

            DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
           // CurrentDate = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day);

            var filter = Builders<FeeStructure>.Filter.Lte(x => x.FromDate, CurrentDate);
            filter = filter & Builders<FeeStructure>.Filter.Gte(x => x.ToDate, CurrentDate);
            filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.CountryId, CountryId);
            filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.FeeTypeId, "5d5a4534339dce0154441aac");
            double AmountToPay = _feeStructure.Find(filter).FirstOrDefault().Amount;
            if (AmountToPay > AmtPaid)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public double Check_TotalPayment_Done(string UserId, string FeeType)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
            //CurrentDate = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day);

            bool userExist = _Feepayment.Find(x => x.userId == UserId && x.feeType == FeeType
                                               && x.ConvertedPaymentDate <= CurrentDate).CountDocuments() > 0;

           // bool userExist = _Feepayment.Find(x => x.userId == UserId && x.feeType == FeeType
                                               //&& x.ConvertedPaymentDate <= DateTime.Parse(DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)).ToString("yyyy-MM-dd"))).CountDocuments() > 0;
            if (userExist)
            {
                var AmtPaid = _Feepayment.AsQueryable()
                         .Where(x => x.userId == UserId && x.feeType == FeeType && x.ConvertedPaymentDate <= CurrentDate)
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


        public void Update_KYC_Details(Put_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            _businessDetails.FindOneAndUpdate(
                Builders<BusinessDetails>.Filter.Eq(x => x.Id, request.businessId),
                Builders<BusinessDetails>.Update
                .Set(x => x.isApproved,
                new Approved
                {
                    ApprovedBy = request.updatedBy,
                    ApprovedOn = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)),
                    Flag = request.isApproved,
                    Reason = request.rejectedReason
                })
                .Set(x => x.isSubscriptionActive, request.isSubscriptionActive)
                );


            if (request.isApproved == 1)
            {
                SaveAgreementDetails(request.businessId, request.updatedBy);
            }
        }

        public void AddSubscriptionDetails(string BusinessId)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            string UserId = _businessDetails.Find(x => x.Id == BusinessId).FirstOrDefault().UserId;
            int CountryId = _userDetails.Find(x => x._id == UserId).FirstOrDefault().countryId;

            DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
            //CurrentDate = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day);
           
            //DateTime startDate = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
            //DateTime endDate = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)).AddYears(1);
            string FeeType = "5d5a4534339dce0154441aac";

            var filter = Builders<FeeStructure>.Filter.Lte(x => x.FromDate, CurrentDate);
            filter = filter & Builders<FeeStructure>.Filter.Gte(x => x.ToDate, CurrentDate);
            filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.CountryId, CountryId);
            filter = filter & Builders<FeeStructure>.Filter.Eq(x => x.FeeTypeId, FeeType);
            double Amount = _feeStructure.Find(filter).FirstOrDefault().Amount;

            var p = new SubscriptionDetails
            {
                userId = UserId,
                StartDate =DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)),
                EndDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)).AddYears(1).AddDays(-1),
                Amount = Amount,
                feeType = FeeType,
                Created = new Created
                {
                    created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                }
            };
            _subscriptionDetails.InsertOne(p);
        }
    }
}
