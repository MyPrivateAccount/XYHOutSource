using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Managers;
using ApplicationCore.Models;
using ApplicationCore.Stores;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Dto.Response;
using XYHShopsPlugin.Models;
using XYHShopsPlugin.Stores;

namespace XYHShopsPlugin.Managers
{
    public class BuildingsManager
    {
        public BuildingsManager(
            IBuildingsStore buildingsStore,
            IBuildingFacilitiesStore buildingFacilitiesStore,
            IBuildingFavoritesStore buildingFavoritesStore,
            IBuildingBaseInfoStore buildingBaseInfoStore,
            IBuildingShopInfoStore buildingShopInfoStore,
            IBuildingFileScopeStore buildingFileScopeStore,
            IFileInfoStore fileInfoStore,
            IUserStore userStore,
            PermissionExpansionManager permissionExpansionManager,
            IMapper mapper
            )
        {
            _buildingsStore = buildingsStore ?? throw new ArgumentNullException(nameof(buildingsStore));
            _buildingFacilitiesStore = buildingFacilitiesStore ?? throw new ArgumentNullException(nameof(buildingFacilitiesStore));
            _ibuildingFavoritesStore = buildingFavoritesStore ?? throw new ArgumentNullException(nameof(buildingFavoritesStore));
            _buildingBaseInfoStore = buildingBaseInfoStore ?? throw new ArgumentNullException(nameof(buildingBaseInfoStore));
            _buildingShopInfoStore = buildingShopInfoStore ?? throw new ArgumentNullException(nameof(buildingShopInfoStore));
            _fileInfoStore = fileInfoStore;
            _buildingFileScopeStore = buildingFileScopeStore;
            _iuserStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            _ipermissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        protected IBuildingsStore _buildingsStore { get; }
        protected IBuildingFacilitiesStore _buildingFacilitiesStore { get; }
        protected IBuildingFavoritesStore _ibuildingFavoritesStore { get; }
        protected IBuildingBaseInfoStore _buildingBaseInfoStore { get; }
        protected IBuildingShopInfoStore _buildingShopInfoStore { get; }
        protected IFileInfoStore _fileInfoStore { get; }
        protected IBuildingFileScopeStore _buildingFileScopeStore { get; }
        protected IUserStore _iuserStore { get; }
        protected PermissionExpansionManager _ipermissionExpansionManager { get; }
        protected IMapper _mapper { get; }
        private readonly ILogger Logger = LoggerManager.GetLogger("BuildingsManager");


        public virtual async Task<BuildingResponse> CreateAsync(string userId, BuildingRequest buildingRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingRequest));
            }

            var buildings = _mapper.Map<Buildings>(buildingRequest);
            buildings.CreateUser = userId;
            buildings.CreateTime = DateTime.Now;
            if (await _ipermissionExpansionManager.HavePermission(userId, "OnSiteRoles"))
            {
                buildings.ResidentUser1 = userId;
            }
            await _buildingsStore.CreateAsync(buildings, cancellationToken);
            return _mapper.Map<BuildingResponse>(buildings);
        }


        public async Task<BuildingResponse> FindByIdAsync(string userid, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = _buildingsStore.GetDetailQuery();
            query = query.Where(x => x.Id == id);
            if (await query.CountAsync() == 0)
            {
                return null;
            }

            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');

            var building = await query.FirstOrDefaultAsync(cancellationToken);
            building.Icon = string.IsNullOrEmpty(building.Icon) ? "" : fr + "/" + building.Icon.TrimStart('/');
            BuildingResponse buildingResponse = _mapper.Map<BuildingResponse>(building);
            buildingResponse.FileList = new List<FileItemResponse>();
            buildingResponse.AttachmentList = new List<AttachmentResponse>();

            if (building.BuildingFileInfos != null && building.BuildingFileInfos.Count() > 0)
            {
                var f = building.BuildingFileInfos.Select(a => a.FileGuid).Distinct();
                foreach (var item in f)
                {
                    var f1 = building.BuildingFileInfos.Where(a => a.Type != "Attachment" && a.FileGuid == item).ToList();
                    if (f1?.Count > 0)
                    {
                        buildingResponse.FileList.Add(ConvertToFileItem(item, f1));
                    }
                    var f2 = building.BuildingFileInfos.Where(a => a.Type == "Attachment" && a.FileGuid == item).ToList();
                    if (f2?.Count > 0)
                    {
                        buildingResponse.AttachmentList.AddRange(ConvertToAttachmentItem(f2));
                    }
                }
            }
            buildingResponse.IsFavorite = await _ibuildingFavoritesStore.GetAsync(x => x.Where(y => y.UserId == userid && y.BuildingId == buildingResponse.Id && !y.IsDeleted)) == null ? false : true;
            var asdasd = (await _ipermissionExpansionManager.GetUseridsHaveOrganPermission(buildingResponse.OrganizationId, "MyManagerBuildings")).FirstOrDefault();

            var user = await _iuserStore.GetAsync(x => x.Where(y => y.Id == asdasd));

            buildingResponse.ResidentUserManager = new UserInfo
            {
                Id = user?.Id,
                UserName = user?.TrueName,
                PhoneNumber = user?.PhoneNumber
            };



            return buildingResponse;
        }

        private List<AttachmentResponse> ConvertToAttachmentItem(List<FileInfo> f2)
        {
            List<AttachmentResponse> list = new List<AttachmentResponse>();
            string fr = ApplicationContext.Current.FileServerRoot;
            foreach (var item in f2)
            {
                list.Add(new AttachmentResponse
                {
                    FileGuid = item.FileGuid,
                    FileName = item.Name,
                    Group = item.Group,
                    Summary = item.Summary,
                    Url = fr + "/" + item.Uri.TrimStart('/')
                });
            }
            return list;
        }


        private FileItemResponse ConvertToFileItem(string fileGuid, List<FileInfo> fl)
        {
            FileItemResponse fi = new FileItemResponse();
            fi.FileGuid = fileGuid;
            fi.Group = fl.FirstOrDefault()?.Group;
            fi.Icon = fl.FirstOrDefault(x => x.Type == "ICON")?.Uri;
            fi.Original = fl.FirstOrDefault(x => x.Type == "ORIGINAL")?.Uri;
            fi.Medium = fl.FirstOrDefault(x => x.Type == "MEDIUM")?.Uri;
            fi.Small = fl.FirstOrDefault(x => x.Type == "SMALL")?.Uri;

            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');
            if (!String.IsNullOrEmpty(fi.Icon))
            {
                fi.Icon = fr + "/" + fi.Icon.TrimStart('/');
            }
            if (!String.IsNullOrEmpty(fi.Original))
            {
                fi.Original = fr + "/" + fi.Original.TrimStart('/');
            }
            if (!String.IsNullOrEmpty(fi.Medium))
            {
                fi.Medium = fr + "/" + fi.Medium.TrimStart('/');
            }
            if (!String.IsNullOrEmpty(fi.Small))
            {
                fi.Small = fr + "/" + fi.Small.TrimStart('/');
            }
            return fi;
        }


        public virtual async Task<PagingResponseMessage<BuildingSearchResponse>> Search(string userId, BuildingListSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            PagingResponseMessage<BuildingSearchResponse> pagingResponse = new PagingResponseMessage<BuildingSearchResponse>();

            var q = _buildingsStore.GetSimpleSerchQuery().Where(a => !a.IsDeleted && a.ExamineStatus == (int)Models.ExamineStatusEnum.Approved);
            if (condition?.HasBank == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasBank ?? true);
            }
            if (condition?.HasBus == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasBus ?? true);
            }
            if (condition?.HasKindergarten == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasKindergarten ?? true);
            }
            if (condition?.HasMarket == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasMarket ?? true);
            }
            if (condition?.HasMiddleSchool == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasMiddleSchool ?? true);
            }
            if (condition?.HasOtherTraffic == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasOtherTraffic ?? true);
            }
            if (condition?.HasPrimarySchool == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasPrimarySchool ?? true);
            }
            if (condition?.HasRail == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasRail ?? true);
            }
            if (condition?.HasSupermarket == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasSupermarket ?? true);
            }
            if (condition?.HasUniversity == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasUniversity ?? true);
            }
            if (!string.IsNullOrEmpty(condition.KeyWord))
            {
                q = q.Where(a => a.BuildingBaseInfo.Name.Contains(condition.KeyWord));
            }
            if (!string.IsNullOrEmpty(condition.City))
            {
                q = q.Where(a => a.BuildingBaseInfo.City == condition.City);
            }
            if (!string.IsNullOrEmpty(condition.Area))
            {
                q = q.Where(a => a.BuildingBaseInfo.Area == condition.Area);
            }
            if (!string.IsNullOrEmpty(condition.District))
            {
                q = q.Where(a => a.BuildingBaseInfo.District == condition.District);
            }
            if (condition?.LowPrice != null && condition?.HighPrice != null)
            {
                q = q.Where(a => a.BuildingBaseInfo.MaxPrice > condition.LowPrice && a.BuildingBaseInfo.MinPrice < condition.HighPrice);
                //if (condition?.HighPrice != null)
                //{
                //    q = q.Where(a => a.BuildingBaseInfo.MaxPrice < condition.HighPrice &&);
                //}
                //else if (condition?.LowPrice != null)
                //{
                //    q = q.Where(a => a.BuildingBaseInfo.MinPrice > condition.LowPrice);
                //}
                //else
                //{
                //    q = q.Where(a => a.BuildingBaseInfo.MinPrice > condition.LowPrice && a.BuildingBaseInfo.MaxPrice > condition.LowPrice || a.BuildingBaseInfo.MaxPrice < condition.HighPrice);
                //}
            }
            else if (condition?.LowPrice != null && condition?.HighPrice == null)
            {
                q = q.Where(a => a.BuildingBaseInfo.MinPrice > condition.LowPrice);
            }
            //if (condition?.HighPrice != null)
            //{
            //    q = q.Where(a => a.BuildingBaseInfo.MaxPrice < condition.HighPrice);
            //}
            if (condition?.SaleStatus?.Count > 0)
            {
                q = q.Where(a => condition.SaleStatus.Contains(a.BuildingShopInfo.SaleStatus));
            }
            if (condition?.PriceIsAscSort == null)
            {
                q = q.OrderBy(a => a.BuildingBaseInfo.OpenDate);
            }
            if (condition?.PriceIsAscSort == true)
            {
                q = q.OrderBy(a => (a.BuildingBaseInfo.MinPrice ?? 0 + a.BuildingBaseInfo.MinPrice ?? 0) / (decimal)2);
            }
            if (condition?.PriceIsAscSort == false)
            {
                q = q.OrderByDescending(a => (a.BuildingBaseInfo.MinPrice ?? 0 + a.BuildingBaseInfo.MinPrice ?? 0) / (decimal)2);
            }
            if (condition.IsReport)
            {
                q = q.Where(a => a.BuildingRule != null && (a.BuildingRule.ReportTime == null || a.BuildingRule.ReportTime != null && a.BuildingRule.ReportTime < DateTime.Now.AddDays(5))/* && a.BuildingRule.IsUse*/);
            }
            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');

            pagingResponse.TotalCount = await q.CountAsync(cancellationToken);
            var qlist = await q.Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);




            var resulte = qlist.Select(a => new BuildingSearchResponse
            {
                Id = a.Id,
                Address = a.BuildingBaseInfo.Address,
                AreaFullName = a.BuildingBaseInfo.CityDefine.Name + "-" + a.BuildingBaseInfo.DistrictDefine.Name + "-" + a.BuildingBaseInfo.AreaDefine.Name,
                MaxPrice = a.BuildingBaseInfo.MaxPrice,
                MinPrice = a.BuildingBaseInfo.MinPrice,
                Name = a.BuildingBaseInfo.Name,
                Icon = string.IsNullOrEmpty(a.Icon) ? "" : fr + "/" + a.Icon.TrimStart('/'),
                HasBus = a.BuildingFacilities.HasBus,
                HasRail = a.BuildingFacilities.HasRail,
                HasOtherTraffic = a.BuildingFacilities.HasOtherTraffic,
                HasKindergarten = a.BuildingFacilities.HasKindergarten,
                HasPrimarySchool = a.BuildingFacilities.HasPrimarySchool,
                HasMiddleSchool = a.BuildingFacilities.HasMiddleSchool,
                HasUniversity = a.BuildingFacilities.HasUniversity,
                HasMarket = a.BuildingFacilities.HasMarket,
                HasSupermarket = a.BuildingFacilities.HasSupermarket,
                HasBank = a.BuildingFacilities.HasBank,

                BeltLook = a.BuildingRule?.ReportTime
            });
            pagingResponse.PageIndex = condition.PageIndex;
            pagingResponse.PageSize = condition.PageSize;
            pagingResponse.Extension = resulte.ToList();
            return pagingResponse;
        }

        public virtual async Task<List<BuildingSiteResponse>> SearchBulidingSite(string userid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var organ = await _ipermissionExpansionManager.GetOrganizationOfPermission(userid, "APPOINT_SCENE");


            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');

            List<BuildingSiteResponse> list = new List<BuildingSiteResponse>();
            var q = _buildingsStore.GetSimpleSerchQuery().Where(x => !x.IsDeleted && organ.Contains(x.OrganizationId));

            q = q.OrderByDescending(x => x.CreateTime);
            var result = await q.ToListAsync();

            result = result.Select(x => { x.Icon = string.IsNullOrEmpty(x.Icon) ? "" : fr + "/" + x.Icon.TrimStart('/'); return x; }).ToList();

            list.AddRange(_mapper.Map<List<BuildingSiteResponse>>(result));

            return list;
        }

        public virtual async Task<List<BuildingResponse>> SimpleSearchOld(UserInfo user, BuildingSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            List<BuildingResponse> list = new List<BuildingResponse>();
            var q = _buildingsStore.GetSimpleSerchQuery();
            q = q.Where(x => !x.IsDeleted);
            if (condition != null)
            {
                if (!String.IsNullOrEmpty(condition.ResidentUser))
                {
                    q = q.Where(b => (b.ResidentUser1 == condition.ResidentUser || b.ResidentUser2 == condition.ResidentUser || b.ResidentUser3 == condition.ResidentUser || b.ResidentUser4 == condition.ResidentUser));
                }
                if (!String.IsNullOrEmpty(condition.KeyWord))
                {
                    q = q.Where(b => (b.BuildingBaseInfo.Name.Contains(condition.KeyWord)));
                }
            }
            q = q.OrderByDescending(x => x.CreateTime);
            var result = await q.ToListAsync();

            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');

            result = result.Select(x => { x.Icon = string.IsNullOrEmpty(x.Icon) ? "" : fr + "/" + x.Icon.TrimStart('/'); return x; }).ToList();

            list.AddRange(_mapper.Map<List<BuildingResponse>>(result));

            return list;
        }

        public async Task<bool> IsResidentUser(string userId, string buildingId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var organ = await _ipermissionExpansionManager.GetOrganizationOfPermission(userId, "MyManagerBuildings");
            if (organ.Count == 0)
            {
                return (await _buildingsStore.ListAsync(x => x.Where(a => (a.ResidentUser1 == userId || a.ResidentUser2 == userId || a.ResidentUser3 == userId || a.ResidentUser4 == userId) && a.Id == buildingId)))?.Count != 0 ? true : false;
            }
            else
            {
                return (await _buildingsStore.ListAsync(x => x.Where(a => (organ.Contains(a.OrganizationId) || a.ResidentUser1 == userId || a.ResidentUser2 == userId || a.ResidentUser3 == userId || a.ResidentUser4 == userId) && a.Id == buildingId)))?.Count != 0 ? true : false;
            }
        }

        public async Task<bool> IsManagerSiteUserAsync(string userid, string buildingId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var sss = false;
            var departmentid = string.Empty;
            departmentid = (await _iuserStore.GetAsync(x => x.Where(y => y.Id == userid))).OrganizationId;
            var organs = await _ipermissionExpansionManager.GetLowerDepartments(departmentid);
            sss = (await _buildingsStore.ListAsync(x => x.Where(a => a.Id == buildingId && organs.Contains(a.OrganizationId))))?.Count != 0 ? true : false;
            return sss;
        }


        /// <summary>
        /// 驻场列表
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<UserResponse>> InSiteList(string userid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var users = await _ipermissionExpansionManager.GetPermissionUserIds("OnSiteRoles");

            var organ = await _ipermissionExpansionManager.GetOrganizationOfPermission(userid, "APPOINT_SCENE");

            var userids = (await _iuserStore.ListAsync(x => x.Where(y => organ.Contains(y.OrganizationId) && users.Contains(y.Id) && !y.IsDeleted)));

            var response = userids.Select(x => new UserResponse
            {
                Id = x.Id,
                PhoneNumber = x.PhoneNumber,
                TrueName = x.TrueName

            }).ToList();


            return response;
        }


        public virtual async Task<List<BuildingResponse>> SimpleSearch(UserInfo user, BuildingSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            var q = _buildingsStore.GetSimpleSerchQuery().Where(a => !a.IsDeleted);

            var organ = await _ipermissionExpansionManager.GetOrganizationOfPermission(user.Id, "MyManagerBuildings");
            if (organ.Count == 0)
            {
                q = q.Where(a => a.ResidentUser1 == user.Id || a.ResidentUser2 == user.Id || a.ResidentUser3 == user.Id || a.ResidentUser4 == user.Id);
            }
            else
            {
                q = q.Where(a => organ.Contains(a.OrganizationId) || a.ResidentUser1 == user.Id || a.ResidentUser2 == user.Id || a.ResidentUser3 == user.Id || a.ResidentUser4 == user.Id);
            }

            //if (!String.IsNullOrEmpty(condition?.ResidentUser))
            //{
            //    q = q.Where(b => (b.ResidentUser1 == condition.ResidentUser || b.ResidentUser2 == condition.ResidentUser || b.ResidentUser3 == condition.ResidentUser));
            //}
            if (!String.IsNullOrEmpty(condition?.KeyWord))
            {
                q = q.Where(b => (b.BuildingBaseInfo.Name.Contains(condition.KeyWord)));
            }
            if (condition?.HasBank == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasBank ?? true);
            }
            if (condition?.HasBus == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasBus ?? true);
            }
            if (condition?.HasKindergarten == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasKindergarten ?? true);
            }
            if (condition?.HasMarket == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasMarket ?? true);
            }
            if (condition?.HasMiddleSchool == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasMiddleSchool ?? true);
            }
            if (condition?.HasOtherTraffic == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasOtherTraffic ?? true);
            }
            if (condition?.HasPrimarySchool == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasPrimarySchool ?? true);
            }
            if (condition?.HasRail == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasRail ?? true);
            }
            if (condition?.HasSupermarket == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasSupermarket ?? true);
            }
            if (condition?.HasUniversity == true)
            {
                q = q.Where(a => a.BuildingFacilities.HasUniversity ?? true);
            }
            if (!string.IsNullOrEmpty(condition.KeyWord))
            {
                q = q.Where(a => a.BuildingBaseInfo.Name.Contains(condition.KeyWord));
            }
            if (!string.IsNullOrEmpty(condition.City))
            {
                q = q.Where(a => a.BuildingBaseInfo.City == condition.City);
            }
            if (!string.IsNullOrEmpty(condition.Area))
            {
                q = q.Where(a => a.BuildingBaseInfo.Area == condition.Area);
            }
            if (!string.IsNullOrEmpty(condition.District))
            {
                q = q.Where(a => a.BuildingBaseInfo.District == condition.District);
            }
            if (condition?.LowPrice != null)
            {
                q = q.Where(a => ((a.BuildingBaseInfo.MinPrice ?? 0 + a.BuildingBaseInfo.MaxPrice ?? 0).CompareTo(condition.LowPrice) > 0));
            }
            if (condition?.HighPrice != null)
            {
                q = q.Where(a => ((a.BuildingBaseInfo.MinPrice ?? 0 + a.BuildingBaseInfo.MaxPrice ?? 0).CompareTo(condition.HighPrice) < 0));
            }
            if (condition?.SaleStatus?.Count > 0)
            {
                q = q.Where(a => condition.SaleStatus.Contains(a.BuildingShopInfo.SaleStatus));
            }
            if (condition?.PriceIsAscSort == null)
            {
                q = q.OrderBy(a => a.BuildingBaseInfo.OpenDate);
            }
            if (condition?.PriceIsAscSort == true)
            {
                q = q.OrderBy(a => (a.BuildingBaseInfo.MinPrice ?? 0 + a.BuildingBaseInfo.MinPrice ?? 0) / (decimal)2);
            }
            if (condition?.PriceIsAscSort == false)
            {
                q = q.OrderByDescending(a => (a.BuildingBaseInfo.MinPrice ?? 0 + a.BuildingBaseInfo.MinPrice ?? 0) / (decimal)2);
            }
            if (condition?.ExamineStatus != null && condition?.ExamineStatus.Count != 0)
            {
                q = q.Where(a => condition.ExamineStatus.Contains(a.ExamineStatus));
            }



            q = q.OrderByDescending(x => x.CreateTime);
            //var result = await q.Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);
            var result = await q.ToListAsync(cancellationToken);

            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');

            result = result.Select(x => { x.Icon = string.IsNullOrEmpty(x.Icon) ? "" : fr + "/" + x.Icon.TrimStart('/'); return x; }).ToList();

            return _mapper.Map<List<BuildingResponse>>(result);
        }


        public virtual async Task DeleteAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _buildingsStore.DeleteAsync(_mapper.Map<SimpleUser>(user), new Buildings() { Id = id });
            //var building = await _buildingsStore.GetAsync(a => a.Where(b => b.Id == id && !b.IsDeleted), cancellationToken);
            //if (building == null)
            //{
            //    return;
            //}
            //building.IsDeleted = true;
            //building.DeleteTime = DateTime.Now;
            //building.DeleteUser = userId;
            //await _buildingsStore.UpdateAsync(building, cancellationToken);
        }

        public virtual async Task DeleteListAsync(string userId, List<string> ids, CancellationToken cancellationToken = default(CancellationToken))
        {
            var buildings = await _buildingsStore.ListAsync(a => a.Where(b => ids.Contains(b.Id) && !b.IsDeleted), cancellationToken);
            if (buildings == null || buildings.Count == 0)
            {
                return;
            }
            for (int i = 0; i < buildings.Count; i++)
            {
                buildings[i].DeleteUser = userId;
                buildings[i].DeleteTime = DateTime.Now;
                buildings[i].IsDeleted = true;
            }
            await _buildingsStore.UpdateListAsync(buildings, cancellationToken);
        }


        public virtual async Task UpdateAsync(string userId, string id, BuildingRequest buildingRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingRequest));
            }
            var building = await _buildingsStore.GetAsync(a => a.Where(b => b.Id == id && !b.IsDeleted));
            if (building == null)
            {
                return;
            }
            var newBuilding = _mapper.Map<Buildings>(buildingRequest);
            newBuilding.IsDeleted = building.IsDeleted;
            newBuilding.CreateTime = building.CreateTime;
            newBuilding.CreateUser = building.CreateUser;
            newBuilding.UpdateTime = DateTime.Now;
            newBuilding.UpdateUser = userId;
            await _buildingsStore.UpdateAsync(newBuilding, cancellationToken);
        }

        public virtual async Task SaveResidentUserAsync(string userId, BuildingsOnSiteRequest buildingsOnSiteRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingsOnSiteRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingsOnSiteRequest));
            }
            var building = await _buildingsStore.GetAsync(a => a.Where(b => b.Id == buildingsOnSiteRequest.Id && !b.IsDeleted));
            if (building == null)
            {
                return;
            }
            building.ResidentUser1 = buildingsOnSiteRequest.ResidentUser1;
            building.ResidentUser2 = buildingsOnSiteRequest.ResidentUser2;
            building.ResidentUser3 = buildingsOnSiteRequest.ResidentUser3;
            building.ResidentUser4 = buildingsOnSiteRequest.ResidentUser4;
            building.UpdateTime = DateTime.Now;
            building.UpdateUser = userId;
            await _buildingsStore.UpdateAsync(building, cancellationToken);
        }


        public virtual async Task SaveSummaryAsync(UserInfo user, string buildingId, string summary, string source, string sourceId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            await _buildingsStore.SaveSummaryAsync(_mapper.Map<SimpleUser>(user), buildingId, summary, source, sourceId, cancellationToken);
        }

        public virtual async Task SaveCommissionAsync(UserInfo user, string buildingId, string commission, string source, string sourceId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await _buildingsStore.SaveCommissionAsync(_mapper.Map<SimpleUser>(user), buildingId, commission, source, sourceId, cancellationToken);
        }

        public virtual async Task SubmitAsync(string buildingId, Dto.ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _buildingsStore.UpdateExamineStatus(buildingId, (Models.ExamineStatusEnum)(int)status, cancellationToken);
        }



    }
}
