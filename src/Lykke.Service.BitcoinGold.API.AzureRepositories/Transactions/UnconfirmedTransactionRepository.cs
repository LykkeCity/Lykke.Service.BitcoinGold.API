using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.BitcoinGold.API.Core.Transactions;

namespace Lykke.Service.BitcoinGold.API.AzureRepositories.Transactions
{
    public class UnconfirmedTransactionRepository: IUnconfirmedTransactionRepository
    {
        private readonly INoSQLTableStorage<UnconfirmedTransactionEntity> _storage;

        public UnconfirmedTransactionRepository(INoSQLTableStorage<UnconfirmedTransactionEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<IUnconfirmedTransaction>> GetAll()
        {
            return await _storage.GetDataAsync();
        }

        public Task InsertOrReplace(IUnconfirmedTransaction tx)
        {
            return _storage.InsertOrReplaceAsync(UnconfirmedTransactionEntity.Create(tx));
        }

        public async Task DeleteIfExist(Guid[] operationIds)
        {
            foreach (var operationId in operationIds)
            {
                await _storage.DeleteIfExistAsync(UnconfirmedTransactionEntity.GeneratePartitionKey(operationId),
                    UnconfirmedTransactionEntity.GenerateRowKey(operationId));
            }
        }
    }
}
