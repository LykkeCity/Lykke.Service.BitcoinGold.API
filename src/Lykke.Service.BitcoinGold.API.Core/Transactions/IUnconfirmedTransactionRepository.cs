using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.BitcoinGold.API.Core.Transactions
{
    public interface IUnconfirmedTransaction
    {
        string TxHash { get; }
        Guid OperationId { get; }
    }

    public class UnconfirmedTransaction : IUnconfirmedTransaction
    {
        public string TxHash { get; set; }
        public Guid OperationId { get; set; }

        public static UnconfirmedTransaction Create(Guid opId, string txHash)
        {
            return new UnconfirmedTransaction
            {
                OperationId = opId,
                TxHash = txHash
            };
        }
    }

    public interface IUnconfirmedTransactionRepository
    {
        Task<IEnumerable<IUnconfirmedTransaction>> GetAll();
        Task InsertOrReplace(IUnconfirmedTransaction tx);
        Task DeleteIfExist(params Guid[] operationIds);
    }
}
