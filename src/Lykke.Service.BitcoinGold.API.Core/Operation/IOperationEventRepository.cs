using System;
using System.Threading.Tasks;
using Common;

namespace Lykke.Service.BitcoinGold.API.Core.Operation
{

    public enum OperationEventType
    {
        Broadcasted,
        DetectedOnBlockChain
    }

    public interface IOperationEvent
    {
        OperationEventType Type { get; }

        DateTime DateTime { get; }

        Guid OperationId { get; }

        string Context { get;  }
    }

    public class OperationEvent : IOperationEvent
    {
        public OperationEventType Type { get; set; }
        public DateTime DateTime { get; set; }
        public Guid OperationId { get; set; }
        public string Context { get; set; }

        public static OperationEvent Create(Guid operationId, OperationEventType type, object context = null, DateTime? dateTime = null)
        {
            return new OperationEvent
            {
                DateTime = dateTime ?? DateTime.UtcNow,
                OperationId = operationId,
                Type = type,
                Context = context?.ToJson()
                
            };
        }
    }

    public interface IOperationEventRepository
    {
        Task InsertIfNotExist(IOperationEvent operationEvent);
        Task<bool> Exist(Guid operationId, OperationEventType type);
    }
}
