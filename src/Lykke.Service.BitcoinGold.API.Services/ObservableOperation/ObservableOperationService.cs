using System;
using System.Threading.Tasks;
using Lykke.Service.BitcoinGold.API.Core.ObservableOperation;
using Lykke.Service.BitcoinGold.API.Core.Transactions;

namespace Lykke.Service.BitcoinGold.API.Services.ObservableOperation
{
    public class ObservableOperationService: IObservableOperationService
    {
        private readonly IObservableOperationRepository _observableOperationRepository;
        private readonly IUnconfirmedTransactionRepository _unconfirmedTransactionRepository;

        public ObservableOperationService(IObservableOperationRepository observableOperationRepository, IUnconfirmedTransactionRepository unconfirmedTransactionRepository)
        {
            _observableOperationRepository = observableOperationRepository;
            _unconfirmedTransactionRepository = unconfirmedTransactionRepository;
        }
        
        public async Task DeleteOperations(params Guid[] opIds)
        {
            await _observableOperationRepository.DeleteIfExist(opIds);
            await _unconfirmedTransactionRepository.DeleteIfExist(opIds);
        }

        public Task<IObservableOperation> GetById(Guid opId)
        {
            return _observableOperationRepository.GetById(opId);
        }
    }
}
