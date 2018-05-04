using System.Collections.Generic;
using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.BitcoinGold.API.Core.Transactions
{
    public interface IBuildedTransaction
    {
        Transaction TransactionData { get; }
        Money Fee { get; }
        Money Amount { get; }

        IEnumerable<Coin> UsedCoins { get; }
    }
    public interface ITransactionBuilderService
    {
        Task<IBuildedTransaction> GetTransferTransaction(BitcoinAddress source, PubKey fromAddressPubkey, BitcoinAddress destination, Money amount,
            bool includeFee);
    }

    
}
