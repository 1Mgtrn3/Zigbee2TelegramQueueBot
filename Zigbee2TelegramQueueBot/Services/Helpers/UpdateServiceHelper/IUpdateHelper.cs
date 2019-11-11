using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Zigbee2TelegramQueueBot.Services.Helpers.UpdateServiceHelper
{
    public interface IUpdateHelper
    {
        long GetChatId(Update update);
        string GetUserName(Update update);
    }
}
