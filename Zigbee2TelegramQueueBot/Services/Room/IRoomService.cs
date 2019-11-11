using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Zigbee2TelegramQueueBot.Services.Room
{
    interface IRoomService
    {
        bool IsBusy { get; set; }

        int Enqueue(long chatId, int timeMinutes = 5);
        void Dequeue([CallerMemberName] string caller = null);
        void DequeueId(long chatId);
       
        int GetQueueSize();
        void AddMoreTime(long chatId, int timeMinutes);
        int GetPlace(long chatId);
        void SkipOrDequeue(long chatId);

        int GetTimeToWait(long chatId);

        void CancelInBetweenTimer(bool toTheRoom);

        void QueueFlush([CallerMemberName] string caller = null);

        CancellationTokenSource roomCancellationTokenSource { get; set; }

        void InitializeRoomDequeue();

        void EnqueueSomeoneInTheRoom(int timeMinutes);
    }
}
