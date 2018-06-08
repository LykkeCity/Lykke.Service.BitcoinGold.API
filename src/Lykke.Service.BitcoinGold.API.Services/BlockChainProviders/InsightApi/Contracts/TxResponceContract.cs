using Newtonsoft.Json;

namespace Lykke.Service.BitcoinGold.API.Services.BlockChainProviders.InsightApi.Contracts
{
    public class TxResponceContract
    {
        [JsonProperty("confirmations")]
        public int Confirmation { get; set; }

        [JsonProperty("vout")]
        public OutputContract[] Outputs { get; set; }

        [JsonProperty("vin")]
        public InputContract[] Inputs { get; set; }

        [JsonProperty("blocktime")]
        public long BlockTime { get; set; }

        public class OutputContract
        {
            [JsonProperty("n")]
            public uint N { get; set; }

            [JsonProperty("scriptPubKey")]
            public ScriptPubKeyContract ScriptPubKey { get; set; }
            public class ScriptPubKeyContract
            {
                [JsonProperty("addresses")]
                public string[] Addresses { get; set; }
            }

            [JsonProperty("value")]
            public decimal ValueBtc { get; set; }
        }

        public class InputContract
        {
            [JsonProperty("valueSat")]
            public long AmountSatoshi { get; set; }

            [JsonProperty("addr")]
            public string Address { get; set; }
        }
    }




}
