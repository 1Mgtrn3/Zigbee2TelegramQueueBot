using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zigbee2TelegramQueueBot.Enums
{
    public enum UserState
    {
        InMainMenu,
        InStatusFree,
        InStatusOccupied,
        InVisitDuration,
        InVisitDurationCustom,
        InQueue,
        InTheRoom,
        InAddMoreTimeInTheRoom,
        InAddMoreTimeInTheQueue,
        InBetweenQueueAndRoom,
        InDoorIsLocked,
        InSimpleMainMenu,
        InSimpleStatus,
        InSimpleSubscribed
    }
}
