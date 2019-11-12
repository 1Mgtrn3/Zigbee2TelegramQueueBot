using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Zigbee2TelegramQueueBot.Configuration;
using Zigbee2TelegramQueueBot.Enums;
using Zigbee2TelegramQueueBot.Services.Bot;
using Zigbee2TelegramQueueBot.Services.Helpers.Log;
using Zigbee2TelegramQueueBot.Services.LockTracker;
using Zigbee2TelegramQueueBot.Services.Notifications;
using Zigbee2TelegramQueueBot.Services.Room.Queue;
using Zigbee2TelegramQueueBot.Services.Users;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;

namespace Zigbee2TelegramQueueBot.Services.Menu
{
    public class ButtonMenuLoader : IMenuLoader
    {
        private readonly IBotService _botService;
        private readonly IUsersService _users;
        private readonly IOptions<BotConfiguration> _config;
        private readonly ILockTrackerService _lockTrackerService;
        private readonly IRoomQueue _roomQueue;
        private readonly ILogHelper _logHelper;
        private readonly INotificationRouter _notificationRouter;
        public string MenuLoaderType => nameof(ButtonMenuLoader);
        public ButtonMenuLoader(IBotService botService,
            IOptions<BotConfiguration> config,
            IUsersService users,
            ILockTrackerService lockTrackerService,
            IRoomQueue roomQueue,
            ILogHelper logHelper,
            INotificationRouter notificationRouter)
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

            _logHelper.Log("NJ543JKL4J", $"Menu is loaded by : {caller} | Userstate : {userState.ToString()} | chatId : {chatId} | RemoveNotification : {removeNotification.ToString()}", LogLevel.Information);
            _users.UserInfo[chatId].State = userState;



            string menuText = "";
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>();
           
            if (removeNotification)
            {
               
                _notificationRouter.RouteNotification(new NotificationItem(chatId, NotificationType.Remove));
            }
            switch (userState)
            {
                case UserState.InMainMenu:
                    
                    menuText = LoadMainMenuText();
                    buttons = LoadMainMenuButtons();
                    break;
                case UserState.InStatusFree:
                    menuText = LoadStatusFreeMenuText();
                    buttons = LoadStatusFreeMenuButtons();
                    break;
                case UserState.InStatusOccupied:
                    menuText = LoadStatusOccupiedMenuText();
                    buttons = LoadStatusOccupiedMenuButtons();
                    break;
                case UserState.InVisitDuration:
                    menuText = LoadVisitDurationMenuText();
                    buttons = LoadVisitDurationMenuButtons();
                    break;
                case UserState.InVisitDurationCustom:
                    menuText = LoadVisitDurationCustomMenuText();
                    buttons = LoadVisitDurationCustomMenuButtons();
                    break;
                case UserState.InQueue:
                    menuText = LoadQueueMenuText();
                    buttons = LoadQueueMenuButtons();
                    break;
                case UserState.InTheRoom:

                    menuText = LoadRoomMenuText();
                    buttons = LoadRoomMenuButtons();
                    break;
                case UserState.InAddMoreTimeInTheRoom:
                    menuText = LoadAddMoreTimeInTheRoomMenuText();
                    buttons = LoadAddMoreTimeInTheRoomMenuButtons();
                    break;
                case UserState.InAddMoreTimeInTheQueue:
                    menuText = LoadAddMoreTimeInTheQueueMenuText();
                    buttons = LoadAddMoreTimeInTheQueueMenuButtons();
                    break;
                case UserState.InBetweenQueueAndRoom:
                    menuText = LoadInBetweenQueueAndRoomMenuText();
                    buttons = LoadInBetweenQueueAndRoomMenuButtons();
                    break;
                case UserState.InDoorIsLocked:
                    menuText = LoadDoorIsLockedMenuText();
                    buttons = LoadDoorIsLockedMenuButtons();
                    break;
                default:
                    menuText = $"Command not implemented yet. userState = {userState.ToString()}";
                    break;
            }

            var markup = GetMarkup(buttons);
            await SendMenu(chatId, menuText, markup);
        }

        public List<InlineKeyboardButton> LoadMainMenuButtons()
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton> { };
            

            var button1 = new InlineKeyboardButton();
            button1.Text = "Status";
            button1.CallbackData = "/status";
            buttons.Add(button1);

            var button2 = new InlineKeyboardButton();
            button2.Text = "Enqueue";
            button2.CallbackData = "/enqueue";
            buttons.Add(button2);

            return buttons;

        }

        public List<InlineKeyboardButton> LoadStatusFreeMenuButtons()
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton> { };
            

            var button1 = new InlineKeyboardButton();
            button1.Text = "Occupy";
            button1.CallbackData = "/occupy";
            buttons.Add(button1);

            var button2 = new InlineKeyboardButton();
            button2.Text = "Exit";
            button2.CallbackData = "/exit";
            buttons.Add(button2);

            return buttons;

        }

        public List<InlineKeyboardButton> LoadStatusOccupiedMenuButtons()
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton> { };
            

            var button1 = new InlineKeyboardButton();
            button1.Text = "Enqueue";
            button1.CallbackData = "/enqueue";
            buttons.Add(button1);

            var button2 = new InlineKeyboardButton();
            button2.Text = "Exit";
            button2.CallbackData = "/exit";
            buttons.Add(button2);

            return buttons;

        }

        public List<InlineKeyboardButton> LoadVisitDurationMenuButtons()
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton> { };
            

            var button1 = new InlineKeyboardButton();
            button1.Text = "5 minutes";
            button1.CallbackData = "/5minutes";
            buttons.Add(button1);

            var button2 = new InlineKeyboardButton();
            button2.Text = "10 minutes";
            button2.CallbackData = "/10minutes";
            buttons.Add(button2);

            var button3 = new InlineKeyboardButton();
            button3.Text = "15 minutes";
            button3.CallbackData = "/15minutes";
            buttons.Add(button3);

            var button4 = new InlineKeyboardButton();
            button4.Text = "Set custom";
            button4.CallbackData = "/custom";
            buttons.Add(button4);

            var button5 = new InlineKeyboardButton();
            button5.Text = "Exit";
            button5.CallbackData = "/exit";
            buttons.Add(button5);

            return buttons;

        }


        public List<InlineKeyboardButton> LoadVisitDurationCustomMenuButtons()
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton> { };
            
            var button1 = new InlineKeyboardButton();
            button1.Text = "Exit";
            button1.CallbackData = "/exit";
            buttons.Add(button1);

            return buttons;

        }

        public List<InlineKeyboardButton> LoadQueueMenuButtons()
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton> { };
            

            var button1 = new InlineKeyboardButton();
            button1.Text = "Cancel enqueue";
            button1.CallbackData = "/cancelenqueue";
            buttons.Add(button1);

            var button2 = new InlineKeyboardButton();
            button2.Text = "Skip";
            button2.CallbackData = "/skip";
            buttons.Add(button2);

            var button3 = new InlineKeyboardButton();
            button3.Text = "Need more time";
            button3.CallbackData = "/needmoretime";
            buttons.Add(button3);



            return buttons;

        }

        public List<InlineKeyboardButton> LoadRoomMenuButtons()
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton> { };
           

            if (!_lockTrackerService.IsLockTrackerInstalled)
            {
                var button1 = new InlineKeyboardButton();
                button1.Text = "Free";
                button1.CallbackData = "/free";
                buttons.Add(button1);
            }


            var button2 = new InlineKeyboardButton();
            button2.Text = "Door is locked";
            button2.CallbackData = "/doorislocked";
            buttons.Add(button2);

            var button3 = new InlineKeyboardButton();
            button3.Text = "Need more time";
            button3.CallbackData = "/needmoretime";
            buttons.Add(button3);



            return buttons;

        }


        public List<InlineKeyboardButton> LoadAddMoreTimeInTheRoomMenuButtons()
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton> { };
            
            var button1 = new InlineKeyboardButton();
            button1.Text = "Cancel";
            button1.CallbackData = "/cancel";
            buttons.Add(button1);

            return buttons;

        }

        public List<InlineKeyboardButton> LoadAddMoreTimeInTheQueueMenuButtons()
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton> { };

            var button1 = new InlineKeyboardButton();
            button1.Text = "Cancel";
            button1.CallbackData = "/cancel";
            buttons.Add(button1);

            return buttons;

        }



        public List<InlineKeyboardButton> LoadInBetweenQueueAndRoomMenuButtons()
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton> { };
            
            if (!_lockTrackerService.IsLockTrackerInstalled)
            {
                var button1 = new InlineKeyboardButton();
                button1.Text = "Start session";
                button1.CallbackData = "/startsession";
                buttons.Add(button1);

                var button4 = new InlineKeyboardButton();
                button4.Text = "Door is locked";
                button4.CallbackData = "/doorislocked";
                buttons.Add(button4);
            }



            var button2 = new InlineKeyboardButton();
            button2.Text = "Skip";
            button2.CallbackData = "/skip";
            buttons.Add(button2);

            var button3 = new InlineKeyboardButton();
            button3.Text = "Dequeue";
            button3.CallbackData = "/dequeue";
            buttons.Add(button3);





            return buttons;

        }

        public List<InlineKeyboardButton> LoadDoorIsLockedMenuButtons()
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton> { };
            
            var button1 = new InlineKeyboardButton();
            button1.Text = "Cancel";
            button1.CallbackData = "/cancel";
            buttons.Add(button1);

            return buttons;

        }


        //TODO: Visit Duration Custom Menu

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

        public string LoadMainMenuText()
        {

            string menuText = _config.Value.MenuTexts.MainMenuText;

            //text assembly
            return menuText;

        }

        private string LoadStatusFreeMenuText()
        {

            string menuText = _config.Value.MenuTexts.StatusFreeMenuText;
            //text assembly
            return menuText;

        }

        private string LoadStatusOccupiedMenuText()
        {
            string menuText = _config.Value.MenuTexts.StatusOccupiedMenuText
                .Replace("[QUEUESIZE]", _roomQueue.QueueList.Count().ToString())
                .Replace("[WAITMINUTES]", _roomQueue.GetOverallWaitTimeMinutes().ToString());


            //text assembly
            return menuText;

        }

        private string LoadVisitDurationMenuText()
        {
            string menuText = _config.Value.MenuTexts.VisitDurationMenuText;
            //text assembly
            return menuText;

        }

        private string LoadVisitDurationCustomMenuText()
        {
            string menuText = _config.Value.MenuTexts.VisitDurationCustomMenuText;
            //text assembly
            return menuText;

        }

        private string LoadQueueMenuText()
        {
            string menuText = _config.Value.MenuTexts.QueueMenuText;
            //text assembly
            return menuText;

        }

        private string LoadRoomMenuText()
        {
            string menuText = _config.Value.MenuTexts.RoomMenuText;
            //text assembly
            return menuText;

        }

        private string LoadAddMoreTimeInTheRoomMenuText()
        {
            string menuText = _config.Value.MenuTexts.AddMoreTimeInTheRoomMenuText;
            //text assembly
            return menuText;
        }

        private string LoadAddMoreTimeInTheQueueMenuText()
        {
            string menuText = _config.Value.MenuTexts.AddMoreTimeInTheQueueMenuText;
            //text assembly
            return menuText;
        }

        private string LoadInBetweenQueueAndRoomMenuText()
        {
            string menuText = _config.Value.MenuTexts.InBetweenQueueAndRoomMenuText;
            //text assembly
            return menuText;
        }

        private string LoadDoorIsLockedMenuText()
        {
            string menuText = _config.Value.MenuTexts.DoorIsLockedMenuText;
            //text assembly
            return menuText;
        }

        
        private async Task SendMenu(long chatId, string text, InlineKeyboardMarkup buttons)
        {
            if (_users.UserInfo[chatId].LastMessageId == default(int))
            {
                await _botService.Client.SendTextMessageAsync(chatId, text, ParseMode.Html, true, false, 0, buttons);
            }
            else
            {
                try
                {
                    await _botService.Client.EditMessageTextAsync(chatId, _users.UserInfo[chatId].LastMessageId, text, ParseMode.Html, true, buttons);
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

        }


        public async Task SendText(long chatId, string text)
        {
            await _botService.Client.SendTextMessageAsync(chatId, text);
            //throw new NotImplementedException();

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
                //var messageId = _users.UserInfo[chatId].LastNotificationMessageId;
                //await _botService.Client.EditMessageTextAsync(chatId, messageId, text);
                //await RemoveNotification(chatId);
                _notificationRouter.RouteNotification(new NotificationItem(chatId, NotificationType.Remove));
                var message = await _botService.Client.SendTextMessageAsync(chatId, text);
                _users.UserInfo[chatId].LastNotificationMessageId = message.MessageId;
            }
            //throw new NotImplementedException();
        }

        public async Task RemoveNotification(long chatId, [CallerMemberName] string caller = null)
        {

            int? messageId = _users.UserInfo[chatId]?.LastNotificationMessageId;
            _logHelper.Log("HJ43K2H4K", $"Last Notification Removal initialized. Last Notification MessageId = {messageId}. Caller : {caller}", chatId, LogLevel.Information);
            if (messageId != default(int?) && messageId != null && messageId != 0)
            {
                int _messageId = (int)messageId;

                //var messageId = _users.UserInfo[chatId].LastNotificationMessageId;
                await _botService.Client.DeleteMessageAsync(chatId, _messageId).ConfigureAwait(false);
                _logHelper.Log("HJ4sdfdsK", $"Notification successfully deleted", chatId, LogLevel.Information);
                _users.UserInfo[chatId].LastNotificationMessageId = default(int);
            }

        }
    }
}
