using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Stores
{
    public class ShopsFavoriteStore : IShopsFavoriteStore
    {
        //Db
        protected ShopsDbContext Context { get; }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="shopsDbContext">Context</param>
        public ShopsFavoriteStore(ShopsDbContext shopsDbContext)
        {
            Context = shopsDbContext;
        }

        //获取所有商铺收藏信息
        public IQueryable<ShopsFavorite> ShopsFavoriteAll()
        {
            var result = from bf in Context.ShopsFavorites.AsNoTracking()
                         join user in Context.Users.AsNoTracking() on bf.UserId equals user.Id into sf
                         from res in sf.DefaultIfEmpty()

                         join s in Context.Shops.AsNoTracking() on bf.ShopsId equals s.Id
                         join b1 in Context.ShopBaseInfos.AsNoTracking() on bf.ShopsId equals b1.Id into b2
                         from b in b2.DefaultIfEmpty()
                         join f1 in Context.ShopFacilities.AsNoTracking() on bf.ShopsId equals f1.Id into f2
                         from f in f2.DefaultIfEmpty()
                         join l1 in Context.ShopLeaseInfos.AsNoTracking() on bf.ShopsId equals l1.Id into l2
                         from l in l2.DefaultIfEmpty()

                             //楼盘
                         join bu1 in Context.Buildings.AsNoTracking() on s.BuildingId equals bu1.Id into bu2
                         from bu in bu2.DefaultIfEmpty()
                         join bb1 in Context.BuildingBaseInfos.AsNoTracking() on bu.Id equals bb1.Id into bb2
                         from bb in bb2.DefaultIfEmpty()

                         join c1 in Context.AreaDefines.AsNoTracking() on bb.City equals c1.Code into c2
                         from city in c2.DefaultIfEmpty()
                         join d1 in Context.AreaDefines.AsNoTracking() on bb.District equals d1.Code into d2
                         from district in d2.DefaultIfEmpty()
                         join a1 in Context.AreaDefines.AsNoTracking() on bb.Area equals a1.Code into a2
                         from area in a2.DefaultIfEmpty()
                         select new ShopsFavorite()
                         {
                             Id = bf.Id,
                             ShopsId = bf.ShopsId,
                             UserId = bf.UserId,
                             FavoriteTime = bf.FavoriteTime,
                             UserNikeName = res.TrueName,
                             IsDeleted = bf.IsDeleted,
                             Shops = new Shops
                             {

                                 BuildingId = s.BuildingId,
                                 Id = s.Id,
                                 CreateTime = s.CreateTime,
                                 CreateUser = s.CreateUser,
                                 ExamineStatus = s.ExamineStatus,
                                 IsDeleted = s.IsDeleted,
                                 OrganizationId = s.OrganizationId,
                                 Icon = s.Icon,
                                 ShopBaseInfo = new ShopBaseInfo()
                                 {
                                     Name = b.Name,
                                     BuildingArea = b.BuildingArea,
                                     SaleStatus = b.SaleStatus,
                                     BuildingNo = b.BuildingNo,
                                     FloorNo = b.FloorNo,
                                     Floors = b.Floors,
                                     Number = b.Number,
                                     Price = b.Price,
                                     TotalPrice = b.TotalPrice,
                                     HouseArea = b.HouseArea,
                                     OutsideArea = b.OutsideArea,
                                     BuildingId = b.BuildingId,
                                     Depth = b.Depth,
                                     HasFree = b.HasFree,
                                     HasStreet = b.HasStreet,
                                     Height = b.Height,
                                     FreeArea = b.FreeArea,
                                     Id = b.Id,
                                     IsCorner = b.IsCorner,
                                     IsFaceStreet = b.IsFaceStreet,
                                     ShopCategory = b.ShopCategory,
                                     Status = b.Status,
                                     StreetDistance = b.StreetDistance,
                                     Toward = b.Toward,
                                     Width = b.Width
                                 },
                                 ShopFacilities = new ShopFacilities()
                                 {
                                     Id = f.Id,
                                     Blowoff = f.Blowoff,
                                     DownWater = f.DownWater,
                                     Capacitance = f.Capacitance,
                                     Chimney = f.Chimney,
                                     Elevator = f.Elevator,
                                     Gas = f.Gas,
                                     OpenFloor = f.OpenFloor,
                                     Outside = f.Outside,
                                     ParkingSpace = f.ParkingSpace,
                                     Split = f.Split,
                                     Staircase = f.Staircase,
                                     UpperWater = f.UpperWater,
                                     Voltage = f.Voltage
                                 },
                                 ShopLeaseInfo = new ShopLeaseInfo()
                                 {
                                     HasLease = l.HasLease,
                                     BackMonth = l.BackMonth,
                                     BackRate = l.BackRate,
                                     CurrentOperation = l.CurrentOperation,
                                     Deposit = l.Deposit,
                                     EndDate = l.EndDate,
                                     HasLeaseback = l.HasLeaseback,
                                     Id = l.Id,
                                     Memo = l.Memo,
                                     StartDate = l.StartDate,
                                     PaymentTime = l.PaymentTime,
                                     Rental = l.Rental,
                                     Upscale = l.Upscale,
                                     DepositType = l.DepositType,
                                     UpscaleInterval = l.UpscaleInterval,
                                     UpscaleStartYear = l.UpscaleStartYear
                                 },
                                 Buildings = new Buildings()
                                 {
                                     BuildingBaseInfo = new BuildingBaseInfo()
                                     {
                                         Name = bb.Name,
                                         Id = bu.Id,
                                         Address = bb.Address,
                                         AreaDefine = new AreaDefine()
                                         {
                                             Code = area.Code,
                                             Name = area.Name
                                         },
                                         DistrictDefine = new AreaDefine()
                                         {
                                             Code = district.Code,
                                             Name = district.Name
                                         },
                                         CityDefine = new AreaDefine()
                                         {
                                             Code = city.Code,
                                             Name = city.Name
                                         }
                                     }
                                 }
                             }
                         };

            return result;
        }

        /// <summary>
        /// 根据某一成员获取一条商铺收藏信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<ShopsFavorite>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ShopsFavorites.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 根据某一成员获取列表商铺收藏信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<ShopsFavorite>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ShopsFavorites.AsNoTracking()).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 新增商铺收藏信息
        /// </summary>
        /// <param name="shopsFavorite">实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<ShopsFavorite> CreateAsync(ShopsFavorite shopsFavorite, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopsFavorite == null)
            {
                throw new ArgumentNullException(nameof(shopsFavorite));
            }
            Context.Add(shopsFavorite);
            await Context.SaveChangesAsync(cancellationToken);
            return shopsFavorite;
        }

        /// <summary>
        /// 删除商铺收藏
        /// </summary>
        /// <param name="user">登陆用户基本信息</param>
        /// <param name="shopsFavorite">商铺收藏实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteAsync(SimpleUser user, ShopsFavorite shopsFavorite, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (shopsFavorite == null)
            {
                throw new ArgumentNullException(nameof(shopsFavorite));
            }
            //删除基本信息
            shopsFavorite.DeleteTime = DateTime.Now;
            shopsFavorite.DeleteUser = user.Id;
            shopsFavorite.IsDeleted = true;
            Context.Attach(shopsFavorite);
            var entry = Context.Entry(shopsFavorite);
            entry.Property(x => x.IsDeleted).IsModified = true;
            entry.Property(x => x.DeleteUser).IsModified = true;
            entry.Property(x => x.DeleteTime).IsModified = true;
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        /// <summary>
        /// 从数据库中删除List
        /// </summary>
        /// <param name="shopsFavoriteList">集合</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteListAsync(List<ShopsFavorite> shopsFavoriteList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopsFavoriteList == null)
            {
                throw new ArgumentNullException(nameof(shopsFavoriteList));
            }
            Context.RemoveRange(shopsFavoriteList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        /// <summary>
        /// 修改商铺收藏信息
        /// </summary>
        /// <param name="shopsFavorite"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateAsync(ShopsFavorite shopsFavorite, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopsFavorite == null)
            {
                throw new ArgumentNullException(nameof(shopsFavorite));
            }
            Context.Attach(shopsFavorite);
            Context.Update(shopsFavorite);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        /// <summary>
        /// 批量修改商铺收藏信息(一般修改删除状态)
        /// </summary>
        /// <param name="shopsFavorite"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateListAsync(List<ShopsFavorite> shopsFavorite, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopsFavorite == null)
            {
                throw new ArgumentNullException(nameof(shopsFavorite));
            }
            Context.AttachRange(shopsFavorite);
            Context.UpdateRange(shopsFavorite);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }
    }
}
