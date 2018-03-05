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
    public class BuildingFavoriteStore : IBuildingFavoritesStore
    {
        //Db
        protected ShopsDbContext Context { get; }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="shopsDbContext">Context</param>
        public BuildingFavoriteStore(ShopsDbContext shopsDbContext)
        {
            Context = shopsDbContext;
        }

        //获取所有楼盘收藏信息
        public IQueryable<BuildingFavorite> BuildingFavoriteAll()
        {
            var result = from bf in Context.BuildingFavorites.AsNoTracking()
                         join user in Context.Users.AsNoTracking() on bf.UserId equals user.Id into bfa
                         from res in bfa.DefaultIfEmpty()

                         join bd in Context.Buildings.AsNoTracking() on bf.BuildingId equals bd.Id
                         join basic1 in Context.BuildingBaseInfos.AsNoTracking() on bd.Id equals basic1.Id into basic2
                         from basic in basic2.DefaultIfEmpty()
                         join f1 in Context.BuildingFacilities.AsNoTracking() on bd.Id equals f1.Id into f2
                         from f in f2.DefaultIfEmpty()
                         join s1 in Context.BuildingShopInfos.AsNoTracking() on bd.Id equals s1.Id into s2
                         from s in s2.DefaultIfEmpty()

                             //区域
                         join c1 in Context.AreaDefines.AsNoTracking() on basic.City equals c1.Code into c2
                         from city in c2.DefaultIfEmpty()
                         join d1 in Context.AreaDefines.AsNoTracking() on basic.District equals d1.Code into d2
                         from district in d2.DefaultIfEmpty()
                         join a1 in Context.AreaDefines.AsNoTracking() on basic.Area equals a1.Code into a2
                         from area in a2.DefaultIfEmpty()

                         select new BuildingFavorite()
                         {
                             Id = bf.Id,
                             BuildingId = bf.BuildingId,
                             UserId = bf.UserId,
                             FavoriteTime = bf.FavoriteTime,
                             UserNikeName = res.TrueName,
                             IsDeleted = bf.IsDeleted,
                             Buildings = new Buildings()
                             {
                                 Id = bd.Id,
                                 Icon = bd.Icon,
                                 IsDeleted=bd.IsDeleted,
                                 BuildingBaseInfo = new BuildingBaseInfo()
                                 {
                                     Id = basic.Id,
                                     Name = basic.Name,
                                     Address = basic.Address,
                                     City = basic.City,
                                     District = basic.District,
                                     Area = basic.Area,
                                     MaxPrice = basic.MaxPrice,
                                     MinPrice = basic.MinPrice,
                                     FloorSurface = basic.FloorSurface,
                                     BuiltupArea = basic.BuiltupArea,
                                     Developer = basic.Developer,
                                     BasementParkingSpace = basic.BasementParkingSpace,
                                     BuildingNum = basic.BuildingNum,
                                     DeliveryDate = basic.DeliveryDate,
                                     LandExpireDate = basic.LandExpireDate,
                                     OpenDate = basic.OpenDate,
                                     GreeningRate = basic.GreeningRate,
                                     HouseHolds = basic.HouseHolds,
                                     PlotRatio = basic.PlotRatio,
                                     ParkingSpace = basic.ParkingSpace,
                                     PMC = basic.PMC,
                                     PMF = basic.PMF,
                                     PropertyTerm = basic.PropertyTerm,
                                     Shops = basic.Shops,
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
                                 },
                                 BuildingFacilities = new BuildingFacilities()
                                 {
                                     HasBus = f.HasBus,
                                     HasRail = f.HasRail,
                                     HasOther = f.HasOther,
                                     HasOtherTraffic = f.HasOtherTraffic,
                                     HasKindergarten = f.HasKindergarten,
                                     HasPrimarySchool = f.HasPrimarySchool,
                                     HasMarket = f.HasMarket,
                                     HasMiddleSchool = f.HasMiddleSchool,
                                     HasUniversity = f.HasUniversity,
                                     HasBank = f.HasBank,
                                     HasHospital = f.HasHospital,
                                     HasSupermarket = f.HasSupermarket,
                                     BankDesc = f.BankDesc,
                                     BusDesc = f.BusDesc,
                                     HospitalDesc = f.HospitalDesc,
                                     KindergartenDesc = f.KindergartenDesc,
                                     Id = f.Id,
                                     MarketDesc = f.MarketDesc,
                                     MiddleSchoolDesc = f.MiddleSchoolDesc,
                                     OtherDesc = f.OtherDesc,
                                     OtherTrafficDesc = f.OtherTrafficDesc,
                                     PrimarySchoolDesc = f.PrimarySchoolDesc,
                                     RailDesc = f.RailDesc,
                                     SupermarketDesc = f.SupermarketDesc,
                                     UniversityDesc = f.UniversityDesc
                                 }
                             }
                         };




            return result;
        }

        /// <summary>
        /// 根据某一成员获取一条楼盘收藏信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingFavorite>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BuildingFavorites.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 根据某一成员获取列表楼盘收藏信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BuildingFavorite>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BuildingFavorites.AsNoTracking()).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 新增楼盘收藏信息
        /// </summary>
        /// <param name="buildingfavorites">实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<BuildingFavorite> CreateAsync(BuildingFavorite buildingfavorites, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingfavorites == null)
            {
                throw new ArgumentNullException(nameof(buildingfavorites));
            }
            Context.Add(buildingfavorites);
            await Context.SaveChangesAsync(cancellationToken);
            return buildingfavorites;
        }

        /// <summary>
        /// 删除楼盘收藏
        /// </summary>
        /// <param name="user">登陆用户基本信息</param>
        /// <param name="buildingfavorites">楼盘收藏实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteAsync(SimpleUser user, BuildingFavorite buildingfavorites, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (buildingfavorites == null)
            {
                throw new ArgumentNullException(nameof(buildingfavorites));
            }
            //删除基本信息
            buildingfavorites.DeleteTime = DateTime.Now;
            buildingfavorites.DeleteUser = user.Id;
            buildingfavorites.IsDeleted = true;
            Context.Attach(buildingfavorites);
            var entry = Context.Entry(buildingfavorites);
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
        /// <param name="buildingfavoritesList">集合</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteListAsync(List<BuildingFavorite> buildingfavoritesList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingfavoritesList == null)
            {
                throw new ArgumentNullException(nameof(buildingfavoritesList));
            }
            Context.RemoveRange(buildingfavoritesList);
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
        /// 修改楼盘收藏信息
        /// </summary>
        /// <param name="buildingfavorites"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateAsync(BuildingFavorite buildingfavorites, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingfavorites == null)
            {
                throw new ArgumentNullException(nameof(buildingfavorites));
            }
            Context.Attach(buildingfavorites);
            Context.Update(buildingfavorites);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        /// <summary>
        /// 批量修改楼盘收藏信息(一般修改删除状态)
        /// </summary>
        /// <param name="buildingfavorites"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateListAsync(List<BuildingFavorite> buildingfavorites, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingfavorites == null)
            {
                throw new ArgumentNullException(nameof(buildingfavorites));
            }
            Context.AttachRange(buildingfavorites);
            Context.UpdateRange(buildingfavorites);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }
    }
}
