using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Zigbee2TelegramQueueBot.Services.Room.Queue;

namespace Zigbee2TelegramQueueBot.SimpleMode
{
    public class SimpleRoomQueue: IRoomQueue
    {
        public ObservableCollection<QueueSlot> QueueList { get; set; }

        public int GetOverallWaitTimeMinutes()
        {
            throw new Exception("GetOverallWaitTimeMinutes is not available for this type of RoomQueue. Please use a different RoomQueue instead");
        }
    }
}
