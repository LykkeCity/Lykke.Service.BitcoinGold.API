using NBitcoin;

namespace Lykke.Service.BitcoinGold.API.Core.Address
{
    public interface IAddressValidator
    {
        bool IsValid(string address);
        BitcoinAddress ParseAddress(string address);
        bool IsPubkeyValid(string pubkey);
        PubKey GetPubkey(string pubkey);
    }
}
