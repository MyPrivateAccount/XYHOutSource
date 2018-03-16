using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public interface ICustomerDealStore
    {
        IQueryable<CustomerDeal> CustomerDealAll();

        IQueryable<CustomerDeal> CustomerDealGetMy();

        Task<CustomerDeal> SubmitCreateAsync(CustomerDeal customerdeal, CancellationToken cancellationToken = default(CancellationToken));

        Task<CustomerDeal> CreateAsync(CustomerDeal customerdeal, CustomerInfo customerInfo, CustomerTransactionsFollowUp customerTransactionsFollowUp, CustomerTransactions customerTransactions, CustomerFollowUp customerFollowUp, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerDeal>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerDeal>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(CustomerDeal customerDeal, CancellationToken cancellationToken = default(CancellationToken));
    }
}
