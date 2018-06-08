using System.Threading.Tasks;
using Lykke.Service.BitcoinGold.API.Core.Fee;

namespace Lykke.Service.BitcoinGold.API.Services.Fee
{
    internal class FeeRateFacade:IFeeRateFacade
    {
        private readonly int _feePerByte;

        public FeeRateFacade(int feePerByte)
        {
            _feePerByte = feePerByte;
        }

        public Task<int> GetFeePerByte()
        {
            return Task.FromResult(_feePerByte);
        }
    }
}
