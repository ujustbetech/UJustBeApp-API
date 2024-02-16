using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Notification.Service.Models;
using Notification.Service.Repositories;
using System.Collections.Generic;
using System.Linq;
using UJBHelper.Data;
using UJBHelper.DataModel;

namespace Notification.Service.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<NotificationList> _notificationList;
        private IConfiguration _iconfiguration;
        public NotificationService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _users = database.GetCollection<User>("Users");
            _notificationList = database.GetCollection<NotificationList>("NotificationList");
        }

        public Get_Request Get_All_Notifications(Post_Request request)
        {
            var res = new Get_Request();
            var notify = _notificationList.Find(x => x.userId == request.userId).ToList();

            res.totalCount = notify.Count();
            res.totalUnreadCount = notify.Where(x => x.isRead == false).Count();

            res.notifications = notify.Select(x => new Request_Info
            {
                id = x._id,
                message = x.messageText,
                date = x.date,
                type = x.type,
                isRead = x.isRead,
                isSystemGenerated = x.isSystemGenerated,
               leadId = x.leadId,
                isReferredByMe = x.isReferredByMe,
            }).OrderByDescending(x => x.date).Skip(request.skipTotal).Take(10).ToList();

            return res;
        }

        public void Update_Notification_Read_Flag(List<string> notificationIds)
        {
            _notificationList.UpdateMany(Builders<NotificationList>.Filter.In(x => x._id, notificationIds)
                ,Builders<NotificationList>.Update
                .Set(x => x.isRead, true));
        }
    }
}
