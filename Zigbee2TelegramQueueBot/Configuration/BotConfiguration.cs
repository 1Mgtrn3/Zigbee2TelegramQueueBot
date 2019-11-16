using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zigbee2TelegramQueueBot.Enums;

namespace Zigbee2TelegramQueueBot.Configuration
{
    public class BotConfiguration
    {
        public BotConfiguration()
        {
            //MenuTexts = new MenuTexts();
            //MenuCommands = new MenuCommands();
            //SimpleMenuTexts = new SimpleMenuTexts();
            SOCKS5Proxy = new SOCKS5Proxy();
            HTTPProxy = new HTTPProxy();

        }
        public Language Language { get; set; }
        public bool SimpleMode { get; set; }
        public string BotToken { get; set; }
        public ProxyMode ProxyMode { get; set; }        
        public string ExternalURL { get; set; }
        public int TimeLimitMinutes { get; set; }

        public string MenuMode { get; set; }

        //public MenuTexts MenuTexts { get; set; }
        //public SimpleMenuTexts SimpleMenuTexts { get; set; }

        public MenuCommands MenuCommands { get; set; }

        public SOCKS5Proxy SOCKS5Proxy { get; set; }

        public HTTPProxy HTTPProxy { get; set; }



        public string NgrokPath { get; set; }
        public string NgrokArguments { get; set; }
        public int AttempstLimit { get; set; }
    }
}
