using System.Collections.Generic;
using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.BitcoinGold.API.Core.BlockChainReaders
{
    public interface IBlockChainProvider
    {        
        Task BroadCastTransaction(Transaction tx);
        Task<int> GetTxConfirmationCount(string txHash);
        Task<IList<Coin>> GetUnspentOutputs(string address, int minConfirmationCount);        
        Task<long> GetBalanceSatoshiFromUnspentOutputs(string address, int minConfirmationCount);
        Task<int> GetLastBlockHeight();
        Task<IEnumerable<string>> GetTransactionsForAddress(string address);
        Task<BtgTransaction> GetTransaction(string txHash);
    }
}
