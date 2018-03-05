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
    public class BuildingRecommendStore : IBuildingRecommendStore
    {
        //Db
        protected ShopsDbContext Context { get; }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="shopsDbContext">Context</param>
        public BuildingRecommendStore(ShopsDbContext shopsDbContext)
        {
            Context = shopsDbContext;
        }

        //获取所有楼盘推荐信息
        public IQueryable<BuildingRecommend> BuildingRecommendAll()
        {
            var result = from bf in Context.BuildingRecommends.AsNoTracking()
                         join user in Context.Users.AsNoTracking() on bf.RecommendUserId equals user.Id into u
                         from res in u.DefaultIfEmpty()
                         join organ1 in Context.Organizations.AsNoTracking() on bf.MainAreaId equals organ1.Id into o
                         from organ in o.DefaultIfEmpty()

                         join b in Context.Buildings.AsNoTracking() on bf.BuildingId equals b.Id into buildings
                         from bd in buildings.DefaultIfEmpty()
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

                         select new BuildingRecommend()
                         {
                             Id = bf.Id,
                             BuildingId = bf.BuildingId,
                             RecommendUserId = bf.RecommendUserId,
                             RecommendTime = bf.RecommendTime,
                             RecommendDays = bf.RecommendDays,
                             UserNikeName = res.TrueName,
                             Order = bf.Order,
                             IsDeleted = bf.IsDeleted,
                             IsOutDate = bf.IsOutDate,
                             IsRegion = bf.IsRegion,
                             MainAreaId = bf.MainAreaId,
                             CreateTime = bf.CreateTime,
                             MainAreaName = organ.OrganizationName,
                             Buildings = new Buildings()
                             {
                                 Id = bd.Id,
                                 IsDeleted = bd.IsDeleted,
                                 Icon = bd.Icon,
                                 OrganizationId = bd.OrganizationId,
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
                                     },
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
        /// 根据某一成员获取一条楼盘推荐信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingRecommend>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BuildingRecommends.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 根据某一成员获取列表楼盘推荐信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BuildingRecommend>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BuildingRecommends.AsNoTracking()).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 新增楼盘推荐信息
        /// </summary>
        /// <param name="buildingRecommend">实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<BuildingRecommend> CreateAsync(BuildingRecommend buildingRecommend, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingRecommend == null)
            {
                throw new ArgumentNullException(nameof(buildingRecommend));
            }
            Context.Add(buildingRecommend);
            await Context.SaveChangesAsync(cancellationToken);
            return buildingRecommend;
        }

        /// <summary>
        /// 删除楼盘推荐
        /// </summary>
        /// <param name="user">登陆用户基本信息</param>
        /// <param name="buildingRecommend">楼盘推荐实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteAsync(SimpleUser user, BuildingRecommend buildingRecommend, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (buildingRecommend == null)
            {
                throw new ArgumentNullException(nameof(buildingRecommend));
            }

            buildingRecommend.DeleteTime = DateTime.Now;
            buildingRecommend.DeleteUser = user.Id;
            buildingRecommend.IsDeleted = true;
            Context.Attach(buildingRecommend);
            var entry = Context.Entry(buildingRecommend);
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
        /// <param name="buildingRecommendList">集合</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteListAsync(List<BuildingRecommend> buildingRecommendList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingRecommendList == null)
            {
                throw new ArgumentNullException(nameof(buildingRecommendList));
            }
            Context.RemoveRange(buildingRecommendList);
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
        /// 修改楼盘推荐信息
        /// </summary>
        /// <param name="buildingRecommend"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateAsync(BuildingRecommend buildingRecommend, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingRecommend == null)
            {
                throw new ArgumentNullException(nameof(buildingRecommend));
            }
            Context.Attach(buildingRecommend);
            Context.Update(buildingRecommend);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        /// <summary>
        /// 批量修改楼盘推荐信息(一般修改删除状态)
        /// </summary>
        /// <param name="buildingRecommend"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateListAsync(List<BuildingRecommend> buildingRecommend, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingRecommend == null)
            {
                throw new ArgumentNullException(nameof(buildingRecommend));
            }
            Context.AttachRange(buildingRecommend);
            Context.UpdateRange(buildingRecommend);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException e) { }
        }

    }
}
