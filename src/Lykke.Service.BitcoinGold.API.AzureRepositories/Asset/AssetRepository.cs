using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BitcoinGold.API.Core.Asset;
using Lykke.Service.BitcoinGold.API.Core.Constants;
using Lykke.Service.BitcoinGold.API.Core.Pagination;

namespace Lykke.Service.BitcoinGold.API.AzureRepositories.Asset
{
    public class AssetRepository:IAssetRepository
    {
        private readonly IEnumerable<IAsset> _mockList = new List<IAsset>
        {

            new Asset
            {
                Address = Constants.Assets.BitcoinGold.Address,
                AssetId = Constants.Assets.BitcoinGold.AssetId,
                Accuracy = Constants.Assets.BitcoinGold.Accuracy,
                Name = Constants.Assets.BitcoinGold.Name
            }
        };

        public Task<IPaginationResult<IAsset>> GetPaged(int take, string continuation)
        {
            return Task.FromResult(PaginationResult<IAsset>.Create(_mockList, null));
        }

        public Task<IAsset> GetById(string assetId)
        {
            return Task.FromResult(_mockList.SingleOrDefault(p=>p.AssetId == assetId));
        }
    }
}
