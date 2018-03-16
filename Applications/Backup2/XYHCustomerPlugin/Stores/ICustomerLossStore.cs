using ApplicationCore.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public interface ICustomerLossStore
    {
        IQueryable<CustomerLoss> CustomerLosss { get; set; }

        Task<CustomerLoss> CreateAsync(CustomerLoss customerLoss, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(CustomerLoss customerLoss, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<CustomerLoss> customerLossList, CancellationToken cancellationToken = default(CancellationToken));

        IQueryable<CustomerLoss> SimpleQuery();

        Task<bool> ActivateLossUser(UserInfo user, string Id, bool isDeleteOldData, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerLoss>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerLoss>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(CustomerLoss customerLoss, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<CustomerLoss> customerLossList, CancellationToken cancellationToken = default(CancellationToken));
    }
}
