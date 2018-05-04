using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.Service.BitcoinGold.API.Core.Transactions;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.BitcoinGold.API.AzureRepositories.Transactions
{
    public class UnconfirmedTransactionEntity : TableEntity, IUnconfirmedTransaction
    {
        public string TxHash { get; set; }
        public Guid OperationId { get; set; }

        public static string GeneratePartitionKey(Guid operationId)
        {
            return operationId.ToString().CalculateHexHash32(3);
        }

        public static string GenerateRowKey(Guid operationId)
        {
            return operationId.ToString();
        }

        public static UnconfirmedTransactionEntity Create(IUnconfirmedTransaction source)
        {
            return new UnconfirmedTransactionEntity
            {
                PartitionKey = GeneratePartitionKey(source.OperationId),
                RowKey = GenerateRowKey(source.OperationId),
                OperationId = source.OperationId,
                TxHash = source.TxHash
            };
        }
    }

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
