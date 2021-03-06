using System;

namespace WarrantyWarden
{
    public interface INotificationManager
    {
        event EventHandler NotificationReceived;
        void Initialize();
        void SendNotification(string title, string message, DateTime? notifyTime = null);
        void DeleteAllNotifications();
        void ReceiveNotification(string title, string message);
    }
}
