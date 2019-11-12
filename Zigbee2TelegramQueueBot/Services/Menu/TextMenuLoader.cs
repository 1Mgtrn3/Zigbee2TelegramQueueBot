using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Zigbee2TelegramQueueBot.Configuration;
using Zigbee2TelegramQueueBot.Enums;
using Zigbee2TelegramQueueBot.Services.Bot;
using Zigbee2TelegramQueueBot.Services.Users;

namespace Zigbee2TelegramQueueBot.Services.Menu
{
    public class TextMenuLoader : IMenuLoader
    {
        private readonly IUsersService _users;
        private readonly IBotService _botService;
        private readonly IOptions<BotConfiguration> _config;

        public string MenuLoaderType => nameof(TextMenuLoader);

        public TextMenuLoader(IBotService botService, IOptions<BotConfiguration> config, IUsersService users)
        {
            _users = users;
            _botService = botService;
            _config = config;
        }
        public async Task LoadStateMenu(long chatId, UserState userState, bool removeNotification = true, [CallerMemberName] string caller = null)
        {
            _users.UserInfo[chatId].State = userState;
            string menuText = "";
            switch (userState)
            {
                case UserState.InMainMenu:
                    menuText = LoadMainMenuText();
                    break;
                case UserState.InStatusFree:
                    menuText = LoadStatusFreeMenuText();
                    break;
                case UserState.InStatusOccupied:
                    menuText = LoadStatusOccupiedMenuText();
                    break;
                case UserState.InVisitDuration:
                    menuText = LoadVisitDurationMenuText();
                    break;
                case UserState.InVisitDurationCustom:
                    menuText = LoadVisitDurationCustomMenuText();
                    break;
                case UserState.InQueue:
                    menuText = LoadQueueMenuText();
                    break;
                case UserState.InTheRoom:
                    menuText = LoadRoomMenuText();
                    break;
                case UserState.InAddMoreTimeInTheRoom:
                    menuText = LoadAddMoreTimeInTheRoomMenuText();
                    break;
                case UserState.InAddMoreTimeInTheQueue:
                    menuText = LoadAddMoreTimeInTheQueueMenuText();
                    break;
                case UserState.InBetweenQueueAndRoom:
                    menuText = LoadInBetweenQueueAndRoomMenuText();
                    break;
                case UserState.InDoorIsLocked:
                    menuText = LoadDoorIsLockedMenuText();
                    break;
                default:
                    menuText = $"Command not implemented yet. userState = {userState.ToString()}";
                    break;
            }

            await SendText(chatId, menuText);

        }


        public string LoadMainMenuText()
        {

            string menuText = _config.Value.MenuTexts.MainMenuText + _config.Value.MenuCommands.MainMenuCommands;

            //text assembly
            return menuText;

        }

        private string LoadStatusFreeMenuText()
        {

            string menuText = _config.Value.MenuTexts.StatusFreeMenuText + _config.Value.MenuCommands.StatusFreeMenuCommands;
            //text assembly
            return menuText;

        }

        private string LoadStatusOccupiedMenuText()
        {
            string menuText = _config.Value.MenuTexts.StatusOccupiedMenuText + _config.Value.MenuCommands.StatusOccupiedMenuCommands;
            //text assembly
            return menuText;

        }

        private string LoadVisitDurationMenuText()
        {
            string menuText = _config.Value.MenuTexts.VisitDurationMenuText + _config.Value.MenuCommands.VisitDurationMenuCommands;
            //text assembly
            return menuText;

        }

        private string LoadVisitDurationCustomMenuText()
        {
            string menuText = _config.Value.MenuTexts.VisitDurationCustomMenuText + _config.Value.MenuCommands.VisitDurationCustomMenuCommands;
            //text assembly
            return menuText;

        }

        private string LoadQueueMenuText()
        {
            string menuText = _config.Value.MenuTexts.QueueMenuText + _config.Value.MenuCommands.QueueMenuCommands;
            //text assembly
            return menuText;

        }

        private string LoadRoomMenuText()
        {
            string menuText = _config.Value.MenuTexts.RoomMenuText + _config.Value.MenuCommands.RoomMenuCommands;
            //text assembly
            return menuText;

        }

        private string LoadAddMoreTimeInTheRoomMenuText()
        {
            string menuText = _config.Value.MenuTexts.AddMoreTimeInTheRoomMenuText + _config.Value.MenuCommands.AddMoreTimeInTheRoomMenuCommands;
            //text assembly
            return menuText;
        }

        private string LoadAddMoreTimeInTheQueueMenuText()
        {
            string menuText = _config.Value.MenuTexts.AddMoreTimeInTheQueueMenuText + _config.Value.MenuCommands.AddMoreTimeInTheQueueMenuCommands;
            //text assembly
            return menuText;
        }

        private string LoadInBetweenQueueAndRoomMenuText()
        {
            string menuText = _config.Value.MenuTexts.InBetweenQueueAndRoomMenuText + _config.Value.MenuCommands.InBetweenQueueAndRoomMenuCommands;
            //text assembly
            return menuText;
        }

        private string LoadDoorIsLockedMenuText()
        {
            string menuText = _config.Value.MenuTexts.DoorIsLockedMenuText + _config.Value.MenuCommands.DoorIsLockedMenuCommands;
            //text assembly
            return menuText;
        }

        public async Task SendText(long chatId, string text)
        {
            await _botService.Client.SendTextMessageAsync(chatId, text);
        }

        public async Task SendNotification(long chatId, string text)
        {
            if (_users.UserInfo[chatId].LastNotificationMessageId == default(int))
            {
                var message = await _botService.Client.SendTextMessageAsync(chatId, text);
                _users.UserInfo[chatId].LastNotificationMessageId = message.MessageId;
            }
            else
            {
                var messageId = _users.UserInfo[chatId].LastNotificationMessageId;
                await _botService.Client.EditMessageTextAsync(chatId, messageId, text);
            }
            //throw new NotImplementedException();
        }

        public async Task RemoveNotification(long chatId, [CallerMemberName] string caller = null)
        {
            var messageId = _users.UserInfo[chatId].LastNotificationMessageId;
            await _botService.Client.DeleteMessageAsync(chatId, messageId);
        }
    }
}
