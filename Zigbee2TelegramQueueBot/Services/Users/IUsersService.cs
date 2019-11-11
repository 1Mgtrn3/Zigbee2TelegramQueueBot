using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zigbee2TelegramQueueBot.Services.Users
{
    public interface IUsersService
    {
        Dictionary<long, UserInfo> UserInfo { get; set; }
    }
}
