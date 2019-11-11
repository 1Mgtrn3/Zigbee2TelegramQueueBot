using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zigbee2TelegramQueueBot.Configuration
{
    public class SOCKS5Proxy
    {
        public string Socks5Host { get; set; }
        public int? Socks5Port { get; set; }
        public string Socks5Username { get; set; }
        public string Socks5Password { get; set; }

    }
}
