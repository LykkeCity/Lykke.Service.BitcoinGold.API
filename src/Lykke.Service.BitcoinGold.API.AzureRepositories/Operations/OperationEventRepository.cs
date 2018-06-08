using System;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.BitcoinGold.API.Core.Operation;

namespace Lykke.Service.BitcoinGold.API.AzureRepositories.Operations
{
    public class OperationEventRepository : IOperationEventRepository
    {
        private readonly INoSQLTableStorage<OperationEventTableEntity> _storage;

        public OperationEventRepository(INoSQLTableStorage<OperationEventTableEntity> storage)
        {
            _storage = storage;
        }

        public async Task InsertIfNotExist(IOperationEvent operationEvent)
        {
            await _storage.CreateIfNotExistsAsync(OperationEventTableEntity.Create(operationEvent));
        }

        public async Task<bool> Exist(Guid operationId, OperationEventType type)
        {
            return await _storage.GetDataAsync(OperationEventTableEntity.GeneratePartitionKey(operationId),
                       OperationEventTableEntity.GenerateRowKey(type)) != null;
        }
    }
}
