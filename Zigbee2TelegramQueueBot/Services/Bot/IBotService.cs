using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Zigbee2TelegramQueueBot.Services.Bot
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
    }
}
