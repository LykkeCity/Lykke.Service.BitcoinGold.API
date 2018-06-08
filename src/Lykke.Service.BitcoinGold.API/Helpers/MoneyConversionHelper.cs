using Lykke.Service.BitcoinGold.API.Core.Constants;
using Lykke.Service.BlockchainApi.Contract;
using NBitcoin;

namespace Lykke.Service.BitcoinGold.API.Helpers
{
    public class MoneyConversionHelper
    {
        public static string SatoshiToContract(long satoshi)
        {
            return Conversions.CoinsToContract(new Money(satoshi).ToUnit(MoneyUnit.BTC), Constants.Assets.BitcoinGold.Accuracy);
        }

        public static long SatoshiFromContract(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return 0;
            }

            var btc = Conversions.CoinsFromContract(input, Constants.Assets.BitcoinGold.Accuracy);

            return new Money(btc, MoneyUnit.BTC).Satoshi;
        }
    }
}
