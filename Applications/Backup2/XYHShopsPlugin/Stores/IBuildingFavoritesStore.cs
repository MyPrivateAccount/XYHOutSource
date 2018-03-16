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
    public interface IBuildingFavoritesStore
    {
        IQueryable<BuildingFavorite> BuildingFavoriteAll();

        Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingFavorite>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BuildingFavorite>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<BuildingFavorite> CreateAsync(BuildingFavorite buildingfavorites, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(SimpleUser user, BuildingFavorite buildingfavorites, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<BuildingFavorite> buildingfavoritesList, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(BuildingFavorite buildingfavorites, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<BuildingFavorite> buildingfavorites, CancellationToken cancellationToken = default(CancellationToken));
    }
}
