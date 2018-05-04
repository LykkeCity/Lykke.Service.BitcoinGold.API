using System;
using System.Threading.Tasks;

namespace Lykke.Service.BitcoinGold.API.Core.ObservableOperation
{
    public interface IObservableOperationService
    {
        Task DeleteOperations(params Guid[] opIds);
        Task<IObservableOperation> GetById(Guid opId);
    }
}
