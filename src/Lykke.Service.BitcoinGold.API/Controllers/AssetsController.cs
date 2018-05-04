using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BitcoinGold.API.Core.Asset;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Assets;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.BitcoinGold.API.Controllers
{
    public class AssetsController: Controller
    {
        private readonly IAssetRepository _assetRepository;

        public AssetsController(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }

        [SwaggerOperation(nameof(GetPaged))]
        [ProducesResponseType(typeof(PaginationResponse<AssetResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [HttpGet("api/assets")]
        public async Task<PaginationResponse<AssetResponse>> GetPaged([FromQuery]int take, [FromQuery]string continuation)
        {
            var paginationResult = await _assetRepository.GetPaged(take, continuation);

            return PaginationResponse.From(paginationResult.Continuation, paginationResult.Items.Select(p => new AssetResponse
            {
                Address = p.Address,
                AssetId = p.AssetId,
                Accuracy = p.Accuracy,
                Name = p.Name
            }).ToList().AsReadOnly());
        }

        [SwaggerOperation(nameof(GetById))]
        [ProducesResponseType(typeof(AssetResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(AssetResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [HttpGet("api/assets/{assetId}")]
        public async Task<IActionResult> GetById(string assetId)
        {
            var asset = await _assetRepository.GetById(assetId);
            if (asset == null)
            {
                return NotFound();
            }

            return Ok(new AssetResponse
            {
                Address = asset.Address,
                AssetId = asset.AssetId,
                Accuracy = asset.Accuracy,
                Name = asset.Name
            });
        }
    }
}
