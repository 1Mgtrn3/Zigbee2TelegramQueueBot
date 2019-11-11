using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Zigbee2TelegramQueueBot.Services.Helpers
{
    public class LogHelper : ILogHelper
    {
        private readonly ILogger<LogHelper> _logger;
        private readonly IRoomQueue _roomQueue;
        private readonly IUsersService _users;
        public LogHelper(ILogger<LogHelper> logger, IRoomQueue roomQueue, IUsersService users)
        {
            _logger = logger;
            _roomQueue = roomQueue;
            _users = users;
        }

        public void Log(string codeLabel, string information, LogLevel logLevel, bool includeQueue = false, [CallerMemberName] string caller = null)
        {


            StringBuilder logRecordBuilder = new StringBuilder();

            logRecordBuilder.AppendLine($"DateTime: {DateTime.Now.ToString()}");
            logRecordBuilder.AppendLine($"Method: {caller}");
            logRecordBuilder.AppendLine($"CODE LABEL: {codeLabel}");
            logRecordBuilder.AppendLine($"Information: {information}");
            if (includeQueue)
            {
                logRecordBuilder.AppendLine($"Queue status:\r\n{GetQueueStatusTrace()}");
            }

            
            switch (logLevel)
            {
                case LogLevel.Trace:
                    _logger.LogTrace(logRecordBuilder.ToString());
                    break;
                case LogLevel.Debug:
                    _logger.LogDebug(logRecordBuilder.ToString());
                    break;
                case LogLevel.Information:
                    _logger.LogInformation(logRecordBuilder.ToString());
                    break;
                case LogLevel.Warning:
                    _logger.LogWarning(logRecordBuilder.ToString());
                    break;
                case LogLevel.Error:
                    _logger.LogError(logRecordBuilder.ToString());
                    break;
                case LogLevel.Critical:
                    _logger.LogCritical(logRecordBuilder.ToString());
                    break;
                case LogLevel.None:
                    _logger.LogTrace(logRecordBuilder.ToString());
                    break;
                default:
                    _logger.LogWarning(logRecordBuilder.ToString());
                    break;
            }

        }

        public void Log(string codeLabel, string information, long chatId, LogLevel logLevel, bool includeQueue = false, [CallerMemberName] string caller = null)
        {
            string username = "";
            if (chatId != 0)
            {
                username = _users.UserInfo[chatId].UserName;
            }
            else
            {
                username = "user0";
            }



            StringBuilder logRecordBuilder = new StringBuilder();

            logRecordBuilder.AppendLine($"DateTime: {DateTime.Now.ToString()}");
            logRecordBuilder.AppendLine($"Method: {caller}");
            logRecordBuilder.AppendLine($"CODE LABEL: {codeLabel}");
            logRecordBuilder.AppendLine($"Username: {username}");
            logRecordBuilder.AppendLine($"Information: {information}");
            if (includeQueue)
            {
                logRecordBuilder.AppendLine($"Queue status:\r\n{GetQueueStatusTrace()}");
            }

            switch (logLevel)
            {
                case LogLevel.Trace:
                    _logger.LogTrace(logRecordBuilder.ToString());
                    break;
                case LogLevel.Debug:
                    _logger.LogDebug(logRecordBuilder.ToString());
                    break;
                case LogLevel.Information:
                    _logger.LogInformation(logRecordBuilder.ToString());
                    break;
                case LogLevel.Warning:
                    _logger.LogWarning(logRecordBuilder.ToString());
                    break;
                case LogLevel.Error:
                    _logger.LogError(logRecordBuilder.ToString());
                    break;
                case LogLevel.Critical:
                    _logger.LogCritical(logRecordBuilder.ToString());
                    break;
                case LogLevel.None:
                    _logger.LogTrace(logRecordBuilder.ToString());
                    break;
                default:
                    _logger.LogWarning(logRecordBuilder.ToString());
                    break;
            }
        }

        public string GetQueueStatusTrace()
        {



            StringBuilder traceBuilder = new StringBuilder();
            traceBuilder.AppendLine("==================");
            traceBuilder.AppendLine("Queue status trace");
            traceBuilder.AppendLine($"Queue size: {_roomQueue.QueueList.Count.ToString()}");
            traceBuilder.AppendLine("#Queue Slots:");
            for (int i = 0; i < _roomQueue.QueueList.Count; i++)
            {
                traceBuilder.AppendLine($"##Number: {i}; ChatID: {_roomQueue.QueueList[i].ChatId}; Time: {_roomQueue.QueueList[i].TimeMinutes}; FailedAttempts: {_roomQueue.QueueList[i].FailedAttempts}");
            }
            traceBuilder.AppendLine("==================");


            return traceBuilder.ToString();
        }


    }
}
