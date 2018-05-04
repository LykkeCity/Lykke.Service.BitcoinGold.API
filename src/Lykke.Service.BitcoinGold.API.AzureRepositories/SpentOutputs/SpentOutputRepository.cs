using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.BitcoinGold.API.Core.TransactionOutputs;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.BitcoinGold.API.AzureRepositories.SpentOutputs
{
    public class SpentOutputEntity : TableEntity, IOutput
    {

        public string TransactionHash { get; set; }
        public int N { get; set; }
        public Guid OperationId { get; set; }

        public static SpentOutputEntity Create(string transactionHash, int n, Guid operationId)
        {
            return new SpentOutputEntity
            {
                PartitionKey = GeneratePartitionKey(transactionHash),
                RowKey = GenerateRowKey(n),
                OperationId = operationId,
                TransactionHash = transactionHash,
                N = n
            };
        }

        public static string GenerateRowKey(int n)
        {
            return n.ToString();
        }

        public static string GeneratePartitionKey(string transactionHash)
        {
            return transactionHash;
        }
    }



    public class SpentOutputRepository : ISpentOutputRepository
    {
        private readonly INoSQLTableStorage<SpentOutputEntity> _table;

        public SpentOutputRepository(INoSQLTableStorage<SpentOutputEntity> table)
        {
            _table = table;
        }

        public Task InsertSpentOutputs(Guid operationId, IEnumerable<IOutput> outputs)
        {
            var entities = outputs.Select(o => SpentOutputEntity.Create(o.TransactionHash, o.N, operationId));
            return Task.WhenAll(entities.GroupBy(o => o.PartitionKey).Select((group) => _table.InsertOrReplaceAsync(group)));
        }

        public async Task<IEnumerable<IOutput>> GetSpentOutputs(IEnumerable<IOutput> outputs)
        {
            return await _table.GetDataAsync(outputs.Select(o =>
                new Tuple<string, string>(SpentOutputEntity.GeneratePartitionKey(o.TransactionHash), SpentOutputEntity.GenerateRowKey(o.N))));
        }

        public async Task RemoveOldOutputs(DateTime bound)
        {
            string continuation = null;
            IEnumerable<SpentOutputEntity> outputs = null;
            do
            {
                (outputs, continuation) = await _table.GetDataWithContinuationTokenAsync(100, continuation);
                await Task.WhenAll(outputs.Where(o => o.Timestamp < bound).GroupBy(o => o.PartitionKey).Select(group => _table.DeleteAsync(group)));                
            } while (continuation != null);
        }
    }
}
