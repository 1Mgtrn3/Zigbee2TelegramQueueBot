using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zigbee2TelegramQueueBot.Configuration;
using Zigbee2TelegramQueueBot.Services.Helpers.Log;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using System.Net;
using MihaZupan;

namespace Zigbee2TelegramQueueBot.Services.Bot
{
    public class BotService : IBotService
    {
        private readonly BotConfiguration _config;
        private readonly ILogHelper _logHelper;
        private readonly INgrokService _ngrokService;


        public BotService(IOptions<BotConfiguration> config, ILogHelper logHelper, INgrokService ngrokService)
        {
            _logHelper = logHelper;
            _logHelper.Log("4H456K454KKFGFF", "Bot service is starting", LogLevel.Information);
            _ngrokService = ngrokService;
            _config = config.Value;

            switch (_config.ProxyMode)
            {
                case Enums.ProxyMode.NONE:
                    _logHelper.Log("K65K65765JFDFGFD55", "Starting without a proxy", LogLevel.Information);                    
                    Client = new TelegramBotClient(_config.BotToken);
                    break;
                case Enums.ProxyMode.HTTP:

                    int httpPort = (int)_config.HTTPProxy.HTTPPort;
                    _logHelper.Log("5K65FJHGJGFDF89893", $"Starting HTTP proxy. host: {_config.HTTPProxy.HTTPHost} port {httpPort.ToString()}",LogLevel.Information);
                   
                    WebProxy httpProxy = new WebProxy(_config.HTTPProxy.HTTPHost, httpPort);
                    Client = new TelegramBotClient(
                       _config.BotToken, httpProxy);

                    break;
                case Enums.ProxyMode.SOCKS5:
                    int socksPort = (int)_config.SOCKS5Proxy.Socks5Port;

                    if (string.IsNullOrEmpty(_config.SOCKS5Proxy.Socks5Username))
                    {
                        _logHelper.Log("L5KLJ345HJHDDS98XC", "Starting with an open SOCKS5 proxy",LogLevel.Information);
                        
                        Client = new TelegramBotClient(
                       _config.BotToken,
                       new HttpToSocks5Proxy(_config.SOCKS5Proxy.Socks5Host, socksPort));

                    }
                    else
                    {
                        _logHelper.Log("NM6456NMLKJ4554", "Starting with a protected SOCKS5 proxy", LogLevel.Information);
                        
                        Client = new TelegramBotClient(
                       _config.BotToken,
                       new HttpToSocks5Proxy(_config.SOCKS5Proxy.Socks5Host,
                                           socksPort,
                                           _config.SOCKS5Proxy.Socks5Username,
                                           _config.SOCKS5Proxy.Socks5Password));

                    }
                    break;
                default:
                    break;
            }
            _logHelper.Log("KBJH43543JH65765KWS", "TelegramBotClient created",LogLevel.Information);


            string externalUrl = "";
            if (_config.ExternalURL != "")
            {
                externalUrl = _config.ExternalURL;
                Client.SetWebhookAsync(externalUrl + "/api/update/").Wait();
            }
            else
            {
                externalUrl = _ngrokService.GetNgrokUrlAsync().Result;

                Client.SetWebhookAsync(externalUrl + "/api/update/").Wait();

            }

            



            var info = Client.GetWebhookInfoAsync().Result;
            if (string.IsNullOrEmpty(info.LastErrorMessage))
            {
                _logHelper.Log("KJ5LK456L4B4654F45", $"Webhook set. Webhook url: {externalUrl}/api/update/",LogLevel.Information);                
            }
            else
            {
                _logHelper.Log("MNVNV435CN435N43KJ", $"Webhook not set. Webhook url: {externalUrl}/api/update/ \r\nError: {info.LastErrorMessage}", LogLevel.Error);
                
            }
        }

        public TelegramBotClient Client { get; }
    }
}
