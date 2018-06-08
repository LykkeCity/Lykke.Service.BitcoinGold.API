using System.Collections.Generic;
using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.BitcoinGold.API.Core.TransactionOutputs
{
    public interface ITransactionOutputsService
    {
        Task<IEnumerable<Coin>> GetUnspentOutputs(string address, int confirmationsCount = 0);
    }
}
