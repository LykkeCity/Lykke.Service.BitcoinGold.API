using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.BitcoinGold.API.Core.TransactionOutputs;
using Lykke.Service.BitcoinGold.API.Services.TransactionOutputs;

namespace Lykke.Job.BitcoinGold.Functions
{
    public class RemoveOldSpentOutputsFunction
    {
        private readonly ISpentOutputRepository _spentOutputRepository;
        private readonly SpentOutputsSettings _settings;

        public RemoveOldSpentOutputsFunction(ISpentOutputRepository spentOutputRepository, SpentOutputsSettings settings)
        {
            _spentOutputRepository = spentOutputRepository;
            _settings = settings;
        }

        [TimerTrigger("01:30:00")]
        public async Task Clean()
        {
            var bound = DateTime.UtcNow.AddDays(-_settings.SpentOutputsExpirationDays);
            await _spentOutputRepository.RemoveOldOutputs(bound);
        }
    }
}
