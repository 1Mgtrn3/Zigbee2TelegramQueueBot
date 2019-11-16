using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zigbee2TelegramQueueBot.Configuration;
using Zigbee2TelegramQueueBot.Services.Bot;
using Zigbee2TelegramQueueBot.Services.Helpers.Log;
using Zigbee2TelegramQueueBot.Services.Helpers.Localization;
using Zigbee2TelegramQueueBot.Services.Helpers.UpdateServiceHelper;
using Zigbee2TelegramQueueBot.Services.LockTracker;
using Zigbee2TelegramQueueBot.Services.Menu;
using Zigbee2TelegramQueueBot.Services.Notifications;
using Zigbee2TelegramQueueBot.Services.Room;
using Zigbee2TelegramQueueBot.Services.Room.Queue;
using Zigbee2TelegramQueueBot.Services.Session;
using Zigbee2TelegramQueueBot.Services.TelegramUpdateHandler;
using Zigbee2TelegramQueueBot.Services.TelegramUpdateHandler.AutomatedNgrok;
using Zigbee2TelegramQueueBot.Services.Users;
using Zigbee2TelegramQueueBot.SimpleMode;

namespace Zigbee2TelegramQueueBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(o =>
            {
                // We will put our translations in a folder called Resources
                o.ResourcesPath = "Resources";
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton<INgrokService, NgrokService>();

            services.AddScoped<IUpdateService, UpdateService>();
            services.AddScoped<IUpdateHelper, UpdateHelper>();
            services.AddSingleton<IBotService, BotService>();

            services.AddSingleton<IUsersService, UsersService>();

            services.AddSingleton<ILockTrackerService, LockTrackerService>();
            services.AddSingleton<IRoomQueue, RoomQueue>();
            services.AddTransient<ILogHelper, LogHelper>();
            services.AddSingleton<INotificationRouter, NotificationRouter>();
            services.AddSingleton<ILocalizationHelper, LocalizationHelper>();


            services.AddHostedService<NotificationService>();


            var isBotModeSimple = Configuration.GetValue<bool>("BotConfiguration:SimpleMode", true);
            if (isBotModeSimple)
            {
                services.AddScoped<ISessionRouter, SimpleSessionRouter>();
                services.AddSingleton<IRoomService, SimpleRoom>();
                services.AddSingleton<IMenuLoader, SimpleButtonMenuLoader>();
            }
            else
            {
                services.AddScoped<ISessionRouter, SessionRouter>();
                services.AddSingleton<IRoomService, RoomService>();
                var MenuMode = Configuration.GetValue<string>("BotConfiguration:MenuMode", "TEXT");
                if (MenuMode == "BUTTONS")
                {
                    services.AddSingleton<IMenuLoader, ButtonMenuLoader>();
                }
                else
                {
                    services.AddSingleton<IMenuLoader, TextMenuLoader>();
                }
            }





            services.Configure<BotConfiguration>(Configuration.GetSection("BotConfiguration"));
            services.Configure<LockTrackerConfiguration>(Configuration.GetSection("LockTrackerConfiguration"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.ApplicationServices.GetService<IBotService>();
            app.ApplicationServices.GetService<ILockTrackerService>();
            app.ApplicationServices.GetService<IRoomQueue>();

            app.UseMvc();
        }
    }
}
