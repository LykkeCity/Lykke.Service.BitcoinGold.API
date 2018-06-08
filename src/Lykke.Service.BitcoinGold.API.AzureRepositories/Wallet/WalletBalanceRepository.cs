using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.BitcoinGold.API.Core.Pagination;
using Lykke.Service.BitcoinGold.API.Core.Wallet;

namespace Lykke.Service.BitcoinGold.API.AzureRepositories.Wallet
{
    public class WalletBalanceRepository : IWalletBalanceRepository
    {
        private readonly INoSQLTableStorage<WalletBalanceEntity> _storage;

        public WalletBalanceRepository(INoSQLTableStorage<WalletBalanceEntity> storage)
        {
            _storage = storage;
        }

        public Task InsertOrReplace(IWalletBalance balance)
        {
            return _storage.InsertOrReplaceAsync(WalletBalanceEntity.Create(balance));
        }

        public Task DeleteIfExist(string address)
        {
            return _storage.DeleteIfExistAsync(WalletBalanceEntity.GeneratePartitionKey(address),
                WalletBalanceEntity.GenerateRowKey(address));
        }

        public async Task<IPaginationResult<IWalletBalance>> GetBalances(int take, string continuation)
        {
            var result = await _storage.GetDataWithContinuationTokenAsync(take, continuation);
            return PaginationResult<IWalletBalance>.Create(result.Entities, result.ContinuationToken);
        }
    }
}
