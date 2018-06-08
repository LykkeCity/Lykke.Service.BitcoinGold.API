using Common;
using Lykke.Service.BitcoinGold.API.Core.Wallet;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.BitcoinGold.API.AzureRepositories.Wallet
{
    public class ObservableWalletEntity : TableEntity, IObservableWallet
    {
        public string Address { get; set; }

        public static string GeneratePartitionKey(string address)
        {
            return address.CalculateHexHash32(3);
        }

        public static string GenerateRowKey(string address)
        {
            return address;
        }

        public static ObservableWalletEntity Create(IObservableWallet source)
        {
            return new ObservableWalletEntity
            {
                Address = source.Address,
                PartitionKey = GeneratePartitionKey(source.Address),
                RowKey = GenerateRowKey(source.Address)
            };
        }
    }
}