using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Dto.Request;
using XYHShopsPlugin.Dto.Response;
using XYHShopsPlugin.Models;
using XYHShopsPlugin.Stores;

namespace XYHShopsPlugin.Managers
{
    public class BuildingFavoriteManager
    {
        #region 成员

        protected IBuildingFavoritesStore _ibuildingFavoritesStore { get; }

        protected IBuildingsStore _ibuildingsStore { get; }

        protected IMapper _mapper { get; }

        #endregion

        public BuildingFavoriteManager(
            IBuildingFavoritesStore ibuildingFavoritesStore,
            IBuildingsStore ibuildingsStore,
            IMapper mapper)
        {
            _ibuildingFavoritesStore = ibuildingFavoritesStore ?? throw new ArgumentNullException(nameof(ibuildingFavoritesStore));

            _ibuildingsStore = ibuildingsStore ?? throw new ArgumentNullException(nameof(ibuildingsStore));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }



        /// <summary>
        /// 根据UserId、楼盘Id获取商铺收藏信息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="buildingid"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<BuildingFavoriteResponse> FindByUserIdAndBuildingIdAsync(string userid, string buildingid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _ibuildingFavoritesStore.GetAsync(a => a.Where(b => b.UserId == userid && b.BuildingId == buildingid && !b.IsDeleted), cancellationToken);
            return _mapper.Map<BuildingFavoriteResponse>(response);
        }

        /// <summary>
        /// 新增楼盘收藏信息
        /// </summary>
        /// <param name="user">创建者</param>
        /// <param name="buildingFavoriteRequest">请求实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<BuildingFavoriteResponse> CreateAsync(UserInfo user, BuildingFavoriteRequest buildingFavoriteRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingFavoriteRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingFavoriteRequest));
            }
            var buildingFavorite = _mapper.Map<BuildingFavorite>(buildingFavoriteRequest);

            buildingFavorite.Id = Guid.NewGuid().ToString();
            buildingFavorite.CreateUser = user.Id;
            buildingFavorite.CreateTime = DateTime.Now;
            buildingFavorite.UserId = user.Id;
            try
            {
                await _ibuildingFavoritesStore.CreateAsync(buildingFavorite, cancellationToken);
            }
            catch
            {
            }
            return _mapper.Map<BuildingFavoriteResponse>(buildingFavorite);
        }


        /// <summary>
        /// 根据Id获取楼盘收藏信息
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<BuildingFavoriteResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _ibuildingFavoritesStore.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<BuildingFavoriteResponse>(response);
        }

        /// <summary>
        /// 获取我的楼盘收藏信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="condition"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<BuildingFavoriteResponse>> FindMyBuildingFavoriteAsync(string userId, PageCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resulte = new PagingResponseMessage<BuildingFavoriteResponse>();

            var response = _ibuildingFavoritesStore.BuildingFavoriteAll().Where(b => b.UserId == userId && !b.IsDeleted&&!b.Buildings.IsDeleted);

            if (!string.IsNullOrEmpty(condition.KeyWord))
            {
                response = response.Where(x => x.Buildings.BuildingBaseInfo.Name.Contains(condition.KeyWord));
            }


            var query = await response.OrderByDescending(x => x.FavoriteTime).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);

            var ext = new List<BuildingFavoriteResponse>();

            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');

            var aasd = query.Select(a => new BuildingFavoriteResponse
            {
                Id = a.Id,
                BuildingId = a.BuildingId,
                UserId = a.UserId,
                FavoriteTime = a.FavoriteTime,
                UserNikeName = a.UserNikeName,
                BuildingSearchResponse = new BuildingSearchResponse
                {
                    Id = a.Buildings.Id,
                    Address = a.Buildings.BuildingBaseInfo.Address,
                    AreaFullName = a.Buildings.BuildingBaseInfo.CityDefine.Name + "-" + a.Buildings.BuildingBaseInfo.DistrictDefine.Name + "-" + a.Buildings.BuildingBaseInfo.AreaDefine.Name,
                    MaxPrice = a.Buildings.BuildingBaseInfo.MaxPrice,
                    MinPrice = a.Buildings.BuildingBaseInfo.MinPrice,
                    Name = a.Buildings.BuildingBaseInfo.Name,
                    Icon = string.IsNullOrEmpty(a.Buildings.Icon) ? "" : fr + "/" + a.Buildings.Icon.TrimStart('/'),
                    HasBus = a.Buildings.BuildingFacilities.HasBus,
                    HasRail = a.Buildings.BuildingFacilities.HasRail,
                    HasOtherTraffic = a.Buildings.BuildingFacilities.HasOtherTraffic,
                    HasKindergarten = a.Buildings.BuildingFacilities.HasKindergarten,
                    HasPrimarySchool = a.Buildings.BuildingFacilities.HasPrimarySchool,
                    HasMiddleSchool = a.Buildings.BuildingFacilities.HasMiddleSchool,
                    HasUniversity = a.Buildings.BuildingFacilities.HasUniversity,
                    HasMarket = a.Buildings.BuildingFacilities.HasMarket,
                    HasSupermarket = a.Buildings.BuildingFacilities.HasSupermarket,
                    HasBank = a.Buildings.BuildingFacilities.HasBank

                }
            });
            resulte.PageIndex = condition.PageIndex;
            resulte.PageSize = condition.PageSize;
            resulte.TotalCount = await response.CountAsync(cancellationToken);
            resulte.Extension = aasd.ToList();
            return resulte;
        }

        /// <summary>
        /// 修改单个楼盘收藏信息
        /// </summary>
        /// <param name="userId">请求者Id</param>
        /// <param name="buildingFavoriteRequest">请求数据</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(string userId, BuildingFavoriteRequest buildingFavoriteRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingFavoriteRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingFavoriteRequest));
            }
            var buildingFavorite = await _ibuildingFavoritesStore.GetAsync(a => a.Where(b => b.Id == buildingFavoriteRequest.Id && !b.IsDeleted));
            if (buildingFavorite == null)
            {
                return;
            }
            var newbuildingFavorite = _mapper.Map<BuildingFavorite>(buildingFavoriteRequest);

            newbuildingFavorite.IsDeleted = buildingFavorite.IsDeleted;
            newbuildingFavorite.CreateTime = buildingFavorite.CreateTime;
            newbuildingFavorite.CreateUser = buildingFavorite.CreateUser;
            newbuildingFavorite.UpdateTime = DateTime.Now;
            newbuildingFavorite.UpdateUser = userId;
            newbuildingFavorite.UserId = buildingFavorite.UserId;
            try
            {
                await _ibuildingFavoritesStore.UpdateAsync(newbuildingFavorite, cancellationToken);
            }
            catch { }
        }

        /// <summary>
        /// 删除楼盘收藏信息
        /// </summary>
        /// <param name="user">删除人基本信息</param>
        /// <param name="buildingId">删除楼盘收藏Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(UserInfo user, string buildingId, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await _ibuildingFavoritesStore.DeleteAsync(_mapper.Map<SimpleUser>(user), await _ibuildingFavoritesStore.GetAsync(x => x.Where(y => y.BuildingId == buildingId && y.UserId == user.Id && !y.IsDeleted)));
            }
            catch { }
        }

        /// <summary>
        /// 批量删除楼盘收藏信息
        /// </summary>
        /// <param name="userId">删除人Id</param>
        /// <param name="ids">删除Id数组</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task DeleteListAsync(string userId, List<string> ids, CancellationToken cancellationToken = default(CancellationToken))
        {
            var buildingFavorite = await _ibuildingFavoritesStore.ListAsync(a => a.Where(b => ids.Contains(b.Id) && !b.IsDeleted), cancellationToken);
            if (buildingFavorite == null || buildingFavorite.Count == 0)
            {
                return;
            }
            for (int i = 0; i < buildingFavorite.Count; i++)
            {
                buildingFavorite[i].DeleteUser = userId;
                buildingFavorite[i].DeleteTime = DateTime.Now;
                buildingFavorite[i].IsDeleted = true;
            }
            try
            {
                await _ibuildingFavoritesStore.UpdateListAsync(buildingFavorite, cancellationToken);
            }
            catch { }
        }
    }
}
