using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Zigbee2TelegramQueueBot.Configuration;
using Zigbee2TelegramQueueBot.Enums;
using Zigbee2TelegramQueueBot.Services;
using Zigbee2TelegramQueueBot.Services.Bot;
using Zigbee2TelegramQueueBot.Services.Helpers.Log;
using Zigbee2TelegramQueueBot.Services.Helpers.UpdateServiceHelper;
using Zigbee2TelegramQueueBot.Services.Menu;
using Zigbee2TelegramQueueBot.Services.Notifications;
using Zigbee2TelegramQueueBot.Services.Room;
using Zigbee2TelegramQueueBot.Services.Users;
using Zigbee2TelegramQueueBot.Services.LockTracker;

namespace Zigbee2TelegramQueueBot.Services.Session
{
    public class SessionRouter : ISessionRouter
    {
        private readonly IUsersService _users;
        private readonly ILogger<SessionRouter> _logger;
        private readonly IRoomService _room;
        private readonly IBotService _botService;
        private readonly IOptions<BotConfiguration> _config;
        //private readonly IUpdateService _updateService;
        private readonly IUpdateHelper _updateHelper;
        private readonly ILockTrackerService _lockTrackerService;
        private readonly ILogHelper _logHelper;
        private readonly INotificationRouter _notificationRouter;


        private readonly IMenuLoader _menuLoader;

        //private IEnumerable<IMenuLoader> _menuLoaders { get; set; }

        string CommandNotRecognized = "Command wasn't recognized. Please try again.";
        string NumberOverLimit = "The value you've entered is over the limit. Please try again.";
        string NumberNotParsed = "You have entered an invalid value. Please enter a number.";
        string doorIsLockedInstruction = "Add yourself some time to cover both the person in the room and yourself.";

        public SessionRouter(IUpdateHelper updateHelper,
                            IMenuLoader menuLoader,
                            ILogger<SessionRouter> logger,
                            IRoomService room,
                            IUsersService users,
                            IBotService botService,
                            IOptions<BotConfiguration> config,
                            ILockTrackerService lockTrackerService,
                            ILogHelper logHelper,
                            INotificationRouter notificationRouter)
        {

            //_menuLoaders = menuLoaders;
            _menuLoader = menuLoader;
            _config = config;
            _logHelper = logHelper;

            _updateHelper = updateHelper;
            _notificationRouter = notificationRouter;
            //if (config.Value.MenuMode=="BUTTONS")
            //{
            //    _menuLoader = _menuLoaders.FirstOrDefault(l => l.MenuLoaderType == "ButtonMenuLoader");//menuLoader;
            //}
            //else
            //{
            //    _menuLoader = _menuLoaders.FirstOrDefault(l => l.MenuLoaderType == "TextMenuLoader");//menuLoader;
            //}



            _users = users;
            _logger = logger;
            _room = room;
            _botService = botService;
            _lockTrackerService = lockTrackerService;

        }

        private UserState GetStatus(Update update)
        {
            long chatId = _updateHelper.GetChatId(update);

            return _users.UserInfo[chatId].State;

        }

        private string GetCommand(Update update)
        {
            long chatId = _updateHelper.GetChatId(update);//update.Message.Chat.Id;

            if (_menuLoader.MenuLoaderType == typeof(TextMenuLoader).ToString())
            {
                return update.Message.Text;
            }
            else
            {
                _users.UserInfo[chatId].LastMessageId = update.CallbackQuery.Message.MessageId;//.Message.MessageId;
                return update.CallbackQuery.Data;//.InlineQuery.Query;
            }


        }

        public void RouteUpdate(Update update)
        {
            var userStatus = GetStatus(update);

            switch (userStatus)
            {
                case UserState.InMainMenu:
                    MainMenuHandler(update);
                    break;
                case UserState.InStatusFree:
                    StatusFreeMenuHandler(update);
                    break;
                case UserState.InStatusOccupied:
                    StatusOccupiedMenuHandler(update);
                    break;
                case UserState.InVisitDuration:
                    VisitDurationMenuHandler(update);
                    break;
                case UserState.InVisitDurationCustom:
                    VisitDurationCustomMenuHandler(update);
                    break;
                case UserState.InQueue:
                    QueueMenuHandler(update);
                    break;
                case UserState.InTheRoom:
                    RoomMenuHandler(update);
                    break;
                case UserState.InAddMoreTimeInTheRoom:
                    AddMoreTimeMenuHandler(update, UserState.InAddMoreTimeInTheRoom);
                    break;
                case UserState.InAddMoreTimeInTheQueue:
                    AddMoreTimeMenuHandler(update, UserState.InAddMoreTimeInTheQueue);
                    break;
                case UserState.InBetweenQueueAndRoom:
                    InBetweenQueueAndRoomMenuHandler(update);
                    break;
                case UserState.InDoorIsLocked:
                    InDoorIsLockedMenuHandler(update);
                    break;
                default:
                    break;
            }


        }





        private void MainMenuHandler(Update update)
        {
            long chatId = _updateHelper.GetChatId(update);
            var command = GetCommand(update);
            switch (command)
            {
                case "/status":
                    if (_room.GetQueueSize() > 0)
                    {
                        //LoadStatusOccupiedMenu(chatId);
                        _menuLoader.LoadStateMenu(chatId, UserState.InStatusOccupied);
                    }
                    else
                    {
                        _menuLoader.LoadStateMenu(chatId, UserState.InStatusFree);
                    }
                    break;
                case "/enqueue":
                    if (_room.GetQueueSize() > 0)
                    {
                        _menuLoader.LoadStateMenu(chatId, UserState.InStatusOccupied);
                    }
                    else
                    {
                        _menuLoader.LoadStateMenu(chatId, UserState.InVisitDuration);
                    }
                    break;
                default:
                    IncorrectCommandHandler(update, UserState.InMainMenu);
                    _menuLoader.LoadStateMenu(chatId, UserState.InMainMenu);

                    break;
            }

        }

        private void StatusFreeMenuHandler(Update update)
        {
            long chatId = _updateHelper.GetChatId(update);
            var command = GetCommand(update);
            switch (command)
            {
                case "/occupy":
                    //LoadVisitDurationMenu(chatId);
                    _menuLoader.LoadStateMenu(chatId, UserState.InVisitDuration);
                    break;
                case "/exit":
                    _menuLoader.LoadStateMenu(chatId, UserState.InMainMenu);
                    break;
                default:
                    //_menuLoader.SendText(chatId, CommandNotRecognized);
                    IncorrectCommandHandler(update, UserState.InStatusFree);
                    _menuLoader.LoadStateMenu(chatId, UserState.InStatusFree);
                    break;
            }
        }

        private void StatusOccupiedMenuHandler(Update update)
        {
            long chatId = _updateHelper.GetChatId(update);
            var command = GetCommand(update);
            switch (command)
            {
                case "/enqueue":
                    //LoadVisitDurationMenu(chatId); //QueueMenu(chatId);
                    _menuLoader.LoadStateMenu(chatId, UserState.InVisitDuration);
                    break;
                case "/exit":
                    _menuLoader.LoadStateMenu(chatId, UserState.InMainMenu);
                    break;
                default:
                    IncorrectCommandHandler(update, UserState.InStatusOccupied);
                    _menuLoader.LoadStateMenu(chatId, UserState.InStatusOccupied);
                    break;
            }
        }

        private void VisitDurationMenuHandler(Update update)
        {
            long chatId = _updateHelper.GetChatId(update);
            var command = GetCommand(update);
            switch (command)
            {
                case "/5minutes":
                    _menuLoader.LoadStateMenu(chatId, UserState.InQueue);
                    _room.Enqueue(chatId, 5);

                    break;
                case "/10minutes":
                    _menuLoader.LoadStateMenu(chatId, UserState.InQueue);
                    _room.Enqueue(chatId, 10);

                    break;
                case "/15minutes":
                    _menuLoader.LoadStateMenu(chatId, UserState.InQueue);
                    _room.Enqueue(chatId, 15);

                    break;
                case "/custom":

                    //LoadVisitDurationCustomMenu(chatId);
                    _menuLoader.LoadStateMenu(chatId, UserState.InVisitDurationCustom);
                    break;
                case "/exit":

                    _menuLoader.LoadStateMenu(chatId, UserState.InMainMenu);
                    break;
                default:
                    IncorrectCommandHandler(update, UserState.InVisitDuration);
                    _menuLoader.LoadStateMenu(chatId, UserState.InVisitDuration);
                    break;
            }
        }


        private void VisitDurationCustomMenuHandler(Update update)
        {

            long chatId = _updateHelper.GetChatId(update);
            if (update.Message.Text == "/exit")
            {
                _menuLoader.LoadStateMenu(chatId, UserState.InMainMenu);
                return;
            }

            int timeMinutes;
            int.TryParse(update.Message.Text, out timeMinutes);
            if (timeMinutes != 0 && timeMinutes < _config.Value.TimeLimitMinutes)
            {
                //_menuLoader.RemoveNotification(chatId);
                _menuLoader.LoadStateMenu(chatId, UserState.InQueue);
                _room.Enqueue(chatId, timeMinutes);
            }
            else
            {
                if (timeMinutes == 0)
                {
                    //_menuLoader.SendText(chatId, NumberNotParsed);
                    try
                    {
                        _logHelper.Log("JL54H6K45K645", $"NumberNotParsed is being sent to {chatId}", chatId,LogLevel.Warning);
                        //_menuLoader.SendNotification(chatId, NumberNotParsed);
                        _notificationRouter.RouteNotification(new NotificationItem(chatId, NotificationType.Send, NumberNotParsed));
                    }
                    catch (Exception ex)
                    {
                        _logHelper.Log("fdsf87ds6f87ds", $"NumberNotParsed notifcation failed. ChatId = {chatId}.\r\nException: {ex.Message}", chatId,LogLevel.Error);

                    }
                }
                else
                {

                    //_menuLoader.SendText(chatId, NumberOverLimit);
                    try
                    {
                        _logHelper.Log("HJGHGHJKFDS778", $"NumberOverLimit is being sent to {chatId}", chatId,LogLevel.Warning);
                        //_menuLoader.SendNotification(chatId, NumberOverLimit);
                        _notificationRouter.RouteNotification(new NotificationItem(chatId, NotificationType.Send, NumberOverLimit));
                    }
                    catch (Exception ex)
                    {

                        _logHelper.Log("dsfds2f3k4j432hj", $"NumberOverLimit notification failed. Chatid = {chatId}\r\nException: {ex.Message}", chatId,LogLevel.Error);
                    }

                }
                _menuLoader.LoadStateMenu(chatId, UserState.InVisitDurationCustom);
            }

        }

        private void QueueMenuHandler(Update update)
        {
            long chatId = _updateHelper.GetChatId(update);
            var command = GetCommand(update);
            switch (command)
            {
                case "/cancelenqueue":
                    _room.DequeueId(chatId);
                    _menuLoader.LoadStateMenu(chatId, UserState.InMainMenu);
                    break;
                case "/skip":
                    _room.SkipOrDequeue(chatId);
                    //_menuLoader.LoadStateMenu(chatId, UserState.InQueue);
                    break;
                case "/needmoretime":
                    _menuLoader.LoadStateMenu(chatId, UserState.InAddMoreTimeInTheQueue);

                    break;
                default:
                    IncorrectCommandHandler(update, UserState.InQueue);
                    _menuLoader.LoadStateMenu(chatId, UserState.InQueue);
                    break;
            }
        }

        private void RoomMenuHandler(Update update)
        {


            long chatId = _updateHelper.GetChatId(update);
            var command = GetCommand(update);
            switch (command)
            {
                case "/free":
                    _room.InitializeRoomDequeue();
                    //_room.cancellationTokenSource.Cancel();
                    //_room.Dequeue();
                    _menuLoader.LoadStateMenu(chatId, UserState.InMainMenu);
                    break;
                case "/needmoretime":
                    _menuLoader.LoadStateMenu(chatId, UserState.InAddMoreTimeInTheRoom);
                    break;
                case "/doorislocked":
                    //_menuLoader.SendText(chatId, doorIsLockedInstruction);
                    if (!_lockTrackerService.IsLockTrackerInstalled)
                    {
                        try
                        {
                            _logHelper.Log("FDSDS988DSFDS87", "Trying to send doorIsLockedInstruction");
                            //_menuLoader.SendNotification(chatId, doorIsLockedInstruction);
                            _notificationRouter.RouteNotification(new NotificationItem(chatId, NotificationType.Send, doorIsLockedInstruction));
                        }
                        catch (Exception ex)
                        {

                            _logHelper.Log("fdsfds7687fdsdsfds", $"DoorIsLockedInstruction notificatoin failed.\r\n{ex.Message}");
                        }
                        _menuLoader.LoadStateMenu(chatId, UserState.InAddMoreTimeInTheRoom);
                    }
                    else
                    {
                        _room.roomCancellationTokenSource.Cancel();
                        _room.EnqueueSomeoneInTheRoom(5);
                        _menuLoader.LoadStateMenu(chatId, UserState.InQueue);

                    }

                    break;
                default:
                    IncorrectCommandHandler(update, UserState.InTheRoom);
                    break;
            }

        }

        private void AddMoreTimeMenuHandler(Update update, UserState userState)
        {
            long chatId = _updateHelper.GetChatId(update);


            if (update.Message.Text == "/cancel")
            {
                if (userState == UserState.InAddMoreTimeInTheRoom)
                {
                    _menuLoader.LoadStateMenu(chatId, UserState.InTheRoom);
                }
                else
                {
                    _menuLoader.LoadStateMenu(chatId, UserState.InQueue);
                }

                return;
            }

            int timeMinutes;
            int.TryParse(update.Message.Text, out timeMinutes);
            if (timeMinutes != 0 && timeMinutes < _config.Value.TimeLimitMinutes)
            {
                _room.AddMoreTime(chatId, timeMinutes);
                if (userState == UserState.InAddMoreTimeInTheRoom)
                {
                    _menuLoader.LoadStateMenu(chatId, UserState.InTheRoom);
                }
                else
                {
                    _menuLoader.LoadStateMenu(chatId, UserState.InQueue);
                }

            }
            else
            {
                if (timeMinutes == 0)
                {
                    //_menuLoader.SendText(chatId, NumberNotParsed);
                    _notificationRouter.RouteNotification(new NotificationItem(chatId, NotificationType.Send, NumberNotParsed));
                }
                else
                {
                    _notificationRouter.RouteNotification(new NotificationItem(chatId, NotificationType.Send, NumberOverLimit));
                    //_menuLoader.SendText(chatId, NumberOverLimit);

                }
                _menuLoader.LoadStateMenu(chatId, userState);

            }



        }

        private void InBetweenQueueAndRoomMenuHandler(Update update)
        {
            long chatId = _updateHelper.GetChatId(update);

            var command = GetCommand(update);
            switch (command)
            {
                case "/startsession":
                    _room.CancelInBetweenTimer(toTheRoom: true);
                    _menuLoader.LoadStateMenu(chatId, UserState.InTheRoom);
                    break;
                case "/skip":
                    _room.CancelInBetweenTimer(toTheRoom: false);
                    _room.SkipOrDequeue(chatId);
                    //_menuLoader.LoadStateMenu(chatId, UserState.InQueue);
                    break;
                case "/dequeue":
                    _room.CancelInBetweenTimer(toTheRoom: false);
                    _room.DequeueId(chatId);
                    _menuLoader.LoadStateMenu(chatId, UserState.InMainMenu);
                    break;
                case "/doorislocked":
                    _room.CancelInBetweenTimer(toTheRoom: true);
                    //_menuLoader.SendText(chatId, doorIsLockedInstruction);
                    _notificationRouter.RouteNotification(new NotificationItem(chatId, NotificationType.Send, doorIsLockedInstruction));
                    _menuLoader.LoadStateMenu(chatId, UserState.InAddMoreTimeInTheRoom);
                    break;
                default:
                    IncorrectCommandHandler(update, UserState.InBetweenQueueAndRoom);
                    _menuLoader.LoadStateMenu(chatId, UserState.InBetweenQueueAndRoom);
                    break;
            }


        }



        private void InDoorIsLockedMenuHandler(Update update)
        {
            long chatId = _updateHelper.GetChatId(update);


        }

        private void IncorrectCommandHandler(Update update, UserState userState)
        {

            long chatId = _updateHelper.GetChatId(update);

            //_menuLoader.SendText(chatId, CommandNotRecognized);
            try
            {
                _logHelper.Log("FDS7FDSFDS7FDS", $"CommandNotRecognized notification sent to {chatId}", chatId,LogLevel.Warning);
                //_menuLoader.SendNotification(chatId, CommandNotRecognized);
                _notificationRouter.RouteNotification(new NotificationItem(chatId, NotificationType.Send, CommandNotRecognized));
            }
            catch (Exception ex)
            {

                _logHelper.Log("fdsd3234fds4343322ds", $"Failed to send CommandNotRecognized notification to {chatId}", chatId,LogLevel.Error);
            }

            _logger.LogError($"Command was not recognized. User state: {userState.ToString()} User input:|{GetCommand(update)}|");

        }


    }
    
}
