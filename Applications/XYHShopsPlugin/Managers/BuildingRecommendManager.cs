using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Managers;
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
    public class BuildingRecommendManager
    {
        #region 成员

        protected IBuildingRecommendStore _ibuildingRecommendsStore { get; }

        protected IBuildingsStore _ibuildingStore { get; }

        protected IOrganizationExpansionStore _iorganizationExpansionStore { get; }

        protected PermissionExpansionManager _permissionExpansionManager { get; }

        protected IMapper _mapper { get; }

        #endregion

        public BuildingRecommendManager(
            IBuildingRecommendStore ibuildingRecommendsStore,
            PermissionExpansionManager permissionExpansionManager,
            IBuildingsStore ibuildingStore,
            IOrganizationExpansionStore organizationExpansionStore,
            IMapper mapper)
        {
            _ibuildingRecommendsStore = ibuildingRecommendsStore ?? throw new ArgumentNullException(nameof(ibuildingRecommendsStore));
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
            _ibuildingStore = ibuildingStore ?? throw new ArgumentNullException(nameof(ibuildingStore));
            _iorganizationExpansionStore = organizationExpansionStore ?? throw new ArgumentNullException(nameof(organizationExpansionStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 新增楼盘收藏信息
        /// </summary>
        /// <param name="user">创建者</param>
        /// <param name="buildingRecommendRequest">请求实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<BuildingRecommendResponse> CreateAsync(UserInfo user, BuildingRecommendRequest buildingRecommendRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingRecommendRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingRecommendRequest));
            }

            var building = await _ibuildingStore.GetAsync(x => x.Where(y => !y.IsDeleted && y.ExamineStatus == (int)Models.ExamineStatusEnum.Approved && y.Id == buildingRecommendRequest.BuildingId));
            if (building == null)
                return null;



            var buildingRecommend = _mapper.Map<BuildingRecommend>(buildingRecommendRequest);

            buildingRecommend.Id = Guid.NewGuid().ToString();
            buildingRecommend.CreateUser = user.Id;
            buildingRecommend.CreateTime = DateTime.Now;
            buildingRecommend.RecommendUserId = user.Id;

            if (buildingRecommend.IsRegion)
            {
                var ss = await _permissionExpansionManager.HavePermission(user.Id, "RECOMMEND_REGION");
                if (!ss)
                    return null;

                var mainareaid = (await _iorganizationExpansionStore.ListAsync(x => x.Where(y => (y.SonId == building.OrganizationId && y.Type == "Region") || (y.OrganizationId == building.OrganizationId && y.Type == "Region")), cancellationToken)).FirstOrDefault()?.OrganizationId;

                if (mainareaid == null)
                {
                    return null;
                }
                else
                {
                    if (await _ibuildingRecommendsStore.GetAsync(x => x.Where(y => y.BuildingId == buildingRecommend.BuildingId && y.IsRegion && !y.IsDeleted && !y.IsOutDate && y.MainAreaId == mainareaid)) != null)
                        return null;
                    buildingRecommend.MainAreaId = mainareaid;
                }
            }
            else
            {
                var ss = await _permissionExpansionManager.HavePermission(user.Id, "RECOMMEND_FILIALE");
                if (!ss)
                    return null;
                var mainareaid = (await _iorganizationExpansionStore.ListAsync(x => x.Where(y => (y.SonId == building.OrganizationId && y.Type == "Filiale") || (y.OrganizationId == building.OrganizationId && y.Type == "Filiale")), cancellationToken)).FirstOrDefault()?.OrganizationId;

                if (await _ibuildingRecommendsStore.GetAsync(x => x.Where(y => y.BuildingId == buildingRecommend.BuildingId && y.IsRegion && !y.IsDeleted && !y.IsOutDate && y.MainAreaId == mainareaid)) != null)
                    return null;
                buildingRecommend.MainAreaId = mainareaid;

            }

            try
            {
                await _ibuildingRecommendsStore.CreateAsync(buildingRecommend, cancellationToken);
            }
            catch
            {
            }
            return _mapper.Map<BuildingRecommendResponse>(buildingRecommend);
        }

        /// <summary>
        /// 根据Id获取楼盘收藏信息
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<BuildingRecommendResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _ibuildingRecommendsStore.GetAsync(a => a.Where(b => b.Id == id).OrderBy(x => x.Order), cancellationToken);
            return _mapper.Map<BuildingRecommendResponse>(response);
        }

        /// <summary>
        /// 获取公司推荐的楼盘信息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="condition"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<BuildingRecommendItem>> SearchFiliale(UserInfo user, PageCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resulte = new PagingResponseMessage<BuildingRecommendItem>();

            //var departmentid = (await _iorganizationExpansionStore.GetAsync(a => a.Where(b => (b.SonId == user.OrganizationId && b.Type == "Filiale") || (b.OrganizationId == user.OrganizationId && b.Type == "Filiale"))))?.OrganizationId;Source sequence contains more than one element.
            var departmentid = (await _iorganizationExpansionStore.ListAsync(a => a.Where(b => (b.SonId == user.OrganizationId && b.Type == "Filiale") || (b.OrganizationId == user.OrganizationId && b.Type == "Filiale")))).FirstOrDefault()?.OrganizationId;
            var response = _ibuildingRecommendsStore.BuildingRecommendAll().Where(b => !b.IsOutDate && !b.IsDeleted && !b.Buildings.IsDeleted);
            if (departmentid == null)
            {
                return resulte;
            }
            else
                response = response.Where(b => b.MainAreaId == departmentid);


            if (!string.IsNullOrEmpty(condition.KeyWord))
            {
                response = response.Where(x => x.Buildings.BuildingBaseInfo.Name.Contains(condition.KeyWord));
            }
            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');

            var query = await response.OrderByDescending(x => x.CreateTime).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);

            var aasd = query.Select(a => new BuildingRecommendItem
            {
                Id = a.Id,
                BuildingId = a.BuildingId,
                RecommendUserId = a.RecommendUserId,
                RecommendTime = a.RecommendTime,
                RecommendDays = a.RecommendDays,
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
            }).ToList();

            resulte.PageIndex = condition.PageIndex;
            resulte.PageSize = condition.PageSize;
            resulte.TotalCount = await response.CountAsync(cancellationToken);
            resulte.Extension = aasd;
            return resulte;
        }

        /// <summary>
        /// 获取大区推荐的楼盘信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<BuildingRecommendItem>> SearchMainArea(UserInfo user, PageCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resulte = new PagingResponseMessage<BuildingRecommendItem>();

            var departmentid = (await _iorganizationExpansionStore.ListAsync(a => a.Where(b => (b.SonId == user.OrganizationId && b.Type == "Region") || (b.OrganizationId == user.OrganizationId && b.Type == "Region")))).Select(x => x.OrganizationId);

            var response = _ibuildingRecommendsStore.BuildingRecommendAll().Where(b => !b.IsOutDate && !b.IsDeleted && b.IsRegion && !b.Buildings.IsDeleted);
            if (departmentid == null)
            {
                return resulte;
            }
            else
                response = response.Where(b => departmentid.Contains(b.MainAreaId));


            if (!string.IsNullOrEmpty(condition.KeyWord))
            {
                response = response.Where(x => x.Buildings.BuildingBaseInfo.Name.Contains(condition.KeyWord));
            }
            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');

            var query = await response.OrderBy(x => x.Order).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);

            var aasd = query.Select(a => new BuildingRecommendItem
            {
                Id = a.Id,
                BuildingId = a.BuildingId,
                RecommendUserId = a.RecommendUserId,
                RecommendTime = a.RecommendTime,
                RecommendDays = a.RecommendDays,
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
            }).ToList();

            resulte.PageIndex = condition.PageIndex;
            resulte.PageSize = condition.PageSize;
            resulte.TotalCount = await response.CountAsync(cancellationToken);
            resulte.Extension = aasd;
            return resulte;
        }

        /// <summary>
        /// 获取大区推荐的楼盘信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<BuildingRecommendResponse>> SearchMainArea2(UserInfo user, PageCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resulte = new PagingResponseMessage<BuildingRecommendResponse>();

            //var departmentid = (await _iorganizationExpansionStore.GetAsync(a => a.Where(b => (b.SonId == user.OrganizationId && b.Type == "Region") || (b.OrganizationId == user.OrganizationId && b.Type == "Region"))))?.OrganizationId;

            //获取当前用户有没有大区上级
            var departmentid = (await _iorganizationExpansionStore.ListAsync(a => a.Where(b => (b.SonId == user.OrganizationId && b.Type == "Region") || (b.OrganizationId == user.OrganizationId && b.Type == "Region")), cancellationToken)).Select(x => x.OrganizationId).ToList();


            var response = _ibuildingRecommendsStore.BuildingRecommendAll().Where(b => !b.IsOutDate && !b.IsDeleted && b.IsRegion && !b.Buildings.IsDeleted);

            if (departmentid?.Count != 0)
            {
                response = response.OrderByDescending(x => departmentid.Contains(x.MainAreaId)).ThenByDescending(x => x.MainAreaName).ThenByDescending(x => x.CreateTime);
            }
            else
            {
                response = response.OrderByDescending(x => x.MainAreaName).ThenByDescending(x => x.CreateTime);
            }


            if (!string.IsNullOrEmpty(condition.KeyWord))
            {
                response = response.Where(x => x.Buildings.BuildingBaseInfo.Name.Contains(condition.KeyWord));
            }
            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');

            var query = await response.Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);

            var aasd = query.Select(a => new BuildingRecommendItem
            {
                Id = a.Id,
                BuildingId = a.BuildingId,
                RecommendUserId = a.RecommendUserId,
                RecommendTime = a.RecommendTime,
                RecommendDays = a.RecommendDays,
                UserNikeName = a.UserNikeName,
                MainAreaName = a.MainAreaName,
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
            }).ToList();

            resulte.PageIndex = condition.PageIndex;
            resulte.PageSize = condition.PageSize;
            resulte.TotalCount = await response.CountAsync(cancellationToken);

            var group = aasd.GroupBy(x => x.MainAreaName);

            var q = new List<BuildingRecommendResponse>();
            foreach (var item in group)
            {
                q.Add(new BuildingRecommendResponse
                {
                    OrganName = item.Key,
                    Source = item.Select(e => e).ToList()
                });
            }



            resulte.Extension = q;
            return resulte;
        }

        /// <summary>
        /// 获取我推荐的楼盘信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<BuildingRecommendItem>> FindMyBuildingRecommendAsync(UserInfo user, PageCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resulte = new PagingResponseMessage<BuildingRecommendItem>();
            var response = _ibuildingRecommendsStore.BuildingRecommendAll();
            var organs = await _permissionExpansionManager.GetLowerDepartments(user.OrganizationId);

            if (await _permissionExpansionManager.HavePermission(user.Id, "RECOMMEND_FILIALE") && await _permissionExpansionManager.HavePermission(user.Id, "RECOMMEND_REGION"))
            {
                response = response.Where(b => organs.Contains(b.Buildings.OrganizationId) /*b.RecommendUserId == userId*/ && !b.IsDeleted && !b.Buildings.IsDeleted);
            }
            else if (await _permissionExpansionManager.HavePermission(user.Id, "RECOMMEND_FILIALE"))
            {
                response = response.Where(b => organs.Contains(b.Buildings.OrganizationId) /*b.RecommendUserId == userId*/ && !b.IsDeleted && !b.Buildings.IsDeleted && !b.IsRegion);
            }
            else if (await _permissionExpansionManager.HavePermission(user.Id, "RECOMMEND_REGION"))
            {
                response = response.Where(b => organs.Contains(b.Buildings.OrganizationId) /*b.RecommendUserId == userId*/ && !b.IsDeleted && !b.Buildings.IsDeleted && b.IsRegion);
            }
            else
            {
                return resulte;
            }
            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');

            var query = await response.OrderByDescending(x => x.CreateTime).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);

            var aasd = query.Select(a => new BuildingRecommendItem
            {
                Id = a.Id,
                BuildingId = a.BuildingId,
                RecommendUserId = a.RecommendUserId,
                RecommendTime = a.RecommendTime,
                RecommendDays = a.RecommendDays,
                UserNikeName = a.UserNikeName,
                IsOutDate = a.IsOutDate,
                IsRegion = a.IsRegion,
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
            }).ToList();

            resulte.PageIndex = condition.PageIndex;
            resulte.PageSize = condition.PageSize;
            resulte.TotalCount = await response.CountAsync(cancellationToken);
            resulte.Extension = aasd;
            return resulte;
        }

        /// <summary>
        /// 修改单个楼盘收藏信息
        /// </summary>
        /// <param name="userId">请求者Id</param>
        /// <param name="buildingRecommendRequest">请求数据</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(string userId, BuildingRecommendRequest buildingRecommendRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingRecommendRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingRecommendRequest));
            }
            var buildingRecommend = await _ibuildingRecommendsStore.GetAsync(a => a.Where(b => b.Id == buildingRecommendRequest.Id && !b.IsDeleted));
            if (buildingRecommend == null)
            {
                return;
            }
            var newbuildingRecommend = _mapper.Map<BuildingRecommend>(buildingRecommendRequest);

            newbuildingRecommend.IsDeleted = buildingRecommend.IsDeleted;
            newbuildingRecommend.CreateTime = buildingRecommend.CreateTime;
            newbuildingRecommend.CreateUser = buildingRecommend.CreateUser;
            newbuildingRecommend.UpdateTime = DateTime.Now;
            newbuildingRecommend.UpdateUser = userId;
            newbuildingRecommend.RecommendUserId = buildingRecommend.RecommendUserId;

            //判断是否改变排序
            if (newbuildingRecommend.Order != buildingRecommend.Order)
            {
                var query = _ibuildingRecommendsStore.BuildingRecommendAll().Where(x => x.Order >= newbuildingRecommend.Order);
                foreach (var item in query)
                {
                    item.Order = item.Order - 1;
                }
                await _ibuildingRecommendsStore.UpdateListAsync(await query.ToListAsync(cancellationToken), cancellationToken);
            }
            try
            {
                await _ibuildingRecommendsStore.UpdateAsync(newbuildingRecommend, cancellationToken);
            }
            catch { }
        }

        /// <summary>
        /// 删除楼盘收藏信息
        /// </summary>
        /// <param name="user">删除人基本信息</param>
        /// <param name="id">删除楼盘收藏Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await _ibuildingRecommendsStore.DeleteAsync(_mapper.Map<SimpleUser>(user), new BuildingRecommend() { Id = id }, cancellationToken);
            }
            catch (Exception e) { }
        }

        /// <summary>
        /// 批量获取楼盘推荐
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<int> AllCountAsync(BuildingRecommendRequest buildingRecommendRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var building = await _ibuildingStore.GetAsync(x => x.Where(y => !y.IsDeleted && y.ExamineStatus == (int)Models.ExamineStatusEnum.Approved && y.Id == buildingRecommendRequest.BuildingId));
                if (building == null)
                    return -1;

                var departmentid = (await _iorganizationExpansionStore.ListAsync(a => a.Where(b => (b.SonId == building.OrganizationId && b.Type == "Filiale") || (b.OrganizationId == building.OrganizationId && b.Type == "Filiale")))).FirstOrDefault()?.OrganizationId;
                if (departmentid == null)
                    return -2;


                var response = _ibuildingRecommendsStore.BuildingRecommendAll().Where(x => !x.IsOutDate && !x.IsDeleted && !x.IsRegion && !x.Buildings.IsDeleted && x.MainAreaId == departmentid);
                return await response.CountAsync(cancellationToken);
            }
            catch (Exception e)
            {
                return 0;

            }
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
            var buildingRecommend = await _ibuildingRecommendsStore.ListAsync(a => a.Where(b => ids.Contains(b.Id)), cancellationToken);
            if (buildingRecommend == null || buildingRecommend.Count == 0)
            {
                return;
            }
            for (int i = 0; i < buildingRecommend.Count; i++)
            {
                buildingRecommend[i].DeleteUser = userId;
                buildingRecommend[i].DeleteTime = DateTime.Now;
                buildingRecommend[i].IsDeleted = true;
            }
            try
            {
                await _ibuildingRecommendsStore.UpdateListAsync(buildingRecommend, cancellationToken);
            }
            catch { }
        }
    }
}
