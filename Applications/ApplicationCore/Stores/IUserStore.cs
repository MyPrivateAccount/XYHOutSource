using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationCore.Stores
{
    public interface IUserStore
    {
        Task<TResult> GetAsync<TResult>(Func<IQueryable<Users>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<Users>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
    }
}
