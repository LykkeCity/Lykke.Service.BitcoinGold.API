using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.Service.BitcoinGold.API.Core.Domain.Health.Exceptions;
using Lykke.Service.BitcoinGold.API.Core.Wallet;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.BitcoinGold.API.AzureRepositories.Wallet
{
    public class ObservableWalletEntity : TableEntity, IObservableWallet
    {
        public string Address { get; set; }

        public static string GeneratePartitionKey(string address)
        {
            return address.CalculateHexHash32(3);
        }

        public static string GenerateRowKey(string address)
        {
            return address;
        }

        public static ObservableWalletEntity Create(IObservableWallet source)
        {
            return new ObservableWalletEntity
            {
                Address = source.Address,
                PartitionKey = GeneratePartitionKey(source.Address),
                RowKey = GenerateRowKey(source.Address)
            };
        }
    }
    public class ObservableWalletRepository : IObservableWalletRepository
    {
        private readonly INoSQLTableStorage<ObservableWalletEntity> _storage;
        private const int EntityExistsHttpStatusCode = 409;
        private const int EntityNotExistsHttpStatusCode = 404;

        public ObservableWalletRepository(INoSQLTableStorage<ObservableWalletEntity> storage)
        {
            _storage = storage;
        }

        public async Task Insert(IObservableWallet wallet)
        {
            if (!await _storage.TryInsertAsync(ObservableWalletEntity.Create(wallet)))
                throw new BusinessException($"Wallet {wallet.Address} already exist", ErrorCode.EntityAlreadyExist);
        }

        public async Task<(IEnumerable<IObservableWallet>, string ContinuationToken)> GetAll(int take, string continuationToken)
        {
            return await _storage.GetDataWithContinuationTokenAsync(take, continuationToken);
        }

        public async Task Delete(string address)
        {
            if (!await _storage.DeleteIfExistAsync(ObservableWalletEntity.GeneratePartitionKey(address), ObservableWalletEntity.GenerateRowKey(address)))
                throw new BusinessException($"Wallet {address} not exist", ErrorCode.EntityNotExist);
        }

        public async Task<IObservableWallet> Get(string address)
        {
            return await _storage.GetDataAsync(ObservableWalletEntity.GeneratePartitionKey(address), ObservableWalletEntity.GenerateRowKey(address));
        }
    }
}
