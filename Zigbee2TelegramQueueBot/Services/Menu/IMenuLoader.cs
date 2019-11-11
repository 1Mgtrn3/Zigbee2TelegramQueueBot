using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Zigbee2TelegramQueueBot.Enums;

namespace Zigbee2TelegramQueueBot.Services.Menu
{
    public interface IMenuLoader
    {
        string MenuLoaderType { get; }
        Task LoadStateMenu(long chatId, UserState userState, bool removeNotification = true, [CallerMemberName] string caller = null);


    }
}
