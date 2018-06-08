using System;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.BitcoinGold.API.Core.Operation;

namespace Lykke.Service.BitcoinGold.API.AzureRepositories.Operations
{
    public class OperationMetaRepository:IOperationMetaRepository
    {
        private readonly INoSQLTableStorage<OperationMetaEntity> _storage;

        public OperationMetaRepository(INoSQLTableStorage<OperationMetaEntity> storage)
        {
            _storage = storage;
        }

        public Task<bool> TryInsert(IOperationMeta meta)
        {
            return _storage.TryInsertAsync(OperationMetaEntity.ByOperationId.Create(meta));
        }

        public async Task<IOperationMeta> Get(Guid id)
        {
            return await _storage.GetDataAsync(OperationMetaEntity.ByOperationId.GeneratePartitionKey(id),
                OperationMetaEntity.ByOperationId.GenerateRowKey(id));
        }

        public async Task<bool> Exist(Guid id)
        {
            return await Get(id) != null;
        }
    }
}
