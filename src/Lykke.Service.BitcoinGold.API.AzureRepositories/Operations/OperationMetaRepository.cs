using System;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.Service.BitcoinGold.API.Core.Operation;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.BitcoinGold.API.AzureRepositories.Operations
{
    public class OperationMetaEntity : TableEntity, IOperationMeta
    {
        public Guid OperationId { get; set; }
        public string Hash { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string AssetId { get; set; }
        public long AmountSatoshi { get; set; }
        public long FeeSatoshi { get; set; }
        public bool IncludeFee { get; set; }
        public DateTime Inserted { get; set; }

        public static OperationMetaEntity Map(string partitionKey, string rowKey, IOperationMeta source)
        {
            return new OperationMetaEntity
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                Hash = source.Hash,
                ToAddress = source.ToAddress,
                FromAddress = source.FromAddress,
                AssetId = source.AssetId,
                OperationId = source.OperationId,
                IncludeFee = source.IncludeFee,
                AmountSatoshi = source.AmountSatoshi,
                Inserted = source.Inserted,
                FeeSatoshi = source.FeeSatoshi
            };
        }

        public static class ByOperationId
        {
            public static string GeneratePartitionKey(Guid operationId)
            {
                return operationId.ToString().CalculateHexHash32(3);
            }

            public static string GenerateRowKey(Guid operationId)
            {
                return operationId.ToString();
            }

            public static OperationMetaEntity Create(IOperationMeta source)
            {
                return Map(GeneratePartitionKey(source.OperationId), GenerateRowKey(source.OperationId), source);
            }
        }
    }

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
