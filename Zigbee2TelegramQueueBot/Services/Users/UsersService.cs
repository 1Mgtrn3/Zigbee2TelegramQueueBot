using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zigbee2TelegramQueueBot.Services.Users
{
    public class UsersService : IUsersService
    {
        public Dictionary<long, UserInfo> UserInfo { get; set; }
        public UsersService()
        {
            UserInfo = new Dictionary<long, UserInfo>();
        }

        
    }
}
