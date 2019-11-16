using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zigbee2TelegramQueueBot.Enums;

namespace Zigbee2TelegramQueueBot.Services.Helpers.Localization
{
   public interface ILocalizationHelper
    {
        string GetLocalizedString(StringToLocalize stringToLocalize);
    }
}
