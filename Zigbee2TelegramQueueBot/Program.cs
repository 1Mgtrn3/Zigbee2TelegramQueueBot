using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Zigbee2TelegramQueueBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).ConfigureLogging((hostingContext, logging) => {
                logging.AddConsole();
                logging.AddDebug();
                logging.AddEventSourceLogger();
                //logging.AddFile("/TelegramAPI/APITest/update.log",  append: true);
            }

            ).UseUrls("http://localhost:8443")
                .UseStartup<Startup>();
    }
}
