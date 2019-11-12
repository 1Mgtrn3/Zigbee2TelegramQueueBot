using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zigbee2TelegramQueueBot.Services.Notifications
{
    public interface INotificationRouter
    {
        void RouteNotification(NotificationItem notificationItem);
    }
}
