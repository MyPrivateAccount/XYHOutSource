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
    public class ShopBaseInfoStore : IShopBaseInfoStore
    {
        public ShopBaseInfoStore(ShopsDbContext baseDataDbContext)
        {
            Context = baseDataDbContext;
            ShopBaseInfos = Context.ShopBaseInfos;
        }

        protected ShopsDbContext Context { get; }

        public IQueryable<ShopBaseInfo> ShopBaseInfos { get; set; }

        public IQueryable<ShopBaseInfo> GetSimpleShopBase()
        {
            var query = from shop in Context.Shops.AsNoTracking()
                        join b1 in Context.ShopBaseInfos.AsNoTracking() on shop.Id equals b1.Id into b2
                        from b in b2.DefaultIfEmpty()

                        select new ShopBaseInfo
                        {
                            BuildingArea = b.BuildingArea,
                            BuildingId = shop.BuildingId,
                            BuildingNo = b.BuildingNo,
                            Depth = b.Depth,
                            FloorNo = b.FloorNo,
                            Floors = b.Floors,
                            FreeArea = b.FreeArea,
                            GuidingPrice = b.GuidingPrice,
                            HasFree = b.HasFree,
                            HasStreet = b.HasStreet,
                            Height = b.Height,
                            Id = b.Id,
                            HouseArea = b.HouseArea,
                            IsCorner = b.IsCorner,
                            IsFaceStreet = b.IsFaceStreet,
                            IsHot = b.IsHot,
                            Name = b.Name,
                            Number = b.Number,
                            OutsideArea = b.OutsideArea,
                            Price = b.Price,
                            SaleStatus = b.SaleStatus,
                            ShopCategory = b.ShopCategory,
                            Status = b.Status,
                            StreetDistance = b.StreetDistance,
                            TotalPrice = b.TotalPrice,
                            Toward = b.Toward,
                            Width = b.Width


                        };

            return query;
        }

        public async Task<ShopBaseInfo> CreateAsync(ShopBaseInfo shopBaseInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopBaseInfo == null)
            {
                throw new ArgumentNullException(nameof(shopBaseInfo));
            }
            if (shopBaseInfo.TotalPrice != null)
            {
                shopBaseInfo.Price = shopBaseInfo.TotalPrice / Convert.ToDecimal(shopBaseInfo.BuildingArea);
            }
            Context.Add(shopBaseInfo);

            await Context.SaveChangesAsync(cancellationToken);
            return shopBaseInfo;
        }



        public async Task DeleteAsync(ShopBaseInfo shopBaseInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopBaseInfo == null)
            {
                throw new ArgumentNullException(nameof(shopBaseInfo));
            }
            Context.Remove(shopBaseInfo);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<ShopBaseInfo> shopBaseInfoList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopBaseInfoList == null)
            {
                throw new ArgumentNullException(nameof(shopBaseInfoList));
            }
            Context.RemoveRange(shopBaseInfoList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<ShopBaseInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ShopBaseInfos).SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> CheckDuplicateShop(string buildingId, string shopId, string buildingNo, string floorNo, string number, CancellationToken cancellationToken = default(CancellationToken))
        {
            var q = from shop in Context.Shops.AsNoTracking()
                    join b in Context.ShopBaseInfos.AsNoTracking() on shop.Id equals b.Id
                    where shop.BuildingId == buildingId && b.Id != shopId && b.FloorNo == floorNo && b.Number == number && b.BuildingNo == buildingNo && shop.IsDeleted == false
                    select b;
            int count = await q.CountAsync();

            return count > 0;

        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<ShopBaseInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ShopBaseInfos).ToListAsync(cancellationToken);
        }

        public Task<List<TResult>> ListSellHistoryAsync<TResult>(Func<IQueryable<SellHistory>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.SellHistorys.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public async Task UpdateSaleStatusAsync(List<ShopBaseInfo> shopBaseInfos, List<SellHistory> sellHistories, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopBaseInfos == null)
            {
                throw new ArgumentNullException(nameof(shopBaseInfos));
            }
            Context.AttachRange(shopBaseInfos);
            Context.UpdateRange(shopBaseInfos);
            if (sellHistories != null)
            {
                Context.AddRange(sellHistories);
            }
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task UpdateAsync(ShopBaseInfo shopBaseInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopBaseInfo == null)
            {
                throw new ArgumentNullException(nameof(shopBaseInfo));
            }
            if (shopBaseInfo.TotalPrice != null)
            {
                shopBaseInfo.Price = shopBaseInfo.TotalPrice / Convert.ToDecimal(shopBaseInfo.BuildingArea);
            }

            Context.Attach(shopBaseInfo);
            Context.Update(shopBaseInfo);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task UpdateListAsync(List<ShopBaseInfo> shopBaseInfoList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopBaseInfoList == null)
            {
                throw new ArgumentNullException(nameof(shopBaseInfoList));
            }
            Context.AttachRange(shopBaseInfoList);
            Context.UpdateRange(shopBaseInfoList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task SaveAsync(SimpleUser user, string buildingId, ShopBaseInfo shopBaseInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (shopBaseInfo == null)
            {
                throw new ArgumentNullException(nameof(shopBaseInfo));
            }

            //查看楼盘是否存在
            if (!Context.Shops.Any(x => x.Id == shopBaseInfo.Id))
            {
                Shops shops = new Shops()
                {
                    Id = shopBaseInfo.Id,
                    BuildingId = buildingId,
                    CreateUser = user.Id,
                    CreateTime = DateTime.Now,
                    OrganizationId = user.OrganizationId,
                    ExamineStatus = 0
                };


                Context.Add(shops);
            }

            if (shopBaseInfo.TotalPrice != null)
            {
                shopBaseInfo.Price = shopBaseInfo.TotalPrice / Convert.ToDecimal(shopBaseInfo.BuildingArea);
            }
            //基本信息
            if (!Context.ShopBaseInfos.Any(x => x.Id == shopBaseInfo.Id))
            {
                Context.Add(shopBaseInfo);
            }
            else
            {
                Context.Attach(shopBaseInfo);
                Context.Update(shopBaseInfo);
            }
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }
    }
}
