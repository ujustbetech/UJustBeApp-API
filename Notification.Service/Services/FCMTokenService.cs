using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Notification.Service.Models.FCMToken;
using Notification.Service.Repositories;
using UJBHelper.Data;
using UJBHelper.DataModel;

namespace Notification.Service.Services
{
    public class FCMTokenService : IFCMTokenService
    {
        private readonly IMongoCollection<User> _users;
        private IConfiguration _iconfiguration;
        public FCMTokenService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _users = database.GetCollection<User>("Users");
        }

        public void Update_FCM_Token(Put_Request request)
        {
            _users.FindOneAndUpdate(
                Builders<User>.Filter.Eq(x => x._id, request.userId),
                Builders<User>.Update.Set(x => x.fcmToken, request.token)
                );
        }
    }
}
