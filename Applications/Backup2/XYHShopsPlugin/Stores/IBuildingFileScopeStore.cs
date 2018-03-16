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
    public interface IBuildingFileScopeStore
    {
        IQueryable<BuildingFileScope> BuildingFileScopes { get; set; }

        Task<BuildingFileScope> CreateAsync(BuildingFileScope buildingFileScope, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(BuildingFileScope buildingFileScope, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<BuildingFileScope> buildingFileScopeList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BuildingFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(BuildingFileScope buildingFileScope, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<BuildingFileScope> buildingFileScopeList, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveAsync(SimpleUser user, string buildingId, List<BuildingFileScope> shopsFileScopeList, CancellationToken cancellationToken = default(CancellationToken));
    }
}
