using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Flurl;
using Flurl.Http;
using Lykke.Service.BitcoinGold.API.Core.BlockChainReaders;
using Lykke.Service.BitcoinGold.API.Core.Domain.Health.Exceptions;
using Lykke.Service.BitcoinGold.API.Services.BlockChainProviders.InsightApi.Contracts;
using NBitcoin;
using Utils = Common.Utils;

namespace Lykke.Service.BitcoinGold.API.Services.BlockChainProviders.InsightApi
{
    internal class InsightApiBlockChainProvider : IBlockChainProvider
    {
        private readonly InsightApiSettings _insightApiSettings;
        private readonly ILog _log;

        public InsightApiBlockChainProvider(InsightApiSettings insightApiSettings, ILog log)
        {
            _insightApiSettings = insightApiSettings;
            _log = log;
        }

        public async Task<int> GetLastBlockHeight()
        {
            var url = _insightApiSettings.Url
                .AppendPathSegment("insight-api/status");


            var resp = await GetJson<StatusResponceContract>(url);

            return resp.Info.LastBlockHeight;
        }


        public async Task<IEnumerable<string>> GetTransactionsForAddress(string address)
        {
            var result = new List<string>();

            const int batchSize = 1000;

            var allTxLoaded = false;
            int counter = 0;

            while (!allTxLoaded)
            {
                var url = _insightApiSettings.Url
                    .AppendPathSegment($"insight-api/addr/{address}")
                    .SetQueryParam("from", counter)
                    .SetQueryParam("to", counter + batchSize);

                var resp = await GetJson<AddressBalanceResponceContract>(url);

                result.AddRange(resp.Transactions);
                allTxLoaded = !resp.Transactions.Any();

                counter += batchSize;
            }

            return result;
        }

        public async Task<BtgTransaction> GetTransaction(string txHash)
        {
            var tx = await GetTx((txHash));

            if (tx == null) return null;

            var btgTx = new BtgTransaction
            {
                Hash = txHash,
                Timestamp = ((uint)tx.BlockTime).FromUnixDateTime()
            };

            btgTx.Inputs = tx.Inputs.Select(o => new BtgInput()
            {
                Address = o.Address,
                Value = new Money(o.AmountSatoshi)
            }).ToList();

            btgTx.Outputs = tx.Outputs.Select(o => new BtgOutput()
            {
                Address = o.ScriptPubKey.Addresses.FirstOrDefault(),
                Value = new Money(o.ValueBtc, MoneyUnit.BTC)
            }).ToList();
            return btgTx;
        }


        public async Task BroadCastTransaction(Transaction tx)
        {
            await _insightApiSettings.Url.AppendPathSegment("insight-api/tx/send")
                .PostJsonAsync(new BroadcastTransactionRequestContract
                {
                    RawTx = tx.ToHex()
                })
                .ReceiveJson<BroadcastTransactionResponceContract>();
        }

        public async Task<int> GetTxConfirmationCount(string txHash)
        {
            return (await GetTx(txHash))?.Confirmation ?? 0;
        }

        public async Task<IList<Coin>> GetUnspentOutputs(string address, int minConfirmationCount)
        {
            var resp = await GetUnspentOutputsResponce(address);

            return resp.Where(p => p.Confirmation >= minConfirmationCount).Select(MapUnspentCoun).ToList();
        }

        public async Task<long> GetBalanceSatoshiFromUnspentOutputs(string address, int minConfirmationCount)
        {
            var unspentOutputs = (await GetUnspentOutputsResponce(address)).Where(p => p.Confirmation >= minConfirmationCount);

            return unspentOutputs.Sum(p => p.Satoshi);
        }

        private async Task<IEnumerable<AddressUnspentOutputsResponce>> GetUnspentOutputsResponce(string address)
        {
            var url = _insightApiSettings.Url
                .AppendPathSegment($"insight-api/addr/{address}/utxo");

            return await GetJson<AddressUnspentOutputsResponce[]>(url);
        }

        private Coin MapUnspentCoun(AddressUnspentOutputsResponce source)
        {
            return new Coin(new OutPoint(uint256.Parse(source.TxHash), source.N), new TxOut(new Money(source.Satoshi, MoneyUnit.Satoshi), source.ScriptPubKey.ToScript()));
        }

        private async Task<TxResponceContract> GetTx(string txHash)
        {
            try
            {
                var url = _insightApiSettings.Url
                    .AppendPathSegment($"insight-api/tx/{txHash}");

                var resp = await GetJson<TxResponceContract>(url);

                return resp;
            }
            catch (FlurlHttpException e) when (e.Call.Response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        private async Task<T> GetJson<T>(Url url, int tryCount = 3)
        {
            bool NeedToRetryException(Exception ex)
            {

                if (!(ex is FlurlHttpException flurlException))
                {
                    return false;
                }

                var isTimeout = flurlException is FlurlHttpTimeoutException;
                if (isTimeout)
                {
                    return true;
                }

                if (flurlException.Call.HttpStatus == HttpStatusCode.ServiceUnavailable ||
                    flurlException.Call.HttpStatus == HttpStatusCode.InternalServerError)
                {
                    return true;
                }

                return false;
            }


            try
            {

                return await Retry.Try(() => url.GetJsonAsync<T>(), NeedToRetryException, tryCount, _log);
            }
            catch (FlurlHttpException e)
            {
                throw new BusinessException("Error while proceeding operation within Blockchain Insight Api", ErrorCode.BlockChainApiError, e);
            }
        }
    }
}
