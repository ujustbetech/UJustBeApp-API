using Auth.Service.Manager.Registeration.Otp;
using Auth.Service.Models.Registeration.Otp;
using Auth.Service.Respositories.Registeration;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using UJBHelper.DataModel;

namespace Auth.Service.Services.Registeration
{
    public class OtpService : IOtpService
    {
        private readonly IMongoCollection<User> _users;
        private IConfiguration _iconfiguration;
        public OtpService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _users = database.GetCollection<User>("Users");
        }

        public void UpdateMobileNo(string UserId,string MobileNo,string countryCode)
        {
            _users.FindOneAndUpdate(
                Builders<User>.Filter.Eq(x => x._id, UserId),
                Builders<User>.Update.Set(x => x.mobileNumber, MobileNo)
                .Set(x => x.countryCode, countryCode));
        }

        public bool Check_If_User_Exists(string userId)
        {
            return _users.Find(x => x._id == userId).CountDocuments() > 0;
        }

        public User_Contact_Details Get_User_Details(string userId)
        {
            return _users.Find(x => x._id == userId).Project(x => new User_Contact_Details
            {
                MobileNo = x.countryCode.Substring(1) + x.mobileNumber,
                EmailId = x.emailId,
                FirstName = x.firstName,
                LastName = x.lastName
            }).FirstOrDefault();
        }

        public void Update_Otp(string userId, string otp)
        {
            _users.FindOneAndUpdate(Builders<User>.Filter.Eq(x => x._id, userId), Builders<User>.Update.Set(x => x.otpVerification.OTP, otp));
        }

        public void Update_Otp_Flag(Post_Request request)
        {
            _users.FindOneAndUpdate(Builders<User>.Filter.Eq(x => x._id, request.userId), Builders<User>.Update.Set(x => x.otpVerification.OTP_Verified, request.otpValidationFlag));
        }
    }
}
