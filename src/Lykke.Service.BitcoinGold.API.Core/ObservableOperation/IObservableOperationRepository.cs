using System;
using System.Threading.Tasks;
using Lykke.Service.BitcoinGold.API.Core.Operation;

namespace Lykke.Service.BitcoinGold.API.Core.ObservableOperation
{
    public enum BroadcastStatus
    {
        InProgress,
        Completed,
        Failed
    }

    public interface IObservableOperation
    {
        BroadcastStatus Status { get; }
        Guid OperationId { get; }
        string FromAddress { get; }
        string ToAddress { get; }
        string AssetId { get; }
        long AmountSatoshi { get; }
        long FeeSatoshi { get; }
        DateTime Updated { get; }
        string TxHash { get; }
        int UpdatedAtBlockHeight { get; }

    }

    public class ObervableOperation : IObservableOperation
    {
        public Guid OperationId { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string AssetId { get; set; }
        public long AmountSatoshi { get; set; }
        public long FeeSatoshi { get; set; }
        public bool IncludeFee { get; set; }
        public DateTime Updated { get; set; }
        public BroadcastStatus Status { get; set; }
        public string TxHash { get; set; }
        public int UpdatedAtBlockHeight { get; set; }

        public static ObervableOperation Create(IOperationMeta operation, BroadcastStatus status, string txHash, int updatedAtBlockHeight, DateTime? updated = null)
        {
            return new ObervableOperation
            {
                OperationId = operation.OperationId,
                AmountSatoshi = operation.AmountSatoshi,
                AssetId = operation.AssetId,
                FromAddress = operation.FromAddress,
                IncludeFee = operation.IncludeFee,
                ToAddress = operation.ToAddress,
                Status = status,
                TxHash = txHash,
                Updated = updated ?? DateTime.UtcNow,
                FeeSatoshi = operation.FeeSatoshi,
                UpdatedAtBlockHeight = updatedAtBlockHeight
            };
        }
    }

    public interface IObservableOperationRepository
    {
        Task InsertOrReplace(IObservableOperation tx);
        Task DeleteIfExist(params Guid[] operationIds);
        Task<IObservableOperation> GetById(Guid opId);
    }
}
