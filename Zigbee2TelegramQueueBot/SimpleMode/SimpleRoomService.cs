using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Zigbee2TelegramQueueBot.Services.Helpers.Log;
using Zigbee2TelegramQueueBot.Services.LockTracker;
using Zigbee2TelegramQueueBot.Services.Notifications;
using Zigbee2TelegramQueueBot.Services.Room;
using Zigbee2TelegramQueueBot.Services.Room.Queue;
using Zigbee2TelegramQueueBot.Services.Helpers.Localization;
using Zigbee2TelegramQueueBot.Enums;

namespace Zigbee2TelegramQueueBot.SimpleMode
{
    public class SimpleRoom : IRoomService
    {
        private ObservableCollection<QueueSlot> SubscribedUsers { get; set; }        
        private readonly ILockTrackerService _lockTrackerService;
        private readonly IRoomQueue _roomQueue;
        private readonly ILogHelper _LogHelper;
        private readonly INotificationRouter _notificationRouter;
        private readonly ILocalizationHelper _localizationHelper;

        public SimpleRoom(
            ILockTrackerService lockTrackerService,
            IRoomQueue roomQueue,
            ILogHelper LogHelper,
            INotificationRouter notificationRouter,
            ILocalizationHelper localizationHelper)
        {
            _roomQueue = roomQueue;
            SubscribedUsers = _roomQueue.QueueList;
            IsBusy = false;
            
            _lockTrackerService = lockTrackerService;

            _LogHelper = LogHelper;
            _notificationRouter = notificationRouter;
            _localizationHelper = localizationHelper;
            _lockTrackerService.PropertyChanged += _lockTrackerService_PropertyChanged;

        }
        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                if (value)
                {
                    _IsBusy = true;
                    Enqueue(0);
                }
                else
                {
                    _IsBusy = false;
                    try
                    {
                        DequeueId(0);
                    }
                    catch (Exception)
                    {


                    }
                }
            }
        }
        public CancellationTokenSource roomCancellationTokenSource { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private void _lockTrackerService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (SubscribedUsers.Any())
            {
                //var chatId = QueueList[0].ChatId;
                if (_lockTrackerService.IsRoomFree)
                {
                    IsBusy = false;
                    foreach (var user in SubscribedUsers)
                    {
                        if (user.ChatId != 0)
                        {
                            string notificationText = _localizationHelper.GetLocalizedString(StringToLocalize.SimpleRoomIsFree)
                                .Replace("[SUBCOUNT]", SubscribedUsers.Count.ToString());
                            _notificationRouter.RouteNotification(new NotificationItem(user.ChatId, Enums.NotificationType.Send, notificationText));
                        }

                    }

                }
                else
                {
                    IsBusy = true;
                    foreach (var user in SubscribedUsers)
                    {
                        if (user.ChatId != 0)
                        {
                            string notificationText = _localizationHelper.GetLocalizedString(StringToLocalize.SimpleRoomIsOccupied)
                                .Replace("[SUBCOUNT]", (SubscribedUsers.Count -1).ToString());
                            _notificationRouter.RouteNotification(new NotificationItem(user.ChatId, Enums.NotificationType.Send, $"The room is occupied. Subscribed (including you): {SubscribedUsers.Count - 1}"));
                        }

                    }



                }
            }

        }


        public void AddMoreTime(long chatId, int timeMinutes)
        {
            throw new NotImplementedException();
        }

        public void CancelInBetweenTimer(bool toTheRoom)
        {
            throw new NotImplementedException();
        }

        public void Dequeue([CallerMemberName] string caller = null)
        {
            throw new NotImplementedException();
        }

        public void DequeueId(long chatId)
        {
            //SubscribedUsers.Remove(chatId);
            var senderIndex = SubscribedUsers.IndexOf(SubscribedUsers.First(item => item.ChatId == chatId));
            SubscribedUsers.RemoveAt(senderIndex);
            //throw new NotImplementedException();
        }

        public int Enqueue(long chatId, int timeMinutes = 5)
        {
            SubscribedUsers.Add(new QueueSlot(chatId, timeMinutes));
            return SubscribedUsers.Count - 1;
            //throw new NotImplementedException();
        }

        public void EnqueueSomeoneInTheRoom(int timeMinutes)
        {
            throw new NotImplementedException();
        }

        public int GetPlace(long chatId)
        {
            throw new NotImplementedException();
        }

        public int GetQueueSize()
        {
            return SubscribedUsers.Count();
        }

        public int GetTimeToWait(long chatId)
        {
            throw new NotImplementedException();
        }

        public void InitializeRoomDequeue()
        {
            throw new NotImplementedException();
        }

        public void QueueFlush([CallerMemberName] string caller = null)
        {
            SubscribedUsers.Clear();
        }

        public void SkipOrDequeue(long chatId)
        {
            throw new NotImplementedException();
        }
    }
}
