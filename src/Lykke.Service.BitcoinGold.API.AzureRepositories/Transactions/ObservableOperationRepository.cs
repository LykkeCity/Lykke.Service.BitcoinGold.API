using System;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.BitcoinGold.API.Core.ObservableOperation;

namespace Lykke.Service.BitcoinGold.API.AzureRepositories.Transactions
{
    public class ObservableOperationRepository : IObservableOperationRepository
    {
        private readonly INoSQLTableStorage<ObservableOperationEntity> _storage;

        public ObservableOperationRepository(INoSQLTableStorage<ObservableOperationEntity> storage)
        {
            _storage = storage;
        }


        public async Task InsertOrReplace(IObservableOperation tx)
        {
            await _storage.InsertOrReplaceAsync(ObservableOperationEntity.ByOperationId.Create(tx));
        }

        public async Task DeleteIfExist(params Guid[] operationIds)
        {
            foreach (var operationId in operationIds)
            {
                await _storage.DeleteIfExistAsync(ObservableOperationEntity.ByOperationId.GeneratePartitionKey(operationId),
                    ObservableOperationEntity.ByOperationId.GenerateRowKey(operationId));
            }
        }

        public async Task<IObservableOperation> GetById(Guid opId)
        {
            return await _storage.GetDataAsync(ObservableOperationEntity.ByOperationId.GeneratePartitionKey(opId),
                UnconfirmedTransactionEntity.GenerateRowKey(opId));
        }
    }
}
