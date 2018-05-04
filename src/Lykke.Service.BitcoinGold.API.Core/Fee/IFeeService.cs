using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.BitcoinGold.API.Core.Fee
{
    public interface IFeeService
    {
        Task<Money> CalcFeeForTransaction(Transaction tx);
        Task<Money> CalcFeeForTransaction(TransactionBuilder builder);
    }
}
