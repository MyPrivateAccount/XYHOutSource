using ApplicationCore.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Dto;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public interface ICustomerPoolStore
    {
        IQueryable<CustomerPool> CustomerPools { get; set; }

        Task<CustomerPool> CreateAsync(CustomerPool customerPool, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<CustomerPool>> CreateListAsync(List<CustomerPool> customerPools, List<MigrationPoolHistory> migrationPoolHistory, List<CustomerFollowUp> customerFollowUp, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(CustomerPool customerPool, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<CustomerPool> customerPoolList, CancellationToken cancellationToken = default(CancellationToken));
        IQueryable<CustomerPoolResponse> GetQuery(string poolId);

        Task JoinCustomerAsync(UserInfo user, CustomerPoolJoinRequest customerPoolJoinRequest, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> ClaimCustomerAsync(UserInfo user, CustomerPoolClaimRequest customerPoolClaimRequest, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerPool>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerPool>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(CustomerPool customerPool, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<CustomerPool> customerPoolList,List<MigrationPoolHistory> migrationPoolHistory, CancellationToken cancellationToken = default(CancellationToken));
    }
}
