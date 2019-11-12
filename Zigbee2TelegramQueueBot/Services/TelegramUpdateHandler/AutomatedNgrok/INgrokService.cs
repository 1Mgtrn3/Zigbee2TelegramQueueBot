using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zigbee2TelegramQueueBot.Services.TelegramUpdateHandler.AutomatedNgrok
{
    public interface INgrokService
    {
        Task<string> GetNgrokUrlAsync();
    }
}
