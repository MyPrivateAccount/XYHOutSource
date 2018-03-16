using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public interface ICustomerFollowUpStore
    {
        IQueryable<CustomerFollowUp> CustomerFollowUpAll();

        Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerFollowUp>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerFollowUp>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<CustomerFollowUp> CreateAsync(CustomerFollowUp customerfollowup, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(SimpleUser user, CustomerFollowUp customerfollowup, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<CustomerFollowUp> customerfollowupList, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(CustomerFollowUp customerfollowup, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<CustomerFollowUp> customerfollowups, CancellationToken cancellationToken = default(CancellationToken));
    }
}
