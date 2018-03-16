using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public interface ICustomerFilescopeStore
    {
        IQueryable<CustomerFileScope> CustomerFileScopes { get; set; }

        Task<CustomerFileScope> CreateAsync(CustomerFileScope customerFileScope, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(CustomerFileScope customerFileScope, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<CustomerFileScope> customerFileScopeList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(CustomerFileScope customerFileScope, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<CustomerFileScope> customerFileScopeList, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveAsync(SimpleUser user, string buildingId, List<CustomerFileScope> customerFileScopeList, CancellationToken cancellationToken = default(CancellationToken));
    }
}
