using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zigbee2TelegramQueueBot.Services.LockTracker;

namespace Zigbee2TelegramQueueBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LockTrackerSimulationController : ControllerBase
    {
        private readonly ILockTrackerService _lockTrackerService;
        public LockTrackerSimulationController(ILockTrackerService lockTrackerService)
        {
            _lockTrackerService = lockTrackerService;
        }


        // POST api/update
        [HttpPost]
        public void Post([FromBody]LockTrackerMessage trackerMessage)
        {
            //sample message: {"contact":true, "linkquality": 200, "battery":100, voltage:3025}
            _lockTrackerService.IsRoomFree = !trackerMessage.contact;

        }
    }
}
