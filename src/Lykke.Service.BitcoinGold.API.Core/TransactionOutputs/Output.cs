using NBitcoin;

namespace Lykke.Service.BitcoinGold.API.Core.TransactionOutputs
{

    public class Output : IOutput
    {
        public Output(OutPoint outpoint)
        {
            TransactionHash = outpoint.Hash.ToString();
            N = (int)outpoint.N;
        }

        public string TransactionHash { get; }
        public int N { get; }
    }
}
