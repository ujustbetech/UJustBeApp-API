using Auth.Service.Respositories.Login;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Linq;
using UJBHelper.DataModel;
using UJBHelper.Common;

namespace Auth.Service.Services.Login
{
    public class LoginService : ILoginService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private readonly IMongoCollection<AdminUser> _adminUsers;
        private IConfiguration _iconfiguration;


        public LoginService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _users = database.GetCollection<User>("Users");
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
            _adminUsers = database.GetCollection<AdminUser>("AdminUsers");
        }

        public string Get_Business_Id(string id)
        {
            return _businessDetails.Find(x => x.UserId == id).Project(x => x.Id).FirstOrDefault();
        }

        public AdminUser Get_Post_Login_Admin_Details(string userid)
        {
            return _adminUsers.Find(u => u._id == userid).FirstOrDefault();
        }

        public User Get_Post_Login_Details(string userid)
        {
            return _users.Find(u => u._id == userid).FirstOrDefault();
        }

        public string Verify_Admin_User(string username, string password)
        {
            if (_adminUsers.Find(user => (user.emailId.ToLower() == username.ToLower() || user.mobileNumber == username) && user.password == password && user.isActive==true).CountDocuments() > 0)
            {
                return _adminUsers.Find(user => (user.emailId.ToLower() == username.ToLower() || user.mobileNumber == username) && user.password == password && user.isActive == true).FirstOrDefault()._id;
            }
            return "";
        }

        public string Verify_User(string username, string password)
        {
            if (_users.Find(user => (user.emailId.ToLower() == username.ToLower() || user.mobileNumber == username) && user.password == password && user.isActive == true).CountDocuments() > 0)
            {
                return _users.Find(user => (user.emailId.ToLower() == username.ToLower() || user.mobileNumber == username) && user.password == password && user.isActive == true).FirstOrDefault()._id;
            }
            return "";
        }

        public string VerifyUser(string username, string password)
        {

            if (_users.Find(user => (user.emailId.ToLower() == username.ToLower() || user.mobileNumber == username)).CountDocuments() > 0)
            {
                if (_users.Find(user => (user.emailId.ToLower() == username.ToLower() || user.mobileNumber == username) && user.isActive == true).CountDocuments() > 0)
                {
                    string hashpassowrd = _users.Find(user => (user.emailId.ToLower() == username.ToLower() || user.mobileNumber == username)).FirstOrDefault().password;

                    if (Verify(password, hashpassowrd))
                    {
                        return _users.Find(user => (user.emailId.ToLower() == username.ToLower() || user.mobileNumber == username) && user.isActive == true).FirstOrDefault()._id;
                    }
                }
                else
                {
                    return "300";
                }
            }
            return "";
            

            // User user =Get_Post_Login_Details(string userid)
            //if (_users.Find(user => (user.emailId.ToLower() == username.ToLower() || user.mobileNumber == username)).CountDocuments() > 0)
            //{
            //    string hashpassowrd = _users.Find(user => (user.emailId.ToLower() == username.ToLower() || user.mobileNumber == username)).FirstOrDefault().password;

            //    if (Verify(password, hashpassowrd))
            //    {
            //        if( _users.Find(user => (user.emailId.ToLower() == username.ToLower() || user.mobileNumber == username) && user.isActive == true).CountDocuments()>0)
            //        {
            //            //return "200";
            //            return _users.Find(user => (user.emailId.ToLower() == username.ToLower() || user.mobileNumber == username)  && user.isActive == true).FirstOrDefault()._id;
            //        }
            //        else
            //        {
            //            return "300";
            //        }

            //    }

            //}
            //return "";
        }

        public bool Verify(string password, string hashpassowrd)
        {
            // User user =Get_Post_Login_Details(string userid)
            string decrppassword = SecurePasswordHasherHelper.Decrypt(hashpassowrd);
            if (password == decrppassword)
            {
                return true;
            }
            return false;
        }

    }
}
