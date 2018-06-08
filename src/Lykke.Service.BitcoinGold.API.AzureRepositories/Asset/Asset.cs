using Lykke.Service.BitcoinGold.API.Core.Asset;

namespace Lykke.Service.BitcoinGold.API.AzureRepositories.Asset
{
    public class Asset : IAsset
    {
        public string AssetId { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public int Accuracy { get; set; }
    }
}