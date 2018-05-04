using System;
using System.Threading.Tasks;
using Lykke.Service.BitcoinGold.API.Core.Transactions;
using NBitcoin;

namespace Lykke.Service.BitcoinGold.API.Core.Operation
{
    public interface IOperationService
    {
        Task<BuildedTransactionInfo> GetOrBuildTransferTransaction(Guid operationId,
            BitcoinAddress fromAddress,
            PubKey fromAddressPubkey,
            BitcoinAddress toAddress,
            string assetId,
            Money amountToSend,
            bool includeFee);
    }
}
