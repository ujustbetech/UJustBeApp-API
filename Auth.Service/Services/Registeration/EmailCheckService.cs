using Auth.Service.Respositories.Registeration;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using UJBHelper.DataModel;

namespace Auth.Service.Services.Registeration
{
    public class EmailCheckService : IEmailCheckService
    {
        private readonly IMongoCollection<User> _users;
        private IConfiguration _iconfiguration;
        public EmailCheckService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _users = database.GetCollection<User>("Users");
        }
        public bool Check_If_Email_Exists(string emailId)
        {
            return _users.Find(x => x.emailId.ToLower() == emailId.ToLower()).CountDocuments() > 0 ? true : false;
        }

        public bool Check_If_Mobile_Exists(string mobileNo)
        {
            return _users.Find(x => x.mobileNumber == mobileNo).CountDocuments() > 0 ? true : false;
        }

       
    }
}
