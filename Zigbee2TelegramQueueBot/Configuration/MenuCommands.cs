using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zigbee2TelegramQueueBot.Configuration
{
    public class MenuCommands
    {
        public string MainMenuCommands { get; set; }
        public string StatusFreeMenuCommands { get; set; }
        public string StatusOccupiedMenuCommands { get; set; }
        public string VisitDurationMenuCommands { get; set; }
        public string VisitDurationCustomMenuCommands { get; set; }
        public string QueueMenuCommands { get; set; }
        public string RoomMenuCommands { get; set; }
        public string AddMoreTimeInTheRoomMenuCommands { get; set; }
        public string AddMoreTimeInTheQueueMenuCommands { get; set; }
        public string InBetweenQueueAndRoomMenuCommands { get; set; }
        public string DoorIsLockedMenuCommands { get; set; }
    }
}
