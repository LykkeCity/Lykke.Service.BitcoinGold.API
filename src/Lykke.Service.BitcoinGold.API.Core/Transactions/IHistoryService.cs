using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.BitcoinGold.API.Core.Transactions
{
    public class HistoricalTransactionDto
    {
        public DateTime TimeStamp { get; set; }

        public string FromAddress { get; set; }
        public string ToAddress { get; set; }

        public string AssetId { get; set; }

        public long AmountSatoshi { get; set; }

        public string TxHash { get; set; }

        public bool IsSending { get; set; }
    }

    public interface IHistoryService
    {
        Task<IEnumerable<HistoricalTransactionDto>> GetHistoryFrom(BitcoinAddress address, string afterHash, int take);
        Task<IEnumerable<HistoricalTransactionDto>> GetHistoryTo(BitcoinAddress address, string afterHash, int take);
    }
}
