using System;
using System.Threading.Tasks;

namespace Lykke.Service.BitcoinGold.API.Core.Transactions
{
    public enum TransactionBlobType
    {
        Initial = 0,
        BeforeBroadcast = 1
    }

    public interface ITransactionBlobStorage
    {
        Task<string> GetTransaction(Guid operationId, string hash, TransactionBlobType type);

        Task AddOrReplaceTransaction(Guid operationId, string hash, TransactionBlobType type, string transactionHex);
    }
}
