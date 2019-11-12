using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Zigbee2TelegramQueueBot.Services.Helpers.Log;
using Zigbee2TelegramQueueBot.Services.Helpers.UpdateServiceHelper;
using Zigbee2TelegramQueueBot.Services.Menu;
using Zigbee2TelegramQueueBot.Services.Room;
using Zigbee2TelegramQueueBot.Services.TelegramUpdateHandler;
using Zigbee2TelegramQueueBot.Services.Users;
using Microsoft.Extensions.Logging;
using Zigbee2TelegramQueueBot.Enums;

namespace Zigbee2TelegramQueueBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        private readonly IUpdateService _updateService;
        //private readonly ILogger<UpdateController> _logger;
        private readonly ILogHelper _logHelper;
        private readonly IRoomService _room;
        private readonly IUsersService _users;
        private readonly IMenuLoader _menuLoader;
        private readonly IUpdateHelper _updateHelper;

        public UpdateController(IUpdateService updateService,
                                ILogHelper logHelper,
                                IRoomService room,
                                IUsersService users,
                                IMenuLoader menuLoader,
                                IUpdateHelper updateHelper)
        {
            _updateService = updateService;
            _logHelper = logHelper;
            _room = room;
            _users = users;
            _menuLoader = menuLoader;
            _updateHelper = updateHelper;
        }

        // POST api/update
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            long chatId = _updateHelper.GetChatId(update);

            
            try
            {
                await _updateService.UpdateHandle(update);
            }
            catch (Exception ex)
            {
                _logHelper.Log("LKJ4L3K54343V3V43", ex.Message, LogLevel.Error);
                
                if (update.Message.Text != null)
                {
                    _logHelper.Log("DFD3S5F6DJLK6L458F", $"Crush report: message: {update.Message.Text}; userState: {_users.UserInfo[chatId].State}", chatId, LogLevel.Critical);
                    //_logger.LogInformation($"CODE LABEL: DFD3S5F6DS8F; Crush report: chatId: {chatId}; message: {update.Message.Text}; userState: {_users.UserInfo[chatId].State}", chatId);
                }
                else
                {
                    _logHelper.Log("KJKL4645LK645V6F45DFD", $"Crush report: userState: {_users.UserInfo[chatId].State}",chatId,LogLevel.Critical);
                    //_logger.LogInformation($"CODE LABEL: DFD3S5F6DS8F; Crush report: chatId: {chatId}; userState: {_users.UserInfo[chatId].State}", chatId);
                }

                _room.QueueFlush();
                
                var users = _users.UserInfo.Keys;
                foreach (var user in users)
                {
                    if (_menuLoader.MenuLoaderType == "SimpleButtonMenuLoader")
                    {

                        await _menuLoader.LoadStateMenu(chatId, UserState.InSimpleMainMenu);
                    }
                    else
                    {
                        await _menuLoader.LoadStateMenu(chatId, UserState.InMainMenu);
                    }
                }
                return Ok();

            }
            return Ok();
        }

    }
}
