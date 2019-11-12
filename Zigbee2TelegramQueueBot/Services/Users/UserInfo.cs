using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zigbee2TelegramQueueBot.Enums;
using Zigbee2TelegramQueueBot.Services.Notifications;

namespace Zigbee2TelegramQueueBot.Services.Users
{
    public class UserInfo
    {
        public bool NotificationProcessingCompleted { get; set; } = true;
        public ConcurrentQueue<NotificationItem> NotificationsQueue { get; set; }
        public UserInfo()
        {
            NotificationsQueue = new ConcurrentQueue<NotificationItem>();

        }
        public UserState State { get; set; }
        public int LastMessageId { get; set; }

        public int LastNotificationMessageId { get; set; }

        public string UserName { get; set; }


    }
    
}
