using System;
using Lykke.Service.BitcoinGold.API.Core.Operation;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.BitcoinGold.API.AzureRepositories.Operations
{
    public class OperationEventTableEntity : TableEntity, IOperationEvent
    {
        OperationEventType IOperationEvent.Type => Enum.Parse<OperationEventType>(Type);

        public string Type { get; set; }

        public DateTime DateTime { get; set; }
        public Guid OperationId { get; set; }

        public string Context { get; set; }

        public static string GeneratePartitionKey(Guid operationId)
        {
            return operationId.ToString();
        }

        public static string GenerateRowKey(OperationEventType type)
        {
            return type.ToString();
        }

        public static OperationEventTableEntity Create(IOperationEvent source)
        {
            return new OperationEventTableEntity
            {
                DateTime = source.DateTime,
                OperationId = source.OperationId,
                PartitionKey = GeneratePartitionKey(source.OperationId),
                RowKey = GenerateRowKey(source.Type),
                Context = source.Context

            };
        }
    }
}