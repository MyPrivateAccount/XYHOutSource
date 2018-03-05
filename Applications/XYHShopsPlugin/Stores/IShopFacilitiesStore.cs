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
    public interface IShopFacilitiesStore
    {
        IQueryable<ShopFacilities> ShopFacilities { get; set; }

        Task<ShopFacilities> CreateAsync(ShopFacilities shopFacilities, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(ShopFacilities shopFacilities, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<ShopFacilities> shopFacilitiesList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<ShopFacilities>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<ShopFacilities>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(ShopFacilities shopFacilities, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<ShopFacilities> shopFacilitiesList, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveAsync(SimpleUser user, string buildingId,  ShopFacilities shopFacilities, CancellationToken cancellationToken = default(CancellationToken));
    }
}
