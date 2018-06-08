using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BitcoinGold.API.Core.Address;
using Lykke.Service.BitcoinGold.API.Core.Constants;
using Lykke.Service.BitcoinGold.API.Core.Domain.Health.Exceptions;
using Lykke.Service.BitcoinGold.API.Core.Wallet;
using Lykke.Service.BitcoinGold.API.Helpers;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Balances;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.BitcoinGold.API.Controllers
{
    public class BalancesController : Controller
    {
        private readonly IAddressValidator _addressValidator;
        private readonly IWalletBalanceService _balanceService;

        public BalancesController(IAddressValidator addressValidator, IWalletBalanceService balanceService)
        {
            _addressValidator = addressValidator;
            _balanceService = balanceService;
        }

        [HttpPost("api/balances/{address}/observation")]
        [SwaggerOperation(nameof(Subscribe))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        public async Task<IActionResult> Subscribe(string address)
        {
            if (!_addressValidator.IsValid(address))
            {
                return BadRequest(ErrorResponse.Create("Invalid address"));
            }

            try
            {
                await _balanceService.Subscribe(address);
            }
            catch (BusinessException e) when (e.Code == ErrorCode.EntityAlreadyExist)
            {

                return StatusCode(409);
            }

            return Ok();
        }

        [HttpDelete("api/balances/{address}/observation")]
        [SwaggerOperation(nameof(Unsubscribe))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 204)]
        public async Task<IActionResult> Unsubscribe(string address)
        {
            if (!_addressValidator.IsValid(address))
            {
                return BadRequest(ErrorResponse.Create("Invalid address"));
            }

            try
            {
                await _balanceService.Unsubscribe(address);
            }
            catch (BusinessException e) when (e.Code == ErrorCode.EntityNotExist)
            {

                return StatusCode((int)HttpStatusCode.NoContent);
            }

            return Ok();
        }

        [HttpGet("api/balances/")]
        [SwaggerOperation(nameof(GetBalances))]
        [ProducesResponseType(typeof(PaginationResponse<WalletBalanceContract>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetBalances([FromQuery, Required]int take, [FromQuery] string continuation)
        {
            if (take < 1)
            {
                return BadRequest(ErrorResponse.Create("Invalid parameter").AddModelError("take", "Must be positive non zero integer"));
            }
            if (!string.IsNullOrEmpty(continuation))
            {
                try
                {
                    JsonConvert.DeserializeObject<TableContinuationToken>(Utils.HexToString(continuation));
                }
                catch (JsonReaderException)
                {
                    return BadRequest(ErrorResponse.Create("Invalid parameter").AddModelError("continuation", "Must be valid continuation token"));
                }
            }

            var padedResult = await _balanceService.GetBalances(take, continuation);

            return Ok(PaginationResponse.From(padedResult.Continuation, padedResult.Items.Select(p => new WalletBalanceContract
            {
                Address = p.Address,
                Balance = MoneyConversionHelper.SatoshiToContract(p.BalanceSatoshi),
                AssetId = Constants.Assets.BitcoinGold.AssetId,
                Block = p.UpdatedAtBlockHeight
            }).ToList().AsReadOnly()));
        }
    }
}
