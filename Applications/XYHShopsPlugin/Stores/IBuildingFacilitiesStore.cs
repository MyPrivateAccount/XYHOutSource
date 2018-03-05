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
    public interface IBuildingFacilitiesStore
    {
        IQueryable<BuildingFacilities> BuildingFacilities { get; set; }

        Task<BuildingFacilities> CreateAsync(BuildingFacilities buildingFacilities, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(BuildingFacilities buildingFacilities, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<BuildingFacilities> buildingFacilitiesList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingFacilities>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BuildingFacilities>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(BuildingFacilities buildingFacilities, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<BuildingFacilities> buildingFacilitiesList, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveAsync(SimpleUser user, BuildingFacilities buildingFacilities, CancellationToken cancellationToken = default(CancellationToken));

    }
}
