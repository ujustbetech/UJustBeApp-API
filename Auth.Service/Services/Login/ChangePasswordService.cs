using Auth.Service.Models.ChangePassword;
using Auth.Service.Respositories.Login;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Linq;
using UJBHelper.DataModel;

namespace Auth.Service.Services.Login
{
    public class ChangePasswordService : IChangePasswordService
    {
        private readonly IMongoCollection<User> _users;
        private IConfiguration _iconfiguration;
        public ChangePasswordService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _users = database.GetCollection<User>("Users");
        }

        public void Create_New_Password(string user_id, string new_password)
        {
            _users.FindOneAndUpdate(
               Builders<User>.Filter.Eq("_id", user_id),
               Builders<User>.Update.Set(x=>x.password, new_password)
                );
        }

        public string Get_Password(string user_Id)
        {
            return _users.Find(x => x._id == user_Id).Project(x => x.password).FirstOrDefault();
        }

        public Get_Request Get_User_Details(string user_id)
        {
            var filter = Builders<User>.Filter.Eq("_id", user_id);
           
            return _users.Find(filter).Project(x => new Get_Request
            {
                _id = x._id,
                EmailId = x.emailId,
                Firstname = x.firstName,
                Is_Otp_Verified = x.otpVerification.OTP_Verified,
                Mobile_Number = x.mobileNumber,
                countryCode = x.countryCode
            }).FirstOrDefault();

        }

        public bool Verify_User(string user_id)
        {
            return _users.Find(x => x._id == user_id).CountDocuments() > 0;
        }
    }
}
