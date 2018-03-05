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
    public interface IShopsFavoriteStore
    {
        IQueryable<ShopsFavorite> ShopsFavoriteAll();

        Task<TResult> GetAsync<TResult>(Func<IQueryable<ShopsFavorite>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<ShopsFavorite>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<ShopsFavorite> CreateAsync(ShopsFavorite shopsfavorite, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(SimpleUser user, ShopsFavorite shopsfavorite, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<ShopsFavorite> shopsfavoriteList, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(ShopsFavorite shopsfavorite, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<ShopsFavorite> shopsfavorite, CancellationToken cancellationToken = default(CancellationToken));
    }
}
