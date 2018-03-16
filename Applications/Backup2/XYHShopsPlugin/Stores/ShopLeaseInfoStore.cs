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
    public class ShopLeaseInfoStore : IShopLeaseInfoStore
    {
        public ShopLeaseInfoStore(ShopsDbContext baseDataDbContext)
        {
            Context = baseDataDbContext;
            ShopLeaseInfos = Context.ShopLeaseInfos;
        }

        protected ShopsDbContext Context { get; }

        public IQueryable<ShopLeaseInfo> ShopLeaseInfos { get; set; }


        public async Task<ShopLeaseInfo> CreateAsync(ShopLeaseInfo shopLeaseInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopLeaseInfo == null)
            {
                throw new ArgumentNullException(nameof(shopLeaseInfo));
            }
            Context.Add(shopLeaseInfo);
            await Context.SaveChangesAsync(cancellationToken);
            return shopLeaseInfo;
        }


        public async Task DeleteAsync(ShopLeaseInfo shopLeaseInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopLeaseInfo == null)
            {
                throw new ArgumentNullException(nameof(shopLeaseInfo));
            }
            Context.Remove(shopLeaseInfo);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<ShopLeaseInfo> shopLeaseInfoList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopLeaseInfoList == null)
            {
                throw new ArgumentNullException(nameof(shopLeaseInfoList));
            }
            Context.RemoveRange(shopLeaseInfoList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<ShopLeaseInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ShopLeaseInfos).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<ShopLeaseInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ShopLeaseInfos).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(ShopLeaseInfo shopLeaseInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopLeaseInfo == null)
            {
                throw new ArgumentNullException(nameof(shopLeaseInfo));
            }
            Context.Attach(shopLeaseInfo);
            Context.Update(shopLeaseInfo);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task UpdateListAsync(List<ShopLeaseInfo> shopLeaseInfoList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopLeaseInfoList == null)
            {
                throw new ArgumentNullException(nameof(shopLeaseInfoList));
            }
            Context.AttachRange(shopLeaseInfoList);
            Context.UpdateRange(shopLeaseInfoList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task SaveAsync(SimpleUser user, string buildingId, ShopLeaseInfo shopLeaseInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (shopLeaseInfo == null)
            {
                throw new ArgumentNullException(nameof(shopLeaseInfo));
            }

            //查看楼盘是否存在
            if (!Context.Shops.Any(x => x.Id == shopLeaseInfo.Id))
            {
                Shops shops = new Shops()
                {
                    Id = shopLeaseInfo.Id,
                    BuildingId = buildingId,
                    CreateUser = user.Id,
                    CreateTime = DateTime.Now,
                    OrganizationId = user.OrganizationId,
                    ExamineStatus = 0
                };


                Context.Add(shops);
            }
            //基本信息
            if (!Context.ShopLeaseInfos.Any(x => x.Id == shopLeaseInfo.Id))
            {
                Context.Add(shopLeaseInfo);
            }
            else
            {
                Context.Attach(shopLeaseInfo);
                Context.Update(shopLeaseInfo);
            }
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }
    }
}
