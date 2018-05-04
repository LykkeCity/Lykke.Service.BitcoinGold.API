using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.BitcoinGold.API.Core.TransactionOutputs
{
    public interface ISpentOutputRepository
    {
        Task InsertSpentOutputs(Guid transactionId, IEnumerable<IOutput> outputs);

        Task<IEnumerable<IOutput>> GetSpentOutputs(IEnumerable<IOutput> outputs);

        Task RemoveOldOutputs(DateTime bound);
    }
}
