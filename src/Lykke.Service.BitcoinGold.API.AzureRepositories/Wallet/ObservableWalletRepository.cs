using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.BitcoinGold.API.Core.Domain.Health.Exceptions;
using Lykke.Service.BitcoinGold.API.Core.Wallet;
using Microsoft.WindowsAzure.Storage;

namespace Lykke.Service.BitcoinGold.API.AzureRepositories.Wallet
{
    public class ObservableWalletRepository : IObservableWalletRepository
    {
        private readonly INoSQLTableStorage<ObservableWalletEntity> _storage;        

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
