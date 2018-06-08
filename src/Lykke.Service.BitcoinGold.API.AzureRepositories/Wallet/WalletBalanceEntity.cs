using System;
using Common;
using Lykke.Service.BitcoinGold.API.Core.Wallet;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.BitcoinGold.API.AzureRepositories.Wallet
{
    public class WalletBalanceEntity : TableEntity, IWalletBalance
    {
        public string Address { get; set; }
        public long BalanceSatoshi { get; set; }
        public DateTime Updated { get; set; }
        public int UpdatedAtBlockHeight { get; set; }

        public static string GeneratePartitionKey(string address)
        {
            return address.CalculateHexHash32(3);
        }

        public static string GenerateRowKey(string address)
        {
            return address;
        }

        public static WalletBalanceEntity Create(IWalletBalance source)
        {
            return new WalletBalanceEntity
            {
                Address = source.Address,
                BalanceSatoshi = source.BalanceSatoshi,
                RowKey = GenerateRowKey(source.Address),
                PartitionKey = GeneratePartitionKey(source.Address),
                Updated = source.Updated,
                UpdatedAtBlockHeight = source.UpdatedAtBlockHeight
            };
        }
    }
}