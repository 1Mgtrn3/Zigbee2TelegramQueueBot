using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zigbee2TelegramQueueBot.Services.Helpers.Log;
using Zigbee2TelegramQueueBot.Services.Users;
using Microsoft.Extensions.Logging;

namespace Zigbee2TelegramQueueBot.Services.Notifications
{
    public class NotificationRouter : INotificationRouter
    {
        private readonly IUsersService _users;
        private readonly ILogHelper _logHelper;
        public NotificationRouter(IUsersService users, ILogHelper logHelper)
        {
            _users = users;
            _logHelper = logHelper;
        }

        public void RouteNotification(NotificationItem notificationItem)
        {

            _logHelper.Log("JK543K34NL34L", $"Routing notification: chatId: {notificationItem.ChatId}; type: {notificationItem.NotificationType}",LogLevel.Information);
            _users.UserInfo[notificationItem.ChatId].NotificationsQueue.Enqueue(notificationItem);

        }

        

    }
}
