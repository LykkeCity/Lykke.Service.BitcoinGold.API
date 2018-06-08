using System.Collections.Generic;

namespace Lykke.Service.BitcoinGold.API.Core.Pagination
{
    public interface IPaginationResult<T>
    {
        IEnumerable<T> Items { get; }

        string Continuation { get; }
    }

    public class PaginationResult<T> : IPaginationResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public string Continuation { get; set; }

        public static IPaginationResult<T> Create(IEnumerable<T> items, string continuation)
        {
            return new PaginationResult<T>
            {
                Continuation = continuation,
                Items = items
            };
        }
    }
}
