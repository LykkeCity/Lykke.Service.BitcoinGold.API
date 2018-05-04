using System.Threading.Tasks;

namespace Lykke.Service.BitcoinGold.API.Core.Fee
{

    public interface IFeeRateFacade
    {
        Task<int> GetFeePerByte();
    }
}
