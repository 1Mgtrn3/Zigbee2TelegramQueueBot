using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zigbee2TelegramQueueBot.Configuration
{
    public class HTTPProxy
    {
        public string HTTPHost { get; set; }
        public int? HTTPPort { get; set; }
        public string HTTPUser { get; set; }
        public string HTTPPassword { get; set; }

    }
}
