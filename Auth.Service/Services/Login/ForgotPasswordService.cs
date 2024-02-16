using Auth.Service.Models.Admin.ForgotPassword;
using Auth.Service.Models.ForgotPassword;
using Auth.Service.Respositories.Login;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Linq;
using UJBHelper.DataModel;

namespace Auth.Service.Services.Login
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<AdminUser> _adminusers;
        private IConfiguration _iconfiguration;
        public ForgotPasswordService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            //var u = new User();
            _users = database.GetCollection<User>("Users");
            _adminusers = database.GetCollection<AdminUser>("AdminUsers");
        }

        public void Create_New_Password(string user_id, string new_password)
        {
            _users.FindOneAndUpdate(
               Builders<User>.Filter.Eq("_id", user_id),
               Builders<User>.Update.Set(x=>x.password, new_password)
                );
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

        public string Verify_User(string username)
        {
            if (_users.Find(x => x.emailId == username || x.mobileNumber == username).CountDocuments() > 0)
            {
                return _users.Find(x => x.emailId == username || x.mobileNumber == username).FirstOrDefault()._id;
            }
            else
            {
                return "";
            }
           // return _users.Find(x => x.emailId == username || x.mobileNumber == username).FirstOrDefault()._id;
        }

        public string Verify_Admin_User(string username)
        {
            if (_adminusers.Find(x => x.emailId == username || x.mobileNumber == username).CountDocuments() > 0)
            {
                return _adminusers.Find(x => x.emailId == username || x.mobileNumber == username).FirstOrDefault()._id;
            }
            else
            {
                return "";
            }
            //return _adminusers.Find(x => x.emailId == username || x.mobileNumber == username).FirstOrDefault()._id;
        }
        public void Create_New_Admin_Password(string user_id, string new_password)
        {
            _adminusers.FindOneAndUpdate(
               Builders<AdminUser>.Filter.Eq("_id", user_id),
               Builders<AdminUser>.Update.Set(x => x.password, new_password)
                );
        }

        public Get_Admin_Request Get_Admin_User_Details(string user_id)
        {
            var filter = Builders<AdminUser>.Filter.Eq("_id", user_id);

            return _adminusers.Find(filter).Project(x => new Get_Admin_Request
            {
                _id = x._id,
                emailId = x.emailId,
                Firstname = x.firstName,                
                mobileNumber = x.mobileNumber,
                countryCode = x.countryCode
            }).FirstOrDefault();

        }


    }
}
