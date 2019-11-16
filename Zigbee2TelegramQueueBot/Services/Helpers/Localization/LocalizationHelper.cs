using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Zigbee2TelegramQueueBot.Configuration;
using Zigbee2TelegramQueueBot.Enums;

namespace Zigbee2TelegramQueueBot.Services.Helpers.Localization
{
    public class LocalizationHelper : ILocalizationHelper
    {
        private readonly IStringLocalizer<LocalizationHelper> _localizer;
        private readonly BotConfiguration _config;
        public LocalizationHelper(IStringLocalizer<LocalizationHelper> localizer, IOptions<BotConfiguration> config)
        {
            _localizer = localizer;
            _config = config.Value;
        }
        public string GetLocalizedString(StringToLocalize stringToLocalize)
        {
            IStringLocalizer actualLocalizer; 
            switch (_config.Language)
            {
                case Language.English:
                    actualLocalizer = _localizer.WithCulture(new CultureInfo("en-US"));
                    break;
                case Language.Russian:
                    actualLocalizer = _localizer.WithCulture(new CultureInfo("ru"));
                    break;
                default:
                    actualLocalizer = _localizer.WithCulture(new CultureInfo("en-US"));
                    break;
            }

            return actualLocalizer[stringToLocalize.ToString()];

            
        }
        
    }
}
