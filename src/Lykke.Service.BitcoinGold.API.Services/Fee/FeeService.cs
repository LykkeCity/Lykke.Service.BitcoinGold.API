using System.Threading.Tasks;
using Lykke.Service.BitcoinGold.API.Core.Fee;
using NBitcoin;

namespace Lykke.Service.BitcoinGold.API.Services.Fee
{
    public class FeeService:IFeeService
    {
        private readonly IFeeRateFacade _feeRateFacade;
        private readonly long _minFeeValueSatoshi;
        private readonly long _maxFeeValueSatoshi;

        public FeeService(IFeeRateFacade feeRateFacade, long minFeeValueSatoshi, long maxFeeValueSatoshi)
        {
            _feeRateFacade = feeRateFacade;
            _minFeeValueSatoshi = minFeeValueSatoshi;
            _maxFeeValueSatoshi = maxFeeValueSatoshi;
        }

        public async Task<Money> CalcFeeForTransaction(Transaction tx)
        {
            var size = tx.ToBytes().Length;

            var feeFromFeeRate = (await GetFeeRate()).GetFee(size);

            return CheckMinMaxThreshold(feeFromFeeRate); 
        }

        public  async Task<Money> CalcFeeForTransaction(TransactionBuilder builder)
        {
            var feeRate = await GetFeeRate();

            var feeFromFeeRate = builder.EstimateFees(builder.BuildTransaction(false), feeRate);

            return CheckMinMaxThreshold(feeFromFeeRate);
        }

        public async Task<FeeRate> GetFeeRate()
        {
            var feePerByte = await _feeRateFacade.GetFeePerByte();

            return new FeeRate(new Money(feePerByte * 1024, MoneyUnit.Satoshi));
        }

        private Money CheckMinMaxThreshold(Money fromFeeRate)
        {
            var min = new Money(_minFeeValueSatoshi, MoneyUnit.Satoshi);
            var max = new Money(_maxFeeValueSatoshi, MoneyUnit.Satoshi);

            return Money.Max(Money.Min(fromFeeRate, max), min);
        }
    }
}
