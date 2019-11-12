using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zigbee2TelegramQueueBot.Services.Room;

namespace Zigbee2TelegramQueueBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _room;
        public RoomController(IRoomService room)
        {
            _room = room;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            
            return new string[] { $"Status: {_room.IsBusy.ToString()}" };
        }

        [HttpGet("occupy")]
        public ActionResult<string> Occupy()
        {
            
            _room.IsBusy = true;
            return $"Status: Occupied";
        }

        [HttpGet("free")]
        public ActionResult<string> Free()
        {
            _room.IsBusy = false;
            
            return $"Status: Free";
        }


    }
}
