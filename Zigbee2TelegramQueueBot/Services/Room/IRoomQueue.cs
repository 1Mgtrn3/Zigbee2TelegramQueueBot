using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Zigbee2TelegramQueueBot.Services.Room
{
    public interface IRoomQueue
    {
        ObservableCollection<QueueSlot> QueueList { get; set; }

        int GetOverallWaitTimeMinutes();

    }
}
