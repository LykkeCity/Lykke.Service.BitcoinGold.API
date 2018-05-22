using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace Lykke.Service.BitcoinGold.API.Core.BlockChainReaders
{
    public class BtgTransaction
    {
        public string Hash { get; set; }

        public DateTime Timestamp { get; set; }

        public IList<BtgInput> Inputs { get; set; }

        public IList<BtgOutput> Outputs { get; set; }
    }

    public class BtgInput
    {
        public string Address { get; set; }

        public Money Value { get; set; }
    }

    public class BtgOutput
    {
        public string Address { get; set; }

        public Money Value { get; set; }
    }
}
