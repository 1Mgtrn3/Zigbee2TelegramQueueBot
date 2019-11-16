using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zigbee2TelegramQueueBot.Enums
{
    public enum StringToLocalize
    {
        MainMenuText,
        StatusFreeMenuText,
        StatusOccupiedMenuText,
        VisitDurationMenuText,
        VisitDurationCustomMenuText,
        QueueMenuText,
        RoomMenuText,
        AddMoreTimeInTheRoomMenuText,
        AddMoreTimeInTheQueueMenuText,
        InBetweenQueueAndRoomMenuText,
        DoorIsLockedMenuText,
        SimpleMainMenuText,
        SimpleStatusMenuText,
        SimpleSubscribedMenuText,
        CommandNotRecognized,
        NumberNotParsed,
        NumberOverLimit,
        doorIsLockedInstruction,
        EnqueueAlert,
        OrderChangeAlert,
        TimeChangeAlert,
        InactivityDequeueAlert,
        SkipFirstAlert,
        SkipOtherAlert,
        SimpleRoomIsFree,
        SimpleRoomIsOccupied,

        MainMenuCommands,
        StatusFreeMenuCommands,
        StatusOccupiedMenuCommands,
        VisitDurationMenuCommands,
        VisitDurationCustomMenuCommands,
        QueueMenuCommands,
        RoomMenuCommands,
        AddMoreTimeInTheRoomMenuCommands,
        AddMoreTimeInTheQueueMenuCommands,
        InBetweenQueueAndRoomMenuCommands,
        DoorIsLockedMenuCommands

    }
}
