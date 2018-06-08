using Autofac;
using AzureStorage.Blob;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.BitcoinGold.API.AzureRepositories.Asset;
using Lykke.Service.BitcoinGold.API.AzureRepositories.Operations;
using Lykke.Service.BitcoinGold.API.AzureRepositories.SpentOutputs;
using Lykke.Service.BitcoinGold.API.AzureRepositories.Transactions;
using Lykke.Service.BitcoinGold.API.AzureRepositories.Wallet;
using Lykke.Service.BitcoinGold.API.Core.Asset;
using Lykke.Service.BitcoinGold.API.Core.ObservableOperation;
using Lykke.Service.BitcoinGold.API.Core.Operation;
using Lykke.Service.BitcoinGold.API.Core.Settings.ServiceSettings;
using Lykke.Service.BitcoinGold.API.Core.TransactionOutputs;
using Lykke.Service.BitcoinGold.API.Core.Transactions;
using Lykke.Service.BitcoinGold.API.Core.Wallet;
using Lykke.SettingsReader;

namespace Lykke.Service.BitcoinGold.API.AzureRepositories.Binder
{
    public class RepositoryModule : Module
    {
        private readonly ILog _log;
        private readonly IReloadingManager<BitcoinGoldApiSettings> _settings;
        public RepositoryModule(IReloadingManager<BitcoinGoldApiSettings> settings, ILog log)
        {
            _log = log;
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterRepo(builder);
            RegisterBlob(builder);
        }

        private void RegisterRepo(ContainerBuilder builder)
        {
            builder.RegisterInstance(new AssetRepository())
                .As<IAssetRepository>();

            builder.RegisterInstance(new OperationMetaRepository(
                AzureTableStorage<OperationMetaEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "OperationMeta", _log)))
                .As<IOperationMetaRepository>();

            builder.RegisterInstance(new OperationEventRepository(
                    AzureTableStorage<OperationEventTableEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "OperationEvents", _log)))
                .As<IOperationEventRepository>();


            builder.RegisterInstance(new UnconfirmedTransactionRepository(
                AzureTableStorage<UnconfirmedTransactionEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "UnconfirmedTransactions", _log)))
                .As<IUnconfirmedTransactionRepository>();

            builder.RegisterInstance(new ObservableOperationRepository(
                AzureTableStorage<ObservableOperationEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "ObservableOperations", _log)))
                .As<IObservableOperationRepository>();

            builder.RegisterInstance(new ObservableWalletRepository(
                AzureTableStorage<ObservableWalletEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "ObservableWallets", _log)))
                .As<IObservableWalletRepository>();

            builder.RegisterInstance(new WalletBalanceRepository(
                    AzureTableStorage<WalletBalanceEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "WalletBalances", _log)))
                .As<IWalletBalanceRepository>();

            builder.RegisterInstance(new SpentOutputRepository(
                    AzureTableStorage<SpentOutputEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "SpentOutputs", _log)))
                .As<ISpentOutputRepository>();
        }

        private void RegisterBlob(ContainerBuilder builder)
        {
            builder.RegisterInstance(
                new TransactionBlobStorage(AzureBlobStorage.Create(_settings.Nested(p => p.Db.DataConnString))))
                .As<ITransactionBlobStorage>();
        }
    }
}
