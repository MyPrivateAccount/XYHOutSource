using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Stores
{
    public interface IShopsFileScopeStore
    {
        IQueryable<ShopsFileScope> ShopsFileScopes { get; set; }
        
        Task<ShopsFileScope> CreateAsync(ShopsFileScope shopsFileScope, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(ShopsFileScope shopsFileScope, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<ShopsFileScope> shopsFileScopeList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<ShopsFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<ShopsFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(ShopsFileScope shopsFileScope, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<ShopsFileScope> shopsFileScopeList, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveAsync(SimpleUser user, string buildingId, List<ShopsFileScope> shopsFileScopeList, CancellationToken cancellationToken = default(CancellationToken));
    }
}
