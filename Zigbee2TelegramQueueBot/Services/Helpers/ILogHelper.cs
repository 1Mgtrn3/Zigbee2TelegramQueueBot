using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Zigbee2TelegramQueueBot.Services.Helpers
{
    public interface ILogHelper
    {
        void Log(string codeLabel, string information, LogLevel logLevel, bool includeQueue = false, [CallerMemberName] string caller = null);
        void Log(string codeLabel, string information, long chatId, LogLevel logLevel, bool includeQueue = false, [CallerMemberName] string caller = null);
        string GetQueueStatusTrace();
    }
}
