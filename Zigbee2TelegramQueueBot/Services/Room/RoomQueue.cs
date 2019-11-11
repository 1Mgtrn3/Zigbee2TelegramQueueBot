using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Zigbee2TelegramQueueBot.Services.Room
{
    public class RoomQueue : IRoomQueue
    {
        public ObservableCollection<QueueSlot> QueueList { get; set; }
        public RoomQueue()
        {
            QueueList = new ObservableCollection<QueueSlot>();
        }

        public int GetOverallWaitTimeMinutes()
        {
            int result = 0;

            foreach (var slot in QueueList)
            {
                result += slot.TimeMinutes;
            }
            return result;
        }
    }
}
