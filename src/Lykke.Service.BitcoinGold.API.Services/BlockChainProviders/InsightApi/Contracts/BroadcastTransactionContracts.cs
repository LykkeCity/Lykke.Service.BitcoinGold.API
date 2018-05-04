using Newtonsoft.Json;

namespace Lykke.Service.BitcoinGold.API.Services.BlockChainProviders.InsightApi.Contracts
{
    internal class BroadcastTransactionRequestContract
    {
        [JsonProperty("rawtx")]
        public string RawTx { get; set; }
    }

    internal class BroadcastTransactionResponceContract
    {
        [JsonProperty("txid")]
        public string TransactionId { get; set; }
    }
}
