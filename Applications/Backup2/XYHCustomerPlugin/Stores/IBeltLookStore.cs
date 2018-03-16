using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public interface IBeltLookStore
    {
        IQueryable<BeltLook> BeltLookAll();

        Task<TResult> GetAsync<TResult>(Func<IQueryable<BeltLook>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BeltLook>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<BeltLook> CreateAsync(BeltLook beltLook, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<BeltLook>> CreateListAsync(List<BeltLook> beltLooks, List<CustomerTransactions> customertransactionss, List<CustomerTransactionsFollowUp> customertransactionstollowups, List<CustomerFollowUp> customerFollowUps, CancellationToken cancellationToken = default(CancellationToken));

        Task CreateAgainBeltLookAsync(CustomerTransactions customertransactions, CustomerTransactionsFollowUp customertransactionstollowup, CustomerFollowUp customerFollowUp, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(BeltLook beltLook, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<BeltLook> beltLooks, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(SimpleUser user, BeltLook beltLook, CancellationToken cancellationToken = default(CancellationToken));
    }
}
