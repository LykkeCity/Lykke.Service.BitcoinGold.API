using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BitcoinGold.API.Core.TransactionOutputs
{
    public interface IOutput
    {
        string TransactionHash { get; }

        int N { get; }        
    }
}
