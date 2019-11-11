using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zigbee2TelegramQueueBot.Services.Room
{
    public class QueueSlot
    {
        public long ChatId { get; set; }
        public int TimeMinutes { get; set; }

        public int FailedAttempts { get; set; }

        public QueueSlot(long chatId, int timeMinutes)
        {
            ChatId = chatId;
            TimeMinutes = timeMinutes;
            FailedAttempts = 0;
        }
    }
}
