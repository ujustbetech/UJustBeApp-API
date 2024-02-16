using Notification.Service.Models.FCMToken;

namespace Notification.Service.Repositories
{
    public interface IFCMTokenService
    {
        void Update_FCM_Token(Put_Request request);
    }
}
