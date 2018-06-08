using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.BitcoinGold.API.Core.Services;
using Lykke.Service.BitcoinGold.API.Core.Settings.ServiceSettings;
using Lykke.Service.BitcoinGold.API.Services.Health;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.BitcoinGold.API.Modules
{
    public class BitcoinGoldApiModule : Module
    {
        private readonly ILog _log;
        

        public BitcoinGoldApiModule(ILog log)
        {
            _log = log;        
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();
        }
    }
}
