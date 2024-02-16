using Partner.Service.Models.ApproveDisapproveBPCP;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using Partner.Service.Repositories.ApproveDisapproveBPCP;
using UJBHelper.DataModel;
using Wkhtmltopdf.NetCore;
using UJBHelper.Common;

namespace Partner.Service.Services.ApproveDisapproveBPCP
{
    public class ApproveDisapproveBPCPService : IApproveDisapproveBPCPService
    {
        private readonly IMongoCollection<UserKYCDetails> _userKYCDetails;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Agreement> _agreement;
        private readonly IMongoCollection<AgreementDetails> _agreementDetails;
        private IConfiguration _iconfiguration;
        readonly IGeneratePdf _generatePdf;
        public ApproveDisapproveBPCPService(IConfiguration config, IGeneratePdf generatePdf)
        {
            _iconfiguration = config;
            _generatePdf = generatePdf;            
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _userKYCDetails = database.GetCollection<UserKYCDetails>("UserKYCDetails");
            _agreement = database.GetCollection<Agreement>("Agreement");
            _agreementDetails = database.GetCollection<AgreementDetails>("AgreementDetails");
            _users = database.GetCollection<User>("Users");
        }

        public string GeneratePDF(string UserId)
        {
            // Get UserDetails
            User userdetails = _users.Find(x => x._id == UserId).FirstOrDefault();
            string MentorName = _users.Find(x => x.myMentorCode == userdetails.mentorCode).FirstOrDefault().firstName + " " + _users.Find(x => x.myMentorCode == userdetails.mentorCode).FirstOrDefault().lastName;
            string HTMlString = "";
            HTMlString = _agreement.Find(x => x.Type == "Partner Agreement").FirstOrDefault().AgreementContent;
            HTMlString = HTMlString.Replace("@UserName", userdetails.firstName + " " + userdetails.lastName);
            HTMlString = HTMlString.Replace("@MentorName", MentorName);
            HTMlString = HTMlString.Replace("@Address", userdetails.address.flatWing.Trim() + ", " +userdetails.address.location.Trim()) ;

            String FileURL = "";
            string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
            FileDestination = FileDestination + _iconfiguration["PartnerAgreementImagePath"];
            FileURL = _iconfiguration["PartnerAgreementImageURL"];
            Byte[] bytes = _generatePdf.GetPDF(HTMlString);
            string fileUniqueName = Utility.UploadFilebytes(bytes, "PartnerAgreement.pdf", FileDestination);
            FileURL = FileURL + fileUniqueName;
            return FileURL; 
        }

        public void SaveAgreementDetails(string UserId,string UpdatedBy)
        {
            string FileURL = GeneratePDF(UserId);
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var p = new AgreementDetails
            {
                UserId = UserId,
                BusinessId=null,
                type="Partner Agreement",
                Version=1.0,
                PdfURL = FileURL,
                created=new Created
                {
                    created_By = UpdatedBy,
                    created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                },
                accepted = new Accepted()
            };
            _agreementDetails.InsertOne(p);

        }

      
        public void Approve_Disapprove_BPCP(Post_Request request,string mentorCode)
        {

            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            if (request.Flag == false)
            {
                _userKYCDetails.FindOneAndUpdate(
                    Builders<UserKYCDetails>.Filter.Eq(x => x.UserId, request.UserId),
                Builders<UserKYCDetails>.Update
                .Set(x => x.IsApproved, new IsApproved
                {
                    Flag = request.Flag,
                    Reason = request.reason,
                    ReasonId = request.reasonId,
                    ApprovedBy = request.Updated_by,
                    ApprovedOn = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                })
                );
            }
            else
            {
                _userKYCDetails.FindOneAndUpdate(
                   Builders<UserKYCDetails>.Filter.Eq(x => x.UserId, request.UserId),
               Builders<UserKYCDetails>.Update
               .Set(x => x.IsApproved, new IsApproved
               {
                   Flag = request.Flag,
                   ApprovedBy = request.Updated_by,
                   ApprovedOn = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
               })
               );

                SaveAgreementDetails(request.UserId, request.Updated_by);

               // _users.FindOneAndUpdate(
               //    Builders<User>.Filter.Eq(x => x._id, request.UserId),
               //Builders<User>.Update
               //.Set(x => x.myMentorCode, mentorCode)               
               //);
            }            
        }
    }
}
