using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zigbee2TelegramQueueBot.Enums;

namespace Zigbee2TelegramQueueBot.Services.Notifications
{
    public class NotificationItem
    {
        public NotificationItem()
        {

        }
        public NotificationItem(long chatId, NotificationType notificationType, string data = "")
        {
            ChatId = chatId;
            NotificationType = notificationType;
            Data = data;
        }
        public long ChatId { get; set; }
        public NotificationType NotificationType { get; set; }
        public string Data { get; set; }
    }
}
