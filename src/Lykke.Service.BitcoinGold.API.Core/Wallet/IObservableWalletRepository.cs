using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.BitcoinGold.API.Core.Operation;

namespace Lykke.Service.BitcoinGold.API.Core.Wallet
{

    public interface IObservableWallet
    {
        string Address { get; }
    }

    public class ObservableWallet : IObservableWallet
    {
        public string Address { get; set; }

        public static ObservableWallet Create(string address)
        {
            return new ObservableWallet
            {
                Address = address
            };
        }
    }
    public interface IObservableWalletRepository
    {
        Task Insert(IObservableWallet wallet);
        Task<(IEnumerable<IObservableWallet>, string ContinuationToken)> GetAll(int take, string continuationToken);
        Task Delete(string address);
        Task<IObservableWallet> Get(string address);
    }
}
