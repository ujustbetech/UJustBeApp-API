using System.Collections.Generic;
using Notification.Service.Models;

namespace Notification.Service.Repositories
{
    public interface INotificationService
    {
        Get_Request Get_All_Notifications(Post_Request request);
        void Update_Notification_Read_Flag(List<string> notificationIds);
    }
}
