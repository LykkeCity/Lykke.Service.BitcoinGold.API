using System.Net;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BitcoinGold.API.Core.Address;
using Lykke.Service.BlockchainApi.Contract.Addresses;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.BitcoinGold.API.Controllers
{
    public class AddressController:Controller
    {
        private readonly IAddressValidator _addressValidator;

        public AddressController(IAddressValidator addressValidator)
        {
            _addressValidator = addressValidator;
        }
        [SwaggerOperation(nameof(Validate))]
        [ProducesResponseType(typeof(AddressValidationResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [HttpGet("api/addresses/{address}/validity")]
        public AddressValidationResponse Validate(string address)
        {
            return new AddressValidationResponse
            {
                IsValid = _addressValidator.IsValid(address)
            };
        }
    }
}
