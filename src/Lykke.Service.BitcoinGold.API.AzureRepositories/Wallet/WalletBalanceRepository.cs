using System;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.Service.BitcoinGold.API.Core.Pagination;
using Lykke.Service.BitcoinGold.API.Core.Wallet;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.BitcoinGold.API.AzureRepositories.Wallet
{
    public class WalletBalanceEntity : TableEntity, IWalletBalance
    {
        public string Address { get; set; }
        public long BalanceSatoshi { get; set; }
        public DateTime Updated { get; set; }
        public int UpdatedAtBlockHeight { get; set; }

        public static string GeneratePartitionKey(string address)
        {
            return address.CalculateHexHash32(3);
        }

        public static string GenerateRowKey(string address)
        {
            return address;
        }

        public static WalletBalanceEntity Create(IWalletBalance source)
        {
            return new WalletBalanceEntity
            {
                Address = source.Address,
                BalanceSatoshi = source.BalanceSatoshi,
                RowKey = GenerateRowKey(source.Address),
                PartitionKey = GeneratePartitionKey(source.Address),
                Updated = source.Updated,
                UpdatedAtBlockHeight = source.UpdatedAtBlockHeight
            };
        }
    }

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
