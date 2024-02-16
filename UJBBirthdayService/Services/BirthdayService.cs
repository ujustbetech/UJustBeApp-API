using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using UJBHelper.Common;
using UJBHelper.Data;
using UJBHelper.DataModel;

namespace UJBBirthdayService.Services
{
    public class BirthdayService
    {
        private readonly IMongoCollection<Notification> _notification;
        private readonly IMongoCollection<NotificationQueue> _notificationQueue;
        private readonly IMongoCollection<User> _users;

        public BirthdayService()
        {

            var client = new MongoClient(DbHelper.GetConnectionString());
            var database = client.GetDatabase(DbHelper.GetDatabaseName());
            _notification = database.GetCollection<Notification>("Notification");
            _notificationQueue = database.GetCollection<NotificationQueue>("NotificationQueue");
            _users = database.GetCollection<User>("Users");
        }

        internal bool Check_If_Birthday()
        {
            var nq = new Notification_Queue();
            var bdayuserExists = _users.Find(x => x.birthDate.HasValue).CountDocuments() != 0;
            //  var bdayuserExists = _users.Find(x => x.birthDate == DateTime.Today && x.Role.Contains("Partner")).CountDocuments() != 0;

            if (bdayuserExists)
            {

                var users = _users.Find(x => x.birthDate.HasValue).ToList();
                var date = DateTime.Now.ToString("yyyy-MM-dd");
                var gte = DateTime.Parse(date + " 00:00:00");
                var lte = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " 00:00:00");
                foreach (var user in users)
                {
                    // var abc = user.birthDate.Value;
                    if (user.birthDate.Value.Month == DateTime.Now.Month && user.birthDate.Value.Day == DateTime.Now.Day)
                    {
                        var aa = _notificationQueue.Find(x => x.userId == user._id && x.Event == "Birthday" && x.status == "success" && x.dateOfNotification > gte && x.dateOfNotification < lte).ToList();
                        var alreadySent = _notificationQueue.Find(x => x.userId == user._id && x.Event == "Birthday" && x.status == "success" && x.dateOfNotification > gte && x.dateOfNotification < lte).CountDocuments() != 0;
                        if (!alreadySent)
                        {
                            var sendNotification = SendNotification(user.firstName, user.lastName, user._id);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public Task SendNotification(string firstName, string lastName, string UserId)
        {
            MessageBody MB = new MessageBody();
            MB.UserName = firstName + " " + lastName;
            var nq = new Notification_Sender();
            nq.SendNotification("Birthday", MB, UserId, "", "");
            return Task.CompletedTask;

        }

    }
}
