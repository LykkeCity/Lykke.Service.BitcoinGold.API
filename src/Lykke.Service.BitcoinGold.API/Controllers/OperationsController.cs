using System;
using System.Net;
using System.Threading.Tasks;
using Common;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BitcoinGold.API.Core.Address;
using Lykke.Service.BitcoinGold.API.Core.Broadcast;
using Lykke.Service.BitcoinGold.API.Core.Constants;
using Lykke.Service.BitcoinGold.API.Core.Domain.Health.Exceptions;
using Lykke.Service.BitcoinGold.API.Core.ObservableOperation;
using Lykke.Service.BitcoinGold.API.Core.Operation;
using Lykke.Service.BitcoinGold.API.Core.Transactions;
using Lykke.Service.BitcoinGold.API.Helpers;
using Lykke.Service.BitcoinGold.API.Models;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.BitcoinGold.API.Controllers
{
    public class OperationsController : Controller
    {
        private readonly IOperationService _operationService;
        private readonly IAddressValidator _addressValidator;
        private readonly IBroadcastService _broadcastService;
        private readonly IObservableOperationService _observableOperationService;
        private readonly Network _network;


        public OperationsController(IOperationService operationService,
            IAddressValidator addressValidator,
            IBroadcastService broadcastService,
            IObservableOperationService observableOperationService, Network network)
        {
            _operationService = operationService;
            _addressValidator = addressValidator;
            _broadcastService = broadcastService;
            _observableOperationService = observableOperationService;
            _network = network;
        }

        [HttpPost("api/transactions/single")]
        [ProducesResponseType(typeof(BuildTransactionResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<ActionResult> BuildSingle([FromBody] BuildSingleTransactionRequest request)
        {
            if (request == null)
            {
                throw new BusinessException("Unable deserialize request", ErrorCode.BadInputParameter);
            }

            var amountSatoshi = MoneyConversionHelper.SatoshiFromContract(request.Amount);

            if (amountSatoshi <= 0)
            {
                throw new BusinessException($"Amount can't be less or equal to zero: {amountSatoshi}", ErrorCode.BadInputParameter);
            }

            if (request.AssetId != Constants.Assets.BitcoinGold.AssetId)
            {

                throw new BusinessException("Invalid assetId", ErrorCode.BadInputParameter);
            }

            var toBitcoinAddress = _addressValidator.ParseAddress(request.ToAddress);
            if (toBitcoinAddress == null)
            {

                throw new BusinessException("Invalid ToAddress ", ErrorCode.BadInputParameter);
            }

            var fromBitcoinAddress = _addressValidator.ParseAddress(request.FromAddress);
            if (fromBitcoinAddress == null)
            {

                throw new BusinessException("Invalid FromAddress", ErrorCode.BadInputParameter);
            }

            if (request.OperationId == Guid.Empty)
            {
                throw new BusinessException("Invalid operation id (GUID)", ErrorCode.BadInputParameter);
            }


            PubKey fromAddressPubkey = null;
            var pubKeyString = request.FromAddressContext?.DeserializeJson<AddressContextContract>()?.PubKey;

            if (pubKeyString != null)
            {
                if (!_addressValidator.IsPubkeyValid(pubKeyString))
                {
                    throw new BusinessException("Invalid pubkey string", ErrorCode.BadInputParameter);
                }

                fromAddressPubkey = _addressValidator.GetPubkey(pubKeyString);
            }

            BuiltTransactionInfo tx;
            try
            {
                tx = await _operationService.GetOrBuildTransferTransaction(request.OperationId, fromBitcoinAddress, fromAddressPubkey, toBitcoinAddress,
                    request.AssetId, new Money(amountSatoshi), request.IncludeFee);
            }
            catch (NotEnoughFundsException)
            {
                return BadRequest(BlockchainErrorResponse.FromKnownError(BlockchainErrorCode.NotEnoughBalance));
            }
            catch (BusinessException e) when(e.Code == ErrorCode.NotEnoughFundsAvailable)
            {
                return BadRequest(BlockchainErrorResponse.FromKnownError(BlockchainErrorCode.NotEnoughBalance));
            }


            return Ok(new BuildTransactionResponse
            {
                TransactionContext = tx.ToJson(_network)
            });
        }

        [HttpPost("api/transactions/broadcast")]
        [SwaggerOperation(nameof(BroadcastTransaction))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> BroadcastTransaction([FromBody] BroadcastTransactionRequest request)
        {
            if (request == null)
            {
                throw new BusinessException("Unable deserialize request", ErrorCode.BadInputParameter);
            }

            try
            {
                await _broadcastService.BroadCastTransaction(request.OperationId, request.SignedTransaction);
            }
            catch (BusinessException e) when (e.Code == ErrorCode.TransactionAlreadyBroadcasted)
            {
                return new StatusCodeResult(409);
            }
            catch (BusinessException e) when (e.Code == ErrorCode.OperationNotFound)
            {
                return new StatusCodeResult((int)HttpStatusCode.NoContent);
            }

            return Ok();
        }

        [HttpGet("api/transactions/broadcast/single/{operationId}")]
        [SwaggerOperation(nameof(GetObservableSingleOperation))]
        [ProducesResponseType(typeof(BroadcastedSingleTransactionResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetObservableSingleOperation(Guid operationId)
        {
            if (operationId == Guid.Empty)
            {
                return BadRequest(ErrorResponse.Create("Invalid parameter").AddModelError(nameof(operationId), "Must be valid guid"));
            }

            var result = await _observableOperationService.GetById(operationId);

            if (result == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NoContent);
            }

            BroadcastedTransactionState MapState(BroadcastStatus status)
            {
                switch (status)
                {
                    case BroadcastStatus.Completed:
                        return BroadcastedTransactionState.Completed;
                    case BroadcastStatus.Failed:
                        return BroadcastedTransactionState.Failed;
                    case BroadcastStatus.InProgress:
                        return BroadcastedTransactionState.InProgress;
                    default:
                        throw new InvalidCastException($"Unknown mapping from {status} ");
                }
            }


            return Ok(new BroadcastedSingleTransactionResponse
            {
                Amount = MoneyConversionHelper.SatoshiToContract(result.AmountSatoshi),
                Fee = MoneyConversionHelper.SatoshiToContract(result.FeeSatoshi),
                OperationId = result.OperationId,
                Hash = result.TxHash,
                Timestamp = result.Updated,
                State = MapState(result.Status),
                Block = result.UpdatedAtBlockHeight
            });
        }

        [HttpDelete("api/transactions/broadcast/{operationId}")]
        [SwaggerOperation(nameof(RemoveObservableOperation))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> RemoveObservableOperation(Guid operationId)
        {
            if (operationId == Guid.Empty)
            {
                return BadRequest(ErrorResponse.Create("Invalid parameter").AddModelError(nameof(operationId), "Must be valid guid"));
            }
            await _observableOperationService.DeleteOperations(operationId);

            return Ok();
        }
    }
}
