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
    public interface IBuildingNoStore
    {
        IQueryable<BuildingNo> BuildingNoAll();

        Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingNo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BuildingNo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<BuildingNo> CreateAsync(BuildingNo buildingnos, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<BuildingNo>> CreateListAsync(List<BuildingNo> buildingNos, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(SimpleUser user, BuildingNo buildingnos, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<BuildingNo> buildingnosList, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(BuildingNo buildingnos, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<BuildingNo> buildingnos, CancellationToken cancellationToken = default(CancellationToken));


    }
}
