using Lykke.Service.BitcoinGold.API.Core.Settings.ServiceSettings;
using Lykke.Service.BitcoinGold.API.Core.Settings.SlackNotifications;

namespace Lykke.Service.BitcoinGold.API.Core.Settings
{
    public class AppSettings
    {
        public BitcoinGoldApiSettings BitcoinGoldApi { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
