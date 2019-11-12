using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Zigbee2TelegramQueueBot.Services.TelegramUpdateHandler
{
    public interface IUpdateService
    {
        Task UpdateHandle(Update update);
    }
}
