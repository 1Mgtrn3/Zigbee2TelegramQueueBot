using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Zigbee2TelegramQueueBot.Services.LockTracker
{
   public interface ILockTrackerService: INotifyPropertyChanged
    {
        bool IsLockTrackerInstalled { get; set; }

        bool IsRoomFree { get; set; }
    }
}
