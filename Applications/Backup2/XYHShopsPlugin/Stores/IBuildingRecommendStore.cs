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
    public interface IBuildingRecommendStore
    {
        IQueryable<BuildingRecommend> BuildingRecommendAll();

        Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingRecommend>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BuildingRecommend>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<BuildingRecommend> CreateAsync(BuildingRecommend buildingrecommend, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(SimpleUser user, BuildingRecommend buildingrecommend, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<BuildingRecommend> buildingrecommendList, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(BuildingRecommend buildingrecommend, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<BuildingRecommend> buildingrecommend, CancellationToken cancellationToken = default(CancellationToken));
    }
}
