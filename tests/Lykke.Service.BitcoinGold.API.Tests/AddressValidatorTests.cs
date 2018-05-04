using Lykke.Service.BitcoinGold.API.Services.Address;
using Xunit;

namespace Lykke.Service.BitcoinGold.API.Tests
{
    public class AddressValidatorTests
    {
        [Fact]
        public void CanPassValidAddress()
        {


            var addresses = new[]
            {
                "mycEmS1rCNnrjzU2PVfC14bvhj8Kj8Uqio",
                "n2gGJpH3PuXBVn2o3ALiQjiNodcEJDRJHh"
            };
            var addressValidator = new AddressValidator(NBitcoin.Altcoins.BGold.Instance.Testnet);

            foreach (var address in addresses)
            {
                Assert.True(addressValidator.IsValid(address));

            }
        }

        [Fact]
        public void CanPassValidMainNetAddress()
        {
            var addresses = new[]
            {
                "GQ6Btf3KmRz4VoMsEZi7WBdCYuY1XeXTrY",
                "AJcr6T2b5C8UcJusDHR18jESaqUm5jhRPo"
            };
            var addressValidator = new AddressValidator(NBitcoin.Altcoins.BGold.Instance.Mainnet);

            foreach (var address in addresses)
            {
                Assert.True(addressValidator.IsValid(address));

            }
        }

        [Fact]
        public void CanDetectInvalidAddress()
        {

            var invalidAddress = "invalid";
            var addressValidator = new AddressValidator(NBitcoin.Altcoins.BGold.Instance.Testnet);

            Assert.False(addressValidator.IsValid(invalidAddress));
        }
    }

}
