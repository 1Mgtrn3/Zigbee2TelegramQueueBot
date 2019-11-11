using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zigbee2TelegramQueueBot.Configuration
{
    public class LockTrackerConfiguration
    {
        public bool IsLockTrackerInstalled { get; set; }
        public string TcpServerIp { get; set; }
        public int TcpServerPort { get; set; }
        public string ClientId { get; set; }
        public string MqttTopic { get; set; }
        public bool TestMode { get; set; }
    }
}
