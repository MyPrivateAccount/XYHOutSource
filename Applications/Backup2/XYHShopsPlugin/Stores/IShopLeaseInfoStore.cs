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
    public interface IShopLeaseInfoStore
    {
        IQueryable<ShopLeaseInfo> ShopLeaseInfos { get; set; }

        Task<ShopLeaseInfo> CreateAsync(ShopLeaseInfo shopLeaseInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(ShopLeaseInfo shopLeaseInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<ShopLeaseInfo> shopLeaseInfoList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<ShopLeaseInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<ShopLeaseInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(ShopLeaseInfo shopLeaseInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<ShopLeaseInfo> shopLeaseInfoList, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveAsync(SimpleUser user, string buildingId, ShopLeaseInfo shopLeaseInfo, CancellationToken cancellationToken = default(CancellationToken));

    }
}
