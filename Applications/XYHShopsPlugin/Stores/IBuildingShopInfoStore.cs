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
    public interface IBuildingShopInfoStore
    {
        IQueryable<BuildingShopInfo> BuildingShopInfos { get; set; }

        Task<BuildingShopInfo> CreateAsync(BuildingShopInfo buildingShopInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(BuildingShopInfo buildingShopInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<BuildingShopInfo> buildingShopInfoList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingShopInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BuildingShopInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(BuildingShopInfo buildingShopInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<BuildingShopInfo> buildingShopInfoList, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveAsync(SimpleUser user, BuildingShopInfo buildingShopInfo, CancellationToken cancellationToken = default(CancellationToken));

    }
}
