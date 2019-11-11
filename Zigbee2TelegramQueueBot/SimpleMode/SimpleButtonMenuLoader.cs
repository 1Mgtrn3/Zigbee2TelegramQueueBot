using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Zigbee2TelegramQueueBot.Configuration;
using Zigbee2TelegramQueueBot.Enums;
using Zigbee2TelegramQueueBot.Services.Helpers.Log;
using Zigbee2TelegramQueueBot.Services.Menu;
using Zigbee2TelegramQueueBot.Services.Room.Queue;
using Zigbee2TelegramQueueBot.Services.Users;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;

namespace Zigbee2TelegramQueueBot.SimpleMode
{
    public class SimpleButtonMenuLoader : IMenuLoader
    {
        private readonly IBotService _botService;
        private readonly IUsersService _users;
        private readonly IOptions<BotConfiguration> _config;
        private readonly ILockTrackerService _lockTrackerService;
        private readonly IRoomQueue _roomQueue;
        //private readonly IRoom _room;
        private readonly ILogHelper _logHelper;
        private readonly INotificationRouter _notificationRouter;
        public string MenuLoaderType => nameof(SimpleButtonMenuLoader);

        public SimpleButtonMenuLoader(IBotService botService,
            IOptions<BotConfiguration> config,
            IUsersService users,
            ILockTrackerService lockTrackerService,
            IRoomQueue roomQueue,
            ILogHelper logHelper,
            INotificationRouter notificationRouter
            )
        {
            _botService = botService;
            _config = config;
            _users = users;
            _lockTrackerService = lockTrackerService;
            _roomQueue = roomQueue;
            _logHelper = logHelper;            
            _notificationRouter = notificationRouter;

        }
        public async Task LoadStateMenu(long chatId, UserState userState, bool removeNotification = true, [CallerMemberName] string caller = null)
        {
            _logHelper.Log("FDSFDS78S8S", $"Menu is loaded by : {caller} | Userstate : {userState.ToString()} | chatId : {chatId} | RemoveNotification : {removeNotification.ToString()}",LogLevel.Information);
            _users.UserInfo[chatId].State = userState;

            string menuText = "";
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
            //throw new NotImplementedException();
            if (removeNotification)
            {
                //await RemoveNotification(chatId);
                _notificationRouter.RouteNotification(new NotificationItem(chatId, NotificationType.Remove));
            }

            switch (userState)
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
                    menuText = LoadSimpleMainMenuText();
                    buttons = LoadSimpleMainMenuButtons();
                    break;
                case UserState.InSimpleStatus:
                    menuText = LoadSimpleStatusMenuText();
                    buttons = LoadSimpleStatusMenuButtons();
                    break;
                case UserState.InSimpleSubscribed:
                    menuText = LoadSimpleSubscribedMenuText();
                    buttons = LoadSimpleSubscribedMenuButtons();
                    break;
                default:
                    menuText = $"Command not implemented yet. userState = {userState.ToString()}";
                    break;
            }

            var markup = GetMarkup(buttons);
            await SendMenu(chatId, menuText, markup);
            //
        }

        private List<InlineKeyboardButton> LoadSimpleSubscribedMenuButtons()
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton> { };
            var button2 = new InlineKeyboardButton();
            button2.Text = "Exit";
            button2.CallbackData = "/exit";
            buttons.Add(button2);

            return buttons;
        }

        private string LoadSimpleSubscribedMenuText()
        {
            string menuText = _config.Value.SimpleMenuTexts.SimpleSubscribedMenuText;//.MenuTexts.VisitDurationMenuText;
            //text assembly
            return menuText;
        }

        private List<InlineKeyboardButton> LoadSimpleStatusMenuButtons()
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton> { };            

            var button2 = new InlineKeyboardButton();
            button2.Text = "Exit";
            button2.CallbackData = "/exit";
            buttons.Add(button2);

            return buttons;
        }

        private string LoadSimpleStatusMenuText()
        {
            string menuText = _config.Value.SimpleMenuTexts.SimpleStatusMenuText;//.MenuTexts.VisitDurationMenuText;
            //text assembly
            string roomStatusString;// = _roomQueue.QueueList[0].?"занята":"свободна";
            if (_roomQueue.QueueList.SingleOrDefault(s => s.ChatId == 0) == default(QueueSlot))
            {
                roomStatusString = "free";
            }
            else
            {
                roomStatusString = "occupied";

            }

            int numberOfSubsRaw = _roomQueue.QueueList.Count();
            string numberOfSubs = "";
            if (numberOfSubsRaw == 0)
            {
                numberOfSubs = "0";
            }
            else
            {
                numberOfSubs = (numberOfSubsRaw - 1).ToString();
            }
            //_roomQueue.QueueList.Count().ToString();
            menuText = menuText.Replace("[ROOMSTATUS]", roomStatusString)
                .Replace("[SUBCOUNT]", numberOfSubs);
            return menuText;
        }

        private async Task SendMenu(long chatId, string menuText, InlineKeyboardMarkup buttons)
        {
            if (_users.UserInfo[chatId].LastMessageId == default(int))
            {
                await _botService.Client.SendTextMessageAsync(chatId, menuText, ParseMode.Html, true, false, 0, buttons);
            }
            else
            {
                try
                {
                    await _botService.Client.EditMessageTextAsync(chatId, _users.UserInfo[chatId].LastMessageId, menuText, ParseMode.Html, true, buttons);
                }
                catch (Exception ex)
                {
                    _logHelper.Log("FDKJ34JK43909", "Could not edit a message",LogLevel.Error);
                    await _botService.Client.SendTextMessageAsync(chatId, menuText, ParseMode.Html, true, false, 0, buttons);
                }
            }
        }

        public InlineKeyboardMarkup GetMarkup(List<InlineKeyboardButton> buttons)
        {
            var tmpMenu = new List<InlineKeyboardButton[]>();

            foreach (var button in buttons)
            {
                tmpMenu.Add(new[] { button });
            }

            var resultMenu = new InlineKeyboardMarkup(tmpMenu);

            return resultMenu;

        }

        private List<InlineKeyboardButton> LoadSimpleMainMenuButtons()
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton> { };
            //buttons.Add(new InlineKeyboardButton("");
            var button1 = new InlineKeyboardButton();
            button1.Text = "Status";//"/status\r\n/enqueue"
            button1.CallbackData = "/status";
            buttons.Add(button1);

            var button2 = new InlineKeyboardButton();
            button2.Text = "Subscribe";
            button2.CallbackData = "/subscribe";
            buttons.Add(button2);

            return buttons;
        }

        private string LoadSimpleMainMenuText()
        {
            string menuText = _config.Value.SimpleMenuTexts.SimpleMainMenuText;
            //text assembly
            return menuText;
        }
    }
}
