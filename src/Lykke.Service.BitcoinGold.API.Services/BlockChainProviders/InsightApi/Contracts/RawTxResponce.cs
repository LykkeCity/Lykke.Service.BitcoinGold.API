using Newtonsoft.Json;

namespace Lykke.Service.BitcoinGold.API.Services.BlockChainProviders.InsightApi.Contracts
{
    public class RawTxResponce
    {
        [JsonProperty("rawtx")]
        public string RawTx { get; set; }
    }
}
