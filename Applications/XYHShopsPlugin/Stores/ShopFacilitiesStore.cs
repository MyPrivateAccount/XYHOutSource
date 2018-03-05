using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Models;
using ApplicationCore.Models;

namespace XYHShopsPlugin.Stores
{
    public class ShopFacilitiesStore : IShopFacilitiesStore
    {
        public ShopFacilitiesStore(ShopsDbContext shopsDbContext)
        {
            Context = shopsDbContext;
            ShopFacilities = Context.ShopFacilities;
        }

        protected ShopsDbContext Context { get; }

        public IQueryable<ShopFacilities> ShopFacilities { get; set; }


        public async Task<ShopFacilities> CreateAsync(ShopFacilities shopFacilities, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopFacilities == null)
            {
                throw new ArgumentNullException(nameof(shopFacilities));
            }
            Context.Add(shopFacilities);
            await Context.SaveChangesAsync(cancellationToken);
            return shopFacilities;
        }


        public async Task DeleteAsync(ShopFacilities shopFacilities, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopFacilities == null)
            {
                throw new ArgumentNullException(nameof(shopFacilities));
            }
            Context.Remove(shopFacilities);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<ShopFacilities> shopFacilitiesList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopFacilitiesList == null)
            {
                throw new ArgumentNullException(nameof(shopFacilitiesList));
            }
            Context.RemoveRange(shopFacilitiesList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<ShopFacilities>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ShopFacilities).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<ShopFacilities>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ShopFacilities).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(ShopFacilities shopFacilities, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopFacilities == null)
            {
                throw new ArgumentNullException(nameof(shopFacilities));
            }
            Context.Attach(shopFacilities);
            Context.Update(shopFacilities);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task UpdateListAsync(List<ShopFacilities> shopFacilitiesList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopFacilitiesList == null)
            {
                throw new ArgumentNullException(nameof(shopFacilitiesList));
            }
            Context.AttachRange(shopFacilitiesList);
            Context.UpdateRange(shopFacilitiesList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task SaveAsync(SimpleUser user, string buildingId, ShopFacilities shopFacilities, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (shopFacilities == null)
            {
                throw new ArgumentNullException(nameof(shopFacilities));
            }

            //查看楼盘是否存在
            if (!Context.Shops.Any(x => x.Id == shopFacilities.Id))
            {
                Shops shops = new Shops()
                {
                    Id = shopFacilities.Id,
                    BuildingId = buildingId,
                    CreateUser = user.Id,
                    CreateTime = DateTime.Now,
                    OrganizationId = user.OrganizationId,
                    ExamineStatus = 0
                };


                Context.Add(shops);
            }
            //基本信息
            if (!Context.ShopFacilities.Any(x => x.Id == shopFacilities.Id))
            {
                Context.Add(shopFacilities);
            }
            else
            {
                Context.Attach(shopFacilities);
                Context.Update(shopFacilities);
            }
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }
    }
}
