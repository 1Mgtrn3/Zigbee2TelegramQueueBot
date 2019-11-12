using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zigbee2TelegramQueueBot.Services.Bot;
using Zigbee2TelegramQueueBot.Services.Helpers.Log;
using Zigbee2TelegramQueueBot.Services.Users;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Zigbee2TelegramQueueBot.Services.Notifications
{
    public class NotificationService : IHostedService, IDisposable
    {
        private readonly IUsersService _users;
        private readonly ILogHelper _logHelper;
        private readonly IBotService _botService;
        public NotificationService(IUsersService users, ILogHelper logHelper, IBotService botService)
        {
            _users = users;
            _logHelper = logHelper;
            _botService = botService;
        }

        public void CheckQueues(object state)
        {
            
            foreach (var userInfo in _users.UserInfo.Values)
            {
                if (!userInfo.NotificationsQueue.IsEmpty && userInfo.NotificationProcessingCompleted)
                {
                    _logHelper.Log("FDS8F9DSKJK", $"Found messages in queues. Username {userInfo.UserName}", LogLevel.Information) ;
                    Task.Run(() => ProcessUserNotifications(userInfo));
                }
            }

        }

        public async void ProcessUserNotifications(UserInfo userInfo)
        {
            //userInfo.
            userInfo.NotificationProcessingCompleted = false;
            while (!userInfo.NotificationsQueue.IsEmpty)
            {
                NotificationItem notificationItem = new NotificationItem(); ;
                bool isSuccessful = userInfo.NotificationsQueue.TryDequeue(out notificationItem);
                if (isSuccessful)
                {
                    switch (notificationItem.NotificationType)
                    {
                        case Enums.NotificationType.Send:
                            await SendNotification(notificationItem.ChatId, notificationItem.Data);
                            break;
                        case Enums.NotificationType.Remove:
                            try
                            {
                                if (notificationItem.Data != "")
                                {
                                    int messageIdUser;
                                    messageIdUser = int.Parse(notificationItem.Data);
                                    //= notificationItem.Data;
                                    await RemoveNotification(notificationItem.ChatId, messageIdUser);
                                }
                                else
                                {
                                    await RemoveNotification(notificationItem.ChatId);
                                }
                            }
                            catch (Exception ex)
                            {

                                _logHelper.Log("FD8S9JK3K3KN5", "Cannot remove a message", LogLevel.Error);
                            }

                            break;
                        default:
                            break;
                    }



                }

            }
            userInfo.NotificationProcessingCompleted = true;
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
                await RemoveNotification(chatId);
                var message = await _botService.Client.SendTextMessageAsync(chatId, text);
                _users.UserInfo[chatId].LastNotificationMessageId = message.MessageId;
            }
            //throw new NotImplementedException();
        }

        public async Task RemoveNotification(long chatId, int messageIdUser = 0, [CallerMemberName] string caller = null)
        {
            if (messageIdUser == 0)
            {
                int? messageIdBot = _users.UserInfo[chatId]?.LastNotificationMessageId;
                _logHelper.Log("K53J5K3K4", $"Last Notification Removal initialized. Last Notification MessageId = {messageIdBot}. Caller : {caller}", chatId, LogLevel.Information);
                if (messageIdBot != default(int?) && messageIdBot != null && messageIdBot != 0)
                {
                    int _messageId = (int)messageIdBot;

                    //var messageId = _users.UserInfo[chatId].LastNotificationMessageId;
                    await _botService.Client.DeleteMessageAsync(chatId, _messageId).ConfigureAwait(false);
                    _logHelper.Log("7FDF7DFDS98", $"Notification successfully deleted", chatId,LogLevel.Information);
                    _users.UserInfo[chatId].LastNotificationMessageId = default(int);
                }
            }
            else
            {
                await _botService.Client.DeleteMessageAsync(chatId, messageIdUser).ConfigureAwait(false);
                _logHelper.Log("7FDF23422FDS98", $"Notification successfully deleted", chatId,LogLevel.Information);
            }


        }

        private Timer _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logHelper.Log("7DS88G9G98FJ", $"Notification service STARTED", LogLevel.Information) ;
            _timer = new Timer(CheckQueues, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _logHelper.Log("7DS88G56765G98FJ", $"Notification service IS STOPPING.", LogLevel.Information);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
