using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Zigbee2TelegramQueueBot.Enums;
using Zigbee2TelegramQueueBot.Services.Bot;
using Zigbee2TelegramQueueBot.Services.Helpers.Log;
using Zigbee2TelegramQueueBot.Services.Helpers.UpdateServiceHelper;
using Zigbee2TelegramQueueBot.Services.Menu;
using Zigbee2TelegramQueueBot.Services.Notifications;
using Zigbee2TelegramQueueBot.Services.Room;
using Zigbee2TelegramQueueBot.Services.Session;
using Zigbee2TelegramQueueBot.Services.Users;
using Microsoft.Extensions.Logging;

namespace Zigbee2TelegramQueueBot.Services.TelegramUpdateHandler
{
    public class UpdateService : IUpdateService
    {
        private readonly IBotService _botService;
        private readonly ILogHelper _logHelper;
        private readonly IRoomService _room;
        private readonly IUsersService _users;
        private readonly ISessionRouter _sessionRouter;
        private readonly IMenuLoader _menuLoader;
        private readonly IUpdateHelper _updateHelper;
        private readonly INotificationRouter _notificationRouter;

        public UpdateService(IBotService botService,
                            //ILogger<UpdateService> logger,
                            ILogHelper logHelper,
                            IRoomService room,
                            IUsersService users,
                            ISessionRouter sessionRouter,
                            IMenuLoader menuLoader,
                            IUpdateHelper updateHelper,
            INotificationRouter notificationRouter)
        {
            _botService = botService;
            _logHelper = logHelper;
            _room = room;
            _users = users;
            _sessionRouter = sessionRouter;
            _menuLoader = menuLoader;
            _updateHelper = updateHelper;
            _notificationRouter = notificationRouter;
        }
        


        public async Task UpdateHandle(Update update)
        {

            if (update.Type == UpdateType.Message || update.Type == UpdateType.CallbackQuery)
            {
                var chatId = _updateHelper.GetChatId(update);

                if (!_users.UserInfo.ContainsKey(chatId))
                {
                    _users.UserInfo.Add(chatId, new UserInfo() { UserName = _updateHelper.GetUserName(update) });
                    _logHelper.Log("L5NB54645KJJKEW2", $"MENU LOADER TYPE: {_menuLoader.MenuLoaderType}", LogLevel.Information);
                    
                    if (_menuLoader.MenuLoaderType == "SimpleButtonMenuLoader")
                    {

                        await _menuLoader.LoadStateMenu(chatId, UserState.InSimpleMainMenu);
                    }
                    else
                    {
                        await _menuLoader.LoadStateMenu(chatId, UserState.InMainMenu);
                    }


                }
                else
                {

                    if (_menuLoader.MenuLoaderType == "SimpleButtonMenuLoader")
                    {
                        if (update.Type == UpdateType.Message)
                        {
                            _notificationRouter.RouteNotification(new NotificationItem(chatId, NotificationType.Remove, update.Message.MessageId.ToString()));
                            try
                            {
                                _room.DequeueId(chatId);
                            }
                            catch (Exception)
                            {


                            }
                            await _menuLoader.LoadStateMenu(chatId, UserState.InSimpleMainMenu);

                        }
                        else
                        {
                            _sessionRouter.RouteUpdate(update);
                        }

                    }
                    else
                    {
                        _sessionRouter.RouteUpdate(update);
                    }


                }

            }
            else
            {



                return;

            }


        }
    }
}
