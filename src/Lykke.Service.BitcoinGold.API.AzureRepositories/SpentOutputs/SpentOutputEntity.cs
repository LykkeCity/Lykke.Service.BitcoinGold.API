using System;
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
}