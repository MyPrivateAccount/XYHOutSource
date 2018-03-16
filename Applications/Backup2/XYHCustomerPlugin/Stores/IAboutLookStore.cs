using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public interface IAboutLookStore
    {
        IQueryable<AboutLook> AboutLookAll();

        Task<TResult> GetAsync<TResult>(Func<IQueryable<AboutLook>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<AboutLook>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<AboutLook> CreateAsync(AboutLook aboutLook, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(AboutLook aboutLook, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<AboutLook> aboutLooks, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(SimpleUser user, AboutLook aboutLook, CancellationToken cancellationToken = default(CancellationToken));
    }
}
