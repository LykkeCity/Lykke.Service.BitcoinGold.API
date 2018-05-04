using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.BitcoinGold.API.Core.Services;
using Lykke.Service.BitcoinGold.API.Core.Settings.ServiceSettings;
using Lykke.Service.BitcoinGold.API.Services.Health;
using Lykke.Service.BitcoinGold.API.Services.LifeiteManagers;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.BitcoinGold.API.Modules
{
    public class BitcoinGoldApiModule : Module
    {
        private readonly IReloadingManager<BitcoinGoldApiSettings> _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public BitcoinGoldApiModule(IReloadingManager<BitcoinGoldApiSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            builder.Populate(_services);
        }
    }
}
