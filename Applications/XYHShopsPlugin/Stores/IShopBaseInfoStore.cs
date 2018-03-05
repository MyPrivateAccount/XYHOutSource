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
    public interface IShopBaseInfoStore
    {
        IQueryable<ShopBaseInfo> ShopBaseInfos { get; set; }

        IQueryable<ShopBaseInfo> GetSimpleShopBase();

        Task<ShopBaseInfo> CreateAsync(ShopBaseInfo shopBaseInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(ShopBaseInfo shopBaseInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<ShopBaseInfo> shopBaseInfoList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<ShopBaseInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<ShopBaseInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListSellHistoryAsync<TResult>(Func<IQueryable<SellHistory>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateSaleStatusAsync(List<ShopBaseInfo> shopBaseInfos, List<SellHistory> sellHistories, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(ShopBaseInfo shopBaseInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<ShopBaseInfo> shopBaseInfoList, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveAsync(SimpleUser user, string buildingId, ShopBaseInfo shopBaseInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> CheckDuplicateShop(string buildingId, string shopId, string buildingNo, string floorNo, string number, CancellationToken cancellationToken = default(CancellationToken));
    }
}
