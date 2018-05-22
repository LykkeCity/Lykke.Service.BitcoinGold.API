using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.BitcoinGold.API.Core.BlockChainReaders;
using Lykke.Service.BitcoinGold.API.Core.Constants;
using Lykke.Service.BitcoinGold.API.Core.Transactions;
using MoreLinq;
using NBitcoin;

namespace Lykke.Service.BitcoinGold.API.Services.Transactions
{
    public class HistoryService : IHistoryService
    {
        private readonly IBlockChainProvider _blockChainProvider;

        public HistoryService(IBlockChainProvider blockChainProvider)
        {
            _blockChainProvider = blockChainProvider;
        }

        public Task<IEnumerable<HistoricalTransactionDto>> GetHistoryFrom(BitcoinAddress address, string afterHash, int take)
        {
            return GetHistory(address.ToString(), afterHash, take, true);
        }

        public Task<IEnumerable<HistoricalTransactionDto>> GetHistoryTo(BitcoinAddress address, string afterHash, int take)
        {
            return GetHistory(address.ToString(), afterHash, take, false);
        }

        private async Task<IEnumerable<HistoricalTransactionDto>> GetHistory(string address, string afterHash, int take, bool isSending)
        {
            var txIds = (await _blockChainProvider.GetTransactionsForAddress(address)).Reverse();

            if (!string.IsNullOrEmpty(afterHash))
            {
                txIds = txIds.SkipWhile(p => p != afterHash).Skip(1);
            }

            var result = new List<HistoricalTransactionDto>();

            foreach (var txId in txIds)
            {
                if (result.Count >= take)
                    break;

                var tx = await _blockChainProvider.GetTransaction(txId);
                var dto = MapToHistoricalTransaction(tx, address);

                if (dto.IsSending == isSending)
                    result.Add(dto);
            }

            return result;
        }


        private HistoricalTransactionDto MapToHistoricalTransaction(BtgTransaction tx, string requestedAddress)
        {
            var isSending = tx.Inputs.Where(p => p.Address == requestedAddress).Sum(p => p.Value) >=
                            tx.Outputs.Where(p => p.Address == requestedAddress).Sum(p => p.Value);
            string from;
            string to;
            long amount;
            if (isSending)
            {
                from = requestedAddress;
                to = tx.Outputs.Select(o => o.Address).FirstOrDefault(o => o != null && o != requestedAddress) ?? requestedAddress;
                amount = tx.Outputs.Where(o => o.Address != requestedAddress).Sum(o => o.Value);
            }
            else
            {
                to = requestedAddress;
                from = tx.Inputs.Select(o => o.Address).FirstOrDefault(o => o != null && o != requestedAddress) ?? requestedAddress;
                amount = tx.Outputs.Where(o => o.Address == requestedAddress).Sum(o => o.Value);
            }

            return new HistoricalTransactionDto
            {
                TxHash = tx.Hash,
                IsSending = isSending,
                AmountSatoshi = amount,
                FromAddress = from,
                AssetId = Constants.Assets.BitcoinGold.AssetId,
                ToAddress = to,
                TimeStamp = tx.Timestamp
            };
        }
    }
}
