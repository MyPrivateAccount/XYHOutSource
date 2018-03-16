using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public interface IDealFileScopeStore
    {
        IQueryable<DealFileScope> DealFileScopes { get; set; }

        Task<DealFileScope> CreateAsync(DealFileScope dealFileScope, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(DealFileScope dealFileScope, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<DealFileScope> dealFileScopeList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<DealFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<DealFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(DealFileScope shopsFileScope, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<DealFileScope> dealFileScopeList, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveAsync(SimpleUser user, string dealId, List<DealFileScope> shopsFileScopeList, CancellationToken cancellationToken = default(CancellationToken));


    }
}
