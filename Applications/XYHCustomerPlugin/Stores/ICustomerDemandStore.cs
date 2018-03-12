using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public interface ICustomerDemandStore
    {
        Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerDemand>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerDemand>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<CustomerDemand> CreateAsync(CustomerDemand customerDemand, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(CustomerDemand customerDemand, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<CustomerDemand> customerDemands, CancellationToken cancellationToken = default(CancellationToken));
    }
}
