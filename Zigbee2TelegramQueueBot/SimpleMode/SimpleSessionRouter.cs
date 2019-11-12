using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Zigbee2TelegramQueueBot.Enums;
using Zigbee2TelegramQueueBot.Services.Helpers.Log;
using Zigbee2TelegramQueueBot.Services.Helpers.UpdateServiceHelper;
using Zigbee2TelegramQueueBot.Services.Room;
using Zigbee2TelegramQueueBot.Services.Session;
using Zigbee2TelegramQueueBot.Services.Users;
using Microsoft.Extensions.Logging;
using Zigbee2TelegramQueueBot.Services.Bot;
using Zigbee2TelegramQueueBot.Services.Menu;
using Zigbee2TelegramQueueBot.Services.Notifications;

namespace Zigbee2TelegramQueueBot.SimpleMode
{
    public class SimpleSessionRouter : ISessionRouter
    {

        private readonly IUsersService _users;
        private readonly IRoomService _room;
        private readonly IBotService _botService;        
        private readonly IUpdateHelper _updateHelper;
        private readonly ILockTrackerService _lockTrackerService;
        private readonly ILogHelper _logHelper;
        private readonly INotificationRouter _notificationRouter;
        private readonly IMenuLoader _menuLoader;

        string CommandNotRecognized = "Command wasn't recognized. Please try again.";
        public SimpleSessionRouter(IUpdateHelper updateHelper,
                            IMenuLoader menuLoader,

                            IRoom room,
                            IUsersService users,
                            IBotService botService,

                            ILockTrackerService lockTrackerService,
                            ILogHelper logHelper,
                            INotificationRouter notificationRouter)
        {

            
            _menuLoader = menuLoader;

            _logHelper = logHelper;

            _updateHelper = updateHelper;
            _notificationRouter = notificationRouter;
            
            _users = users;

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
                    break;
                case UserState.InStatusFree:
                    break;
                case UserState.InStatusOccupied:
                    break;
                case UserState.InVisitDuration:
                    break;
                case UserState.InVisitDurationCustom:
                    break;
                case UserState.InQueue:
                    break;
                case UserState.InTheRoom:
                    break;
                case UserState.InAddMoreTimeInTheRoom:
                    break;
                case UserState.InAddMoreTimeInTheQueue:
                    break;
                case UserState.InBetweenQueueAndRoom:
                    break;
                case UserState.InDoorIsLocked:
                    break;
                case UserState.InSimpleMainMenu:
                    SimpleMainMenuHandler(update);
                    break;
                case UserState.InSimpleStatus:
                    SimpleStatusMenuHandler(update);
                    break;
                case UserState.InSimpleSubscribed:
                    SimpleSubscribedMenuHandler(update);
                    break;
                default:
                    break;
            }
        }

        private void SimpleSubscribedMenuHandler(Update update)
        {
            long chatId = _updateHelper.GetChatId(update);
            var command = GetCommand(update);
            switch (command)
            {

                case "/exit":
                    _room.DequeueId(chatId);
                    _menuLoader.LoadStateMenu(chatId, UserState.InSimpleMainMenu);
                    break;
                default:
                    IncorrectCommandHandler(update, UserState.InSimpleSubscribed);
                    _menuLoader.LoadStateMenu(chatId, UserState.InSimpleSubscribed);
                    break;
            }
        }

        private void SimpleStatusMenuHandler(Update update)
        {
            long chatId = _updateHelper.GetChatId(update);
            var command = GetCommand(update);
            switch (command)
            {
                case "/reload":
                    _menuLoader.LoadStateMenu(chatId, UserState.InSimpleStatus);
                    break;
                case "/exit":
                    _menuLoader.LoadStateMenu(chatId, UserState.InSimpleMainMenu);
                    break;
                default:
                    IncorrectCommandHandler(update, UserState.InSimpleStatus);
                    _menuLoader.LoadStateMenu(chatId, UserState.InSimpleStatus);
                    break;
            }
        }

        private void SimpleMainMenuHandler(Update update)
        {
            long chatId = _updateHelper.GetChatId(update);
            var command = GetCommand(update);
            switch (command)
            {
                case "/status":
                    _menuLoader.LoadStateMenu(chatId, UserState.InSimpleStatus);
                    break;
                case "/subscribe":
                    _room.Enqueue(chatId);
                    _menuLoader.LoadStateMenu(chatId, UserState.InSimpleSubscribed);
                    break;
                default:
                    IncorrectCommandHandler(update, UserState.InSimpleMainMenu);
                    _menuLoader.LoadStateMenu(chatId, UserState.InSimpleMainMenu);
                    break;
            }
        }

        private void IncorrectCommandHandler(Update update, UserState userState)
        {

            long chatId = _updateHelper.GetChatId(update);

            //_menuLoader.SendText(chatId, CommandNotRecognized);
            try
            {
                _logHelper.Log("df798sfs98", $"CommandNotRecognized notification sent to {chatId}", chatId, LogLevel.Warning);
                //_menuLoader.SendNotification(chatId, CommandNotRecognized);
                _notificationRouter.RouteNotification(new NotificationItem(chatId, NotificationType.Send, CommandNotRecognized));
            }
            catch (Exception ex)
            {

                _logHelper.Log("fdsd3234fds4s", $"Failed to send CommandNotRecognized notification to {chatId}", chatId, LogLevel.Error);
            }

            _logHelper.Log("F98DSJ98HJ9HJG", $"Command was not recognized. User state: {userState.ToString()} User input:|{GetCommand(update)}|");

        }
    }
}
