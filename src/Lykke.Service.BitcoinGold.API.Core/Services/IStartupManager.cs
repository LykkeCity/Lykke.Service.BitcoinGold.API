using System.Threading.Tasks;

namespace Lykke.Service.BitcoinGold.API.Core.Services
{
    public interface IStartupManager
    {
        Task StartAsync();
    }
}