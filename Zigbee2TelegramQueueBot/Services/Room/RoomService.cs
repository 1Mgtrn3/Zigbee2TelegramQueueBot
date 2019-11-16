using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zigbee2TelegramQueueBot.Services.Helpers.Log;
using Zigbee2TelegramQueueBot.Services.LockTracker;
using Zigbee2TelegramQueueBot.Services.Menu;
using Zigbee2TelegramQueueBot.Services.Notifications;
using Zigbee2TelegramQueueBot.Services.Room.Queue;
using Zigbee2TelegramQueueBot.Services.Users;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using Zigbee2TelegramQueueBot.Enums;
using System.Collections.Specialized;
using System.Text;
using Zigbee2TelegramQueueBot.Services.Helpers.Localization;

namespace Zigbee2TelegramQueueBot.Services.Room
{
    public class RoomService:IRoomService
    {
        public bool IsBusy { get; set; }
        private ObservableCollection<QueueSlot> QueueList { get; set; }
        private readonly IMenuLoader _menuLoader;
        private readonly IUsersService _usersService;
        
        private readonly ILockTrackerService _lockTrackerService;
        private readonly IRoomQueue _roomQueue;
        private readonly ILogHelper _LogHelper;
        private readonly INotificationRouter _notificationRouter;
        private readonly ILocalizationHelper _localizationHelper;

        public CancellationTokenSource roomCancellationTokenSource { get; set; }
        public CancellationTokenSource inBetweenCancellationTokenSource { get; set; }


        public RoomService(IMenuLoader menuLoader,
            IUsersService usersService,
            
            ILockTrackerService lockTrackerService,
            IRoomQueue roomQueue,
            ILogHelper LogHelper,
            INotificationRouter notificationRouter,
            ILocalizationHelper localizationHelper)
        {


            //LogHelper = new LogHelper();
            _menuLoader = menuLoader;
            _usersService = usersService;
            
            _lockTrackerService = lockTrackerService;
            _roomQueue = roomQueue;
            _LogHelper = LogHelper;
            _notificationRouter = notificationRouter;
            _localizationHelper = localizationHelper;

            IsBusy = false;
            _LogHelper.Log("HJ64K54J45", "Room instance created", LogLevel.Information);
            QueueList = _roomQueue.QueueList;//new ObservableCollection<QueueSlot>();
            QueueList.CollectionChanged += QueueChangeHandler;
            _lockTrackerService.PropertyChanged += _lockTrackerService_PropertyChanged;
            roomCancellationTokenSource = new CancellationTokenSource();
            inBetweenCancellationTokenSource = new CancellationTokenSource();

        }

        private async void _lockTrackerService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (QueueList.Any())
            {
                var chatId = QueueList[0].ChatId;
                if (_lockTrackerService.IsRoomFree)
                {

                    InitializeRoomDequeue();
                   
                    _menuLoader.LoadStateMenu(chatId, UserState.InMainMenu);
                }
                else
                {
                    CancelInBetweenTimer(toTheRoom: true);
                    

                    _menuLoader.LoadStateMenu(chatId, UserState.InTheRoom);


                    _LogHelper.Log("LLN435N43FDGFDGFD879", $"About to delete a notification for {chatId}", chatId,LogLevel.Information);
                    
                }
            }
            else
            {
                _LogHelper.Log("NBN75KJ6U55Y", "Adding user0 to the EMPTY queue", LogLevel.Information);
                var slot = new QueueSlot(0, 5);
                QueueList.Add(slot);

                ProcessUserInTheRoom();


            }


        }



        public void QueueFlush([CallerMemberName] string caller = null)
        {


            string logInformation = $"QUEUEFLUSH is called. Caller: {caller}";

            _LogHelper.Log("FDKJ435435FDVFES", logInformation,LogLevel.Warning);

            QueueList.Clear();
        }

        private void QueueChangeHandler(object sender, NotifyCollectionChangedEventArgs e)
        {


            string logInformation = $"Queue is changed. Action: {e.Action.ToString()}";
            _LogHelper.Log("424FDKLLK324LK2FD", logInformation, LogLevel.Information, true);

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {





                if (QueueList.Count > 0)
                {
                    if (_usersService.UserInfo[QueueList[0].ChatId].State == UserState.InQueue)
                    {
                        ProcessUserInBetweenQueueAndRoom();
                    }


                }
            }

            

        }



        private bool _toTheRoom { get; set; }

        public void CancelInBetweenTimer(bool toTheRoom)
        {


            _toTheRoom = toTheRoom;
            inBetweenCancellationTokenSource.Cancel();
            

        }
        private async Task ProcessUserInBetweenQueueAndRoom()
        {


            _LogHelper.Log("FDSF77LK32K4324F", "Processing InBetween Starts", LogLevel.Information, true);

            string initialNotificationMessage = "Проследуйте в комнату, у вас минута. Вы также можете нажать /начать чтобы начать свой таймер посещения прямо сейчас.";
            string passResultSkip = "По истечении минуты вы пропустите человека после вас вперед.";
            string passResultDequeue = "По истечении минуты вы будете исключены из очереди.";

            string timerNotificationStem = "секунд осталось. Вы также можете нажать /начать чтобы начать свой таймер посещения прямо сейчас.";
            string timerNotification = "";
            if (QueueList.Count == 1)
            {
                initialNotificationMessage = $"{initialNotificationMessage} {passResultDequeue}";
                timerNotification = $"{timerNotificationStem} {passResultDequeue}";
            }
            else
            {
                initialNotificationMessage = $"{initialNotificationMessage} {passResultSkip}";
                timerNotification = $"{timerNotificationStem} {passResultSkip}";
            }
            var chatId = QueueList[0].ChatId;
            await _menuLoader.LoadStateMenu(chatId, UserState.InBetweenQueueAndRoom);
            AlertSubscriber(chatId, initialNotificationMessage);


            var cancellationToken = inBetweenCancellationTokenSource.Token;
            //bool isCanceled = false;
            int count = 60;
            int intervalSeconds = 10;
            var period = TimeSpan.FromSeconds(10);
            while (!cancellationToken.IsCancellationRequested && count > 0)
            {
                
                try
                {
                    await Task.Delay(period, cancellationToken);
                }
                catch (Exception ex)
                {
                    _LogHelper.Log("SD23EDH", $"Exception happened during inBetween delay. Message {ex.Message} Type {ex.GetType().Name}", LogLevel.Error, true);

                }

                

                count -= intervalSeconds;
                if (!cancellationToken.IsCancellationRequested)
                {
                    AlertSubscriber(QueueList[0].ChatId, $"{count} {timerNotification}");
                }

            }
            if (!cancellationToken.IsCancellationRequested)
            {
                
                _LogHelper.Log("FDSF87FDSF", $"GOING TO SKIP OR DEQUEUE DUE TO INACTIVITY", LogLevel.Information, true);
                _LogHelper.Log("LK543LK543NNC6456", $"About to delete a notification for {chatId}",  chatId, LogLevel.Information);
                
                SkipOrDequeue(QueueList[0].ChatId);
                inBetweenCancellationTokenSource.Dispose();
                inBetweenCancellationTokenSource = new CancellationTokenSource();

            }
            else
            {
                

                _LogHelper.Log("FEWD1DWDSA", $"IF {_toTheRoom.ToString()} = True -> GOING TO THE ROOM", LogLevel.Information, true);

                
                inBetweenCancellationTokenSource.Dispose();
                inBetweenCancellationTokenSource = new CancellationTokenSource();
                if (_toTheRoom)
                {
                    ProcessUserInTheRoom();
                }


            }



            _LogHelper.Log("F7SDF8DS", "Processing InBetween Ends", LogLevel.Information, true);
        }

        private async Task ProcessUserInTheRoom()
        {



            _LogHelper.Log("FDS77FDSFUI", "Start", LogLevel.Trace, true);
            var cancellationToken = roomCancellationTokenSource.Token;
            var period = TimeSpan.FromMinutes(1);
            
            _LogHelper.Log("FDS7F7FDS", $"IS INTHEROOM CANCELLATION REQUESTED: {cancellationToken.IsCancellationRequested}", LogLevel.Information, false);
            bool isDequedAlready = false;
            while (!cancellationToken.IsCancellationRequested && QueueList[0].TimeMinutes > 0)
            {
                
                _LogHelper.Log("FDS8DS8FDS", $"before await", LogLevel.Trace, false);
                try
                {
                    await Task.Delay(period, cancellationToken);
                }
                catch (Exception ex)
                {
                    
                    _LogHelper.Log("F9DSF9DSFS", $"Exception happened during room delay. Message {ex.Message} Type {ex.GetType().Name}",LogLevel.Error, false);

                }
                _LogHelper.Log("FDS98FDS98", $"after await", LogLevel.Trace, false);
                if (!cancellationToken.IsCancellationRequested && QueueList[0].TimeMinutes > 0)
                {                    
                    QueueList[0].TimeMinutes -= 1;
                    if (QueueList[0].TimeMinutes > 0)
                    {
                        AlertSubscriber(QueueList[0].ChatId, $"{QueueList[0].TimeMinutes} minutes left!");
                    }
                }
                else
                {
                    _LogHelper.Log("FDS8DSDS", $"trying to Dequeue with the first option", LogLevel.Trace, true);
                    roomCancellationTokenSource.Dispose();
                    roomCancellationTokenSource = new CancellationTokenSource();

                    if (!_lockTrackerService.IsLockTrackerInstalled)
                    {
                        Dequeue();
                        isDequedAlready = true;
                        _LogHelper.Log("FDS98DSF98", $"after Dequeue", LogLevel.Trace, false);
                    }


                   
                }

            }
            

            if (!isDequedAlready)
            {

                roomCancellationTokenSource.Dispose();
                roomCancellationTokenSource = new CancellationTokenSource();

                if (!_lockTrackerService.IsLockTrackerInstalled)
                {
                    _LogHelper.Log("FDS9DSF9S", $"trying to Dequeue with the second option", LogLevel.Trace, true);
                    Dequeue();
                                        
                    _LogHelper.Log("DSFDS87DSDS", $"after Dequeue with the second option", LogLevel.Trace, true);
                }

            }




        }

        

        public void InitializeRoomDequeue()
        {


           
            _LogHelper.Log("GF8787GF8", $"Start", LogLevel.Trace, true);

            if (!_lockTrackerService.IsLockTrackerInstalled)
            {
                roomCancellationTokenSource.Cancel();
            }
            else
            {
                roomCancellationTokenSource.Cancel();
                Dequeue();
            }


        }



        public int Enqueue(long id, int timeMinutes = 5)
        {



            QueueList.Add(new QueueSlot(id, timeMinutes));
            if (QueueList.Count - 1 > 0)
            {
                string notificationText = _localizationHelper.GetLocalizedString(StringToLocalize.EnqueueAlert)
                    .Replace("[QUEUEPOS]", (QueueList.Count - 1).ToString())
                    .Replace("[WAITTIME]", GetTimeToWait(id).ToString());
                AlertSubscriber(id, notificationText);
                return QueueList.Count - 1;
            }
            else
            {
                //var t = Task.Run(() => ProcessUserInBetweenQueueAndRoom());
                ProcessUserInBetweenQueueAndRoom();
                return 0;
            }



        }


        public void DequeueId(long chatId)
        {

            var senderIndex = QueueList.IndexOf(QueueList.First(item => item.ChatId == chatId));
            QueueList.RemoveAt(senderIndex);
            AlertSubscribers(NotificationEventType.OrderChange, senderIndex);


        }

        public void Dequeue([CallerMemberName] string caller = null)
        {


            //_logger.LogError($"DEQUEUE CALLED BY {caller}");

            _LogHelper.Log("S87FD7FDS78DS", $"DEQUEUE CALLED BY {caller}", LogLevel.Trace, true);

            QueueList.RemoveAt(0);
            AlertSubscribers(NotificationEventType.OrderChange, 0);


        }

        public void AlertSubscriber(long chatId, string text)
        {


            if (chatId != 0)
            {
                _LogHelper.Log("FDS7DSF8DS7F", $"Subscriber {chatId} is notified of |{text}|", chatId, LogLevel.Information, false);
                //_menuLoader.SendNotification(chatId, text);//.SendText(chatId, text);
                _notificationRouter.RouteNotification(new NotificationItem(chatId, NotificationType.Send, text));
            }


        }

        public void AlertSubscribers(NotificationEventType notificationType, long senderIndex)
        {

            _LogHelper.Log("FDS32432KJ", $"Start. Notification type {notificationType.ToString()} Sender index: {senderIndex}", LogLevel.Information, true);


            if (notificationType == NotificationEventType.OrderChange)
            {
                for (int i = QueueList.Count - 1; i > senderIndex; i--)
                {
                    if (i != 0)
                    {
                        string notificationText = _localizationHelper.GetLocalizedString(StringToLocalize.OrderChangeAlert)
                            .Replace("[QUEUEPOS]", i.ToString())
                            .Replace("[WAITTIME]", GetTimeToWait(QueueList[i].ChatId).ToString());
                        AlertSubscriber(QueueList[i].ChatId, notificationText);
                    }
                }


                
            }
            else
            {
                for (int i = QueueList.Count - 1; i > senderIndex; i--)
                {
                    string notificationText = _localizationHelper.GetLocalizedString(StringToLocalize.TimeChangeAlert)
                        .Replace("[WAITTIME]", GetTimeToWait(QueueList[i].ChatId).ToString());
                    AlertSubscriber(QueueList[i].ChatId, notificationText);
                }


            }

        }

        public int GetQueueSize()
        {


            return QueueList.Count;
        }

        public void AddMoreTime(long chatId, int timeMinutes)
        {


            var tempIndex = QueueList.IndexOf(QueueList.FirstOrDefault(x => x.ChatId == chatId));

            if (tempIndex == 0)
            {
                
                QueueList[tempIndex].TimeMinutes += timeMinutes;
            }
            else
            {
                QueueList[tempIndex].TimeMinutes += timeMinutes;

            }
            var senderIndex = QueueList.IndexOf(QueueList.First(item => item.ChatId == chatId));

            AlertSubscribers(NotificationEventType.TimeChange, senderIndex);
            
        }

        public int GetPlace(long chatId)
        {


            return QueueList.IndexOf(QueueList.FirstOrDefault(x => x.ChatId == chatId));
        }

        public async void SkipOrDequeue(long chatId)
        {


            var tempIndex = QueueList.IndexOf(QueueList.FirstOrDefault(x => x.ChatId == chatId));
            _LogHelper.Log("FDS9FDJ23J423K", $"About to decide (SkipOrDequeue) for index {tempIndex} and chatId {chatId}", LogLevel.Trace, true);
            if (QueueList.Last().ChatId == chatId || QueueList[tempIndex].FailedAttempts == 1)
            {
                _LogHelper.Log("LKN5LK43N543V5CX", "Will dequeue", chatId, LogLevel.Trace);
                string notificationText = _localizationHelper.GetLocalizedString(StringToLocalize.InactivityDequeueAlert);
                AlertSubscriber(chatId, notificationText);
                await _menuLoader.LoadStateMenu(chatId, UserState.InMainMenu);
                DequeueId(chatId);


            }
            else
            {
                
                _LogHelper.Log("43hjh432j4h2klk", $"Will skip. tempindex = {tempIndex}", chatId, LogLevel.Trace);
                
                ++QueueList[tempIndex].FailedAttempts;
                var temp = QueueList[tempIndex];

                QueueList[tempIndex] = QueueList[tempIndex + 1];
                QueueList[tempIndex + 1] = temp;

                if (tempIndex == 0)
                {
                    ProcessUserInBetweenQueueAndRoom();
                    string notificationText = _localizationHelper.GetLocalizedString(StringToLocalize.SkipFirstAlert)
                        .Replace("[QUEUEPOS]",  (tempIndex + 1).ToString())
                        .Replace("[WAITTIME]", GetTimeToWait(QueueList[tempIndex + 1].ChatId).ToString());

                    AlertSubscriber(QueueList[tempIndex + 1].ChatId, notificationText);
                    _menuLoader.LoadStateMenu(chatId, UserState.InQueue);


                }
                else
                {
                    string notificationText = _localizationHelper.GetLocalizedString(StringToLocalize.SkipOtherAlert)
                        .Replace("[QUEUEPOS]", tempIndex.ToString())
                        .Replace("[WAITTIME]", GetTimeToWait(QueueList[tempIndex].ChatId).ToString());
                    AlertSubscriber(QueueList[tempIndex].ChatId, notificationText);


                    //_menuLoader.LoadStateMenu(chatId, UserState.InQueue);
                }



            }


        }



        public int GetTimeToWait(long chatId)
        {


            var tempIndex = QueueList.IndexOf(QueueList.FirstOrDefault(x => x.ChatId == chatId));
            int resultMinutes = 0;

            for (int i = tempIndex - 1; i >= 0; i--)
            {
                resultMinutes += QueueList[i].TimeMinutes;
            }

            return resultMinutes;
        }

        private string GetQueueStatusTrace()
        {



            StringBuilder traceBuilder = new StringBuilder();
            traceBuilder.AppendLine("==================");
            traceBuilder.AppendLine("Queue status trace");
            traceBuilder.AppendLine($"Queue size: {GetQueueSize().ToString()}");
            traceBuilder.AppendLine("#Queue Slots:");
            for (int i = 0; i < QueueList.Count; i++)
            {
                traceBuilder.AppendLine($"##Number: {i}; ChatID: {QueueList[i].ChatId}; Time: {QueueList[i].TimeMinutes};");
            }
            traceBuilder.AppendLine("==================");


            return traceBuilder.ToString();
        }

        
        public void EnqueueSomeoneInTheRoom(int timeMinutes)
        {
            QueueList.Insert(0, new QueueSlot(0, timeMinutes));
            //throw new NotImplementedException();
        }
    }
}
