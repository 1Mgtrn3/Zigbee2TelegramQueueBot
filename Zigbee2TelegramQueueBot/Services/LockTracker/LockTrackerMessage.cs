using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zigbee2TelegramQueueBot.Services.LockTracker
{
    public class LockTrackerMessage
    {
        public bool contact { get; set; }
        public int linkquality { get; set; }
        public int battery { get; set; }
        public int voltage { get; set; }
    }
}
