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
    public interface IBuildingBaseInfoStore
    {
        IQueryable<BuildingBaseInfo> BuildingBaseInfos { get; set; }

        Task<BuildingBaseInfo> CreateAsync(BuildingBaseInfo buildingBaseInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(BuildingBaseInfo buildingBaseInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<BuildingBaseInfo> buildingBaseList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingBaseInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BuildingBaseInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(BuildingBaseInfo buildingBase, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<BuildingBaseInfo> buildingBaseList, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveAsync(SimpleUser user, BuildingBaseInfo buildingBaseInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> CheckDuplicateBuilding(string buildingId, string name, string city, string district, string area, CancellationToken cancellationToken = default(CancellationToken));

    }
}
