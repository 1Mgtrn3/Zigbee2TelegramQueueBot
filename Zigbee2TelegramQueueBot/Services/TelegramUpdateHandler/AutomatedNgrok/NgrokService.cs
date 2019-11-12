using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zigbee2TelegramQueueBot.Configuration;
using Zigbee2TelegramQueueBot.Services.Helpers.Log;
using Microsoft.Extensions.Logging;
using HtmlAgilityPack;
using System.Threading;
using System.Diagnostics;

namespace Zigbee2TelegramQueueBot.Services.TelegramUpdateHandler.AutomatedNgrok
{
    public class NgrokService : INgrokService
    {
        private readonly BotConfiguration _config;
        private readonly ILogHelper _logHelper;
        private string NGrockUrl { get; set; } = "";

        public NgrokService(IOptions<BotConfiguration> config, ILogHelper logHelper)
        {
            _config = config.Value;
            _logHelper = logHelper;
            _logHelper.Log("K4358FD8JKK4333", "Starting NGROK",LogLevel.Information);

            if (_config.ExternalURL == "")
            {
                var isSuccessfullyLoaded = StartNgrok().Result;
                if (!isSuccessfullyLoaded)
                {
                    throw new Exception("Ngrok tunneling failed to start. Try to update it or check if you exceeded the number or simultanious connections");
                }
            }



        }
        public async Task<string> GetNgrokUrlAsync()
        {
            _logHelper.Log("C6MHJ645G43HJ5", "Trying to get the url of ngrok",LogLevel.Information);

            //Thread.Sleep(5000);
            var url = "http://127.0.0.1:4040/api/tunnels";
            var web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();


            bool isSuccessfullyLoaded = false;
            int attempts = 0;
            int attemptsLimit = _config.AttempstLimit;

            int httpsHead = 0;
            while (!isSuccessfullyLoaded && attempts < attemptsLimit)
            {
                isSuccessfullyLoaded = true;
                _logHelper.Log("LK5436765D5G645", $"About to check ngrok url presence", LogLevel.Information);

                doc = await web.LoadFromWebAsync(url);
                _logHelper.Log("5KJ43KJB65765V7", $"Doc loaded:\r\n{doc.Text}", LogLevel.Information);
                httpsHead = doc.Text.IndexOf("https://");
                if (httpsHead == -1)
                {
                    isSuccessfullyLoaded = false;
                    Thread.Sleep(1000);
                }



                ++attempts;

            }


            var NGrockUrl = "";
            if (isSuccessfullyLoaded)
            {
                NGrockUrl = doc.Text.Substring(httpsHead, 25).Trim();
            }


            return NGrockUrl;
        }

        public async Task<bool> StartNgrok()
        {

            string ngrokPath = _config.NgrokPath;
            string ngrokArguments = _config.NgrokArguments;
            _logHelper.Log("K43233234KK4333",
                $"Ngrok path: {ngrokPath} Ngrok arguments: {ngrokArguments}",LogLevel.Information);

            var pInfo = new ProcessStartInfo()
            {
                FileName = ngrokPath,
                Arguments = ngrokArguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };
            var process = new Process() { StartInfo = pInfo };

            try
            {

                process.Start();
            }
            catch (Exception ex)
            {
                _logHelper.Log("LKJ45LK65BB765",
                $"Exception occured during Ngrok start.\r\n{ex.Message}", LogLevel.Error);

                return false;
            }
            //process.BeginOutputReadLine();

            _logHelper.Log("HJLK6543NMGFFD", "Seems like Ngrok is started", LogLevel.Information);
            var url = "http://127.0.0.1:4040/status";
            var web = new HtmlWeb();

            bool isSuccessfullyLoaded = false;
            int attempts = 0;
            int attemptsLimit = _config.AttempstLimit;

            while (!isSuccessfullyLoaded && attempts < attemptsLimit)
            {
                isSuccessfullyLoaded = true;
                _logHelper.Log("LK6LK4CDD5G645", $"About to check status", LogLevel.Information);
                try
                {
                    var doc = await web.LoadFromWebAsync(url);//.Load(url);
                }
                catch (Exception)
                {

                    isSuccessfullyLoaded = false;
                }
                ++attempts;
            }
            _logHelper.Log("LK6L5555D5G645", $"Status is {isSuccessfullyLoaded.ToString()}", LogLevel.Warning);


            return isSuccessfullyLoaded;


        }
    }
}
