using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BitcoinGold.API.Core.Address;
using Lykke.Service.BitcoinGold.API.Core.Domain.Health.Exceptions;
using Lykke.Service.BitcoinGold.API.Core.Transactions;
using Lykke.Service.BitcoinGold.API.Helpers;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.BitcoinGold.API.Controllers
{
    [Route("api/transactions/history")]
    public class HistoryController : Controller
    {
        private readonly IHistoryService _historyService;
        private readonly IAddressValidator _addressValidator;

        public HistoryController(IHistoryService historyService, IAddressValidator addressValidator)
        {
            _historyService = historyService;
            _addressValidator = addressValidator;
        }

        [HttpPost("from/{address}/observation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult ObserveFrom(
            [FromRoute] string address)
        {
            return Ok();
        }

        [HttpPost("to/{address}/observation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult ObserveTo(
            [FromRoute] string address)
        {
            return Ok();
        }

        [HttpDelete("from/{address}/observation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult DeleteObservationFrom(
            [FromRoute] string address)
        {
            return Ok();
        }

        [HttpDelete("to/{address}/observation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult DeleteObservationTo(
            [FromRoute] string address)
        {
            return Ok();
        }

        [HttpGet("from/{address}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(HistoricalTransactionContract[]))]
        public async Task<IActionResult> GetHistoryFrom(
            [FromRoute] string address,
            [FromQuery] string afterHash,
            [FromQuery] int take)
        {
            if (take <= 0)
            {
                return BadRequest(new ErrorResponse() { ErrorMessage = $"{nameof(take)} must be greater than zero" });
            }

            if (!_addressValidator.IsValid(address))
            {
                throw new BusinessException($"Invalid BTG address ${address}", ErrorCode.BadInputParameter);
            }

            var addr = _addressValidator.ParseAddress(address);
            var result = await _historyService.GetHistoryFrom(addr, afterHash, take);

            return Ok(result.Select(ToHistoricalTransaction));
        }

        [HttpGet("to/{address}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(HistoricalTransactionContract[]))]
        public async Task<IActionResult> GetHistoryTo(
            [FromRoute] string address,
            [FromQuery] string afterHash,
            [FromQuery] int take)
        {
            if (take <= 0)
            {
                return BadRequest(new ErrorResponse() { ErrorMessage = $"{nameof(take)} must be greater than zero" });
            }

            if (!_addressValidator.IsValid(address))
            {
                throw new BusinessException($"Invalid BTG address ${address}", ErrorCode.BadInputParameter);
            }

            var btcAddress = _addressValidator.ParseAddress(address);
            var result = await _historyService.GetHistoryTo(btcAddress, afterHash, take);

            return Ok(result.Select(ToHistoricalTransaction));
        }

        private HistoricalTransactionContract ToHistoricalTransaction(HistoricalTransactionDto source)
        {
            return new HistoricalTransactionContract
            {
                ToAddress = source.ToAddress,
                FromAddress = source.FromAddress,
                AssetId = source.AssetId,
                Amount = MoneyConversionHelper.SatoshiToContract(source.AmountSatoshi),
                Hash = source.TxHash,
                Timestamp = source.TimeStamp,
                TransactionType = source.IsSending ? TransactionType.Send : TransactionType.Receive
            };
        }
    }
}
