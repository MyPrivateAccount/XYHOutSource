using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Dto.Request;
using XYHShopsPlugin.Dto.Response;
using XYHShopsPlugin.Models;
using XYHShopsPlugin.Stores;

namespace XYHShopsPlugin.Managers
{
    public class ShopsManager
    {
        public ShopsManager(
            IShopsStore shopsStore,
            IShopFacilitiesStore shopFacilitiesStore,
            IShopsFavoriteStore shopsFavoriteStore,
            IShopBaseInfoStore shopBaseInfoStore,
            IShopLeaseInfoStore shopLeaseInfoStore,
            IFileInfoStore fileInfoStore,
            IShopsFileScopeStore shopFileScopeStore,
            IMapper mapper
            )
        {
            _shopsStore = shopsStore ?? throw new ArgumentNullException(nameof(shopsStore));
            _shopFacilitiesStore = shopFacilitiesStore ?? throw new ArgumentNullException(nameof(shopFacilitiesStore));
            _ishopsFavoriteStore = shopsFavoriteStore ?? throw new ArgumentNullException(nameof(shopsFavoriteStore));
            _shopBaseInfoStore = shopBaseInfoStore ?? throw new ArgumentNullException(nameof(shopBaseInfoStore));
            _shopLeaseInfoStore = shopLeaseInfoStore ?? throw new ArgumentNullException(nameof(shopLeaseInfoStore));
            _fileInfoStore = fileInfoStore;
            _shopFileScopeStore = shopFileScopeStore;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        protected IShopsStore _shopsStore { get; }
        protected IShopFacilitiesStore _shopFacilitiesStore { get; }
        protected IShopsFavoriteStore _ishopsFavoriteStore { get; }
        protected IShopBaseInfoStore _shopBaseInfoStore { get; }
        protected IShopLeaseInfoStore _shopLeaseInfoStore { get; }
        protected IFileInfoStore _fileInfoStore { get; }
        protected IShopsFileScopeStore _shopFileScopeStore { get; }
        protected IMapper _mapper { get; }


        public virtual async Task<ShopInfoResponse> CreateAsync(UserInfo user, ShopsRequest shopsRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopsRequest == null)
            {
                throw new ArgumentNullException(nameof(shopsRequest));
            }
            var shops = _mapper.Map<Shops>(shopsRequest);
            shops.CreateUser = user.Id;
            shops.CreateTime = DateTime.Now;
            shops.OrganizationId = user.OrganizationId;
            await _shopsStore.CreateAsync(shops);
            return _mapper.Map<ShopInfoResponse>(shops);
        }
        public virtual async Task<ShopInfoResponse> FindByIdAsync(string userid, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = _shopsStore.GetDetailQuery();
            query = query.Where(x => x.Id == id);

            var shops = await query.FirstOrDefaultAsync(cancellationToken);
            if (shops == null) return null;

            ShopInfoResponse shopsResponse = _mapper.Map<ShopInfoResponse>(shops);

            shopsResponse.FileList = new List<FileItemResponse>();
            shopsResponse.AttachmentList = new List<AttachmentResponse>();

            if (shops.ShopsFileInfos?.Count() > 0)
            {
                var f = shops.ShopsFileInfos.Select(a => a.FileGuid).Distinct();
                foreach (var item in f)
                {
                    var f1 = shops.ShopsFileInfos.Where(a => a.Type != "Attachment" && a.FileGuid == item).ToList();
                    if (f1?.Count > 0)
                    {
                        shopsResponse.FileList.Add(ConvertToFileItem(item, f1));
                    }
                    var f2 = shops.ShopsFileInfos.Where(a => a.Type == "Attachment" && a.FileGuid == item).ToList();
                    if (f2?.Count > 0)
                    {
                        shopsResponse.AttachmentList.AddRange(ConvertToAttachmentItem(f2));
                    }
                }
            }
            shopsResponse.IsFavorite = await _ishopsFavoriteStore.GetAsync(x => x.Where(y => y.UserId == userid && y.ShopsId == shopsResponse.Id && !y.IsDeleted)) == null ? false : true;
            return shopsResponse;
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
                    Summary = item.Summary,
                    Group = item.Group,
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


        public virtual async Task<PagingResponseMessage<ShopListSearchResponse>> Search(string userId, ShopListSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            PagingResponseMessage<ShopListSearchResponse> pagingResponse = new PagingResponseMessage<ShopListSearchResponse>();

            var q = _shopsStore.GetDetailQuery().Where(a => !a.IsDeleted && a.ExamineStatus == (int)Models.ExamineStatusEnum.Approved);
            if (condition?.BuildingIds?.Count > 0)
            {
                q = q.Where(a => condition.BuildingIds.Contains(a.BuildingId));
            }

            if (!string.IsNullOrEmpty(condition?.CustomerId))
            {
                var shopsid = (await _shopsStore.ListNotShopsAsync(x => x.Where(y => y.CustomerId == condition.CustomerId))).Select(x => x.ShopsId);
                q = q.Where(a => !shopsid.Contains(a.Id));
            }
            if (condition?.SaleStatus?.Count > 0)
            {
                q = q.Where(a => condition.SaleStatus.Contains(a.ShopBaseInfo.SaleStatus));
            }
            if (condition?.UpperWater == true)
            {
                q = q.Where(a => a.ShopFacilities.UpperWater ?? true);
            }
            if (condition?.DownWater == true)
            {
                q = q.Where(a => a.ShopFacilities.DownWater ?? true);
            }
            if (condition?.Gas == true)
            {
                q = q.Where(a => a.ShopFacilities.Gas ?? true);
            }
            if (condition?.Chimney == true)
            {
                q = q.Where(a => a.ShopFacilities.Chimney ?? true);
            }
            if (condition?.Blowoff == true)
            {
                q = q.Where(a => a.ShopFacilities.Blowoff ?? true);
            }
            if (condition?.Split == true)
            {
                q = q.Where(a => a.ShopFacilities.Split ?? true);
            }
            if (condition?.Elevator == true)
            {
                q = q.Where(a => a.ShopFacilities.Elevator ?? true);
            }
            if (condition?.Staircase == true)
            {
                q = q.Where(a => a.ShopFacilities.Staircase ?? true);
            }
            if (condition?.Outside == true)
            {
                q = q.Where(a => a.ShopFacilities.Outside ?? true);
            }
            if (condition?.OpenFloor == true)
            {
                q = q.Where(a => a.ShopFacilities.OpenFloor ?? true);
            }
            if (condition?.ParkingSpace == true)
            {
                q = q.Where(a => a.ShopFacilities.ParkingSpace ?? true);
            }
            if (condition?.IsCorner == true)
            {
                q = q.Where(a => a.ShopBaseInfo.IsCorner ?? true);
            }
            if (condition?.IsFaceStreet == true)
            {
                q = q.Where(a => a.ShopBaseInfo.IsFaceStreet ?? true);
            }
            if (condition?.HasFree == true)
            {
                q = q.Where(a => a.ShopBaseInfo.HasFree ?? true);
            }
            if (condition?.HasStreet == true)
            {
                q = q.Where(a => a.ShopBaseInfo.HasStreet ?? true);
            }
            if (!string.IsNullOrEmpty(condition.KeyWord))
            {
                q = q.Where(a => a.ShopBaseInfo.Name.Contains(condition.KeyWord));
            }
            if (!string.IsNullOrEmpty(condition.City))
            {
                q = q.Where(a => a.Buildings.BuildingBaseInfo.City == condition.City);
            }
            if (!string.IsNullOrEmpty(condition.Area))
            {
                q = q.Where(a => a.Buildings.BuildingBaseInfo.Area == condition.Area);
            }
            if (!string.IsNullOrEmpty(condition.District))
            {
                q = q.Where(a => a.Buildings.BuildingBaseInfo.District == condition.District);
            }
            if (condition?.LowArea != null)
            {
                q = q.Where(a => a.ShopBaseInfo.BuildingArea >= condition.LowArea);
            }
            if (condition?.HighArea != null)
            {
                q = q.Where(a => a.ShopBaseInfo.BuildingArea <= condition.HighArea);
            }
            if (condition?.LowPrice != null)
            {
                q = q.Where(a => a.ShopBaseInfo.Price >= condition.LowPrice);
            }
            if (condition?.HighPrice != null)
            {
                q = q.Where(a => a.ShopBaseInfo.Price <= condition.HighPrice);
            }
            if (condition?.PriceIsAscSort == null && condition?.AreaIsAscSort == null)
            {
                q = q.OrderBy(a => a.ShopLeaseInfo.StartDate);
            }
            if (condition?.PriceIsAscSort == true)
            {
                q = q.OrderBy(a => (a.ShopBaseInfo.Price ?? 0));
            }
            if (condition?.PriceIsAscSort == false)
            {
                q = q.OrderByDescending(a => (a.ShopBaseInfo.Price ?? 0));
            }
            if (condition?.AreaIsAscSort == true)
            {
                q = q.OrderBy(a => (a.ShopBaseInfo.BuildingArea ?? 0));
            }
            if (condition?.AreaIsAscSort == false)
            {
                q = q.OrderByDescending(a => (a.ShopBaseInfo.BuildingArea ?? 0));
            }
            if (condition?.MinPrice != null)
            {
                q = q.Where(a => (a.ShopBaseInfo.TotalPrice >= condition.MinPrice));
            }
            if (condition?.MaxPrice != null)
            {
                q = q.Where(a => (a.ShopBaseInfo.TotalPrice <= condition.MaxPrice));
            }
            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');

            pagingResponse.TotalCount = await q.CountAsync(cancellationToken);
            var qlist = await q.Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);
            var resulte = qlist.Select(a => new ShopListSearchResponse
            {
                Id = a.Id,
                BuildingId = a.BuildingId,
                BuildingName = a.Buildings.BuildingBaseInfo.Name,
                Address = a.Buildings.BuildingBaseInfo.Address,
                AreaFullName = a.Buildings.BuildingBaseInfo.CityDefine.Name + "-" + a.Buildings.BuildingBaseInfo.DistrictDefine.Name + "-" + a.Buildings.BuildingBaseInfo.AreaDefine.Name,
                Price = a.ShopBaseInfo.Price,
                Name = a.ShopBaseInfo.Name,
                Depth = a.ShopBaseInfo.Depth,
                Height = a.ShopBaseInfo.Height,
                Width = a.ShopBaseInfo.Width,
                BuildingArea = a.ShopBaseInfo.BuildingArea,
                BuildingNo = a.ShopBaseInfo.BuildingNo,
                FloorNo = a.ShopBaseInfo.FloorNo,
                Number = a.ShopBaseInfo.Number,
                SaleStatus = a.ShopBaseInfo.SaleStatus,
                Status = a.ShopBaseInfo.Status,
                Icon = string.IsNullOrEmpty(a.Icon) ? "" : fr + "/" + a.Icon.TrimStart('/'),
                UpperWater = a.ShopFacilities.UpperWater,
                DownWater = a.ShopFacilities.DownWater,
                Gas = a.ShopFacilities.Gas,
                Chimney = a.ShopFacilities.Chimney,
                Blowoff = a.ShopFacilities.Blowoff,
                Split = a.ShopFacilities.Split,
                Elevator = a.ShopFacilities.Elevator,
                Staircase = a.ShopFacilities.Staircase,
                Outside = a.ShopFacilities.Outside,
                OpenFloor = a.ShopFacilities.OpenFloor,
                ParkingSpace = a.ShopFacilities.ParkingSpace,
                IsCorner = a.ShopBaseInfo.IsCorner,
                IsFaceStreet = a.ShopBaseInfo.IsFaceStreet,
                HasFree = a.ShopBaseInfo.HasFree,
                HasStreet = a.ShopBaseInfo.HasStreet,
                IsHot = a.ShopBaseInfo.IsHot,
                TotalPrice = a.ShopBaseInfo.TotalPrice,
                LockTime = a.ShopBaseInfo.LockTime
            });


            if (condition.SequencingConditions != null)
            {
                foreach (var item in condition.SequencingConditions)
                {
                    if (item.Name == "BuildingNo")
                    {
                        resulte = item.Value ? resulte.OrderBy(x => x.BuildingNo).ToList() : resulte.OrderByDescending(x => x.BuildingNo).ToList();
                    }
                    else if (item.Name == "Number")
                    {
                        resulte = item.Value ? resulte.OrderBy(x => x.Number).ToList() : resulte.OrderByDescending(x => x.Number).ToList();
                    }
                }
            }

            pagingResponse.PageIndex = condition.PageIndex;
            pagingResponse.PageSize = condition.PageSize;
            pagingResponse.Extension = resulte.ToList();
            return pagingResponse;
        }

        public virtual async Task<PagingResponseMessage<ShopListSearchResponse>> SearchRecommend(string userId, ShopListSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            PagingResponseMessage<ShopListSearchResponse> pagingResponse = new PagingResponseMessage<ShopListSearchResponse>();

            var q = _shopsStore.GetDetailQuery().Where(a => !a.IsDeleted && a.ExamineStatus == (int)Models.ExamineStatusEnum.Approved);
            if (condition?.BuildingIds?.Count > 0)
            {
                q = q.Where(a => condition.BuildingIds.Contains(a.BuildingId));
            }

            if (!string.IsNullOrEmpty(condition?.CustomerId))
            {
                var shopsid = (await _shopsStore.ListNotShopsAsync(x => x.Where(y => y.CustomerId == condition.CustomerId))).Select(x => x.ShopsId);
                q = q.Where(a => !shopsid.Contains(a.Id));
            }
            if (condition?.SaleStatus?.Count > 0)
            {
                q = q.Where(a => condition.SaleStatus.Contains(a.ShopBaseInfo.SaleStatus));
            }
            if (condition?.UpperWater == true)
            {
                q = q.Where(a => a.ShopFacilities.UpperWater ?? true);
            }
            if (condition?.DownWater == true)
            {
                q = q.Where(a => a.ShopFacilities.DownWater ?? true);
            }
            if (condition?.Gas == true)
            {
                q = q.Where(a => a.ShopFacilities.Gas ?? true);
            }
            if (condition?.Chimney == true)
            {
                q = q.Where(a => a.ShopFacilities.Chimney ?? true);
            }
            if (condition?.Blowoff == true)
            {
                q = q.Where(a => a.ShopFacilities.Blowoff ?? true);
            }
            if (condition?.Split == true)
            {
                q = q.Where(a => a.ShopFacilities.Split ?? true);
            }
            if (condition?.Elevator == true)
            {
                q = q.Where(a => a.ShopFacilities.Elevator ?? true);
            }
            if (condition?.Staircase == true)
            {
                q = q.Where(a => a.ShopFacilities.Staircase ?? true);
            }
            if (condition?.Outside == true)
            {
                q = q.Where(a => a.ShopFacilities.Outside ?? true);
            }
            if (condition?.OpenFloor == true)
            {
                q = q.Where(a => a.ShopFacilities.OpenFloor ?? true);
            }
            if (condition?.ParkingSpace == true)
            {
                q = q.Where(a => a.ShopFacilities.ParkingSpace ?? true);
            }
            if (condition?.IsCorner == true)
            {
                q = q.Where(a => a.ShopBaseInfo.IsCorner ?? true);
            }
            if (condition?.IsFaceStreet == true)
            {
                q = q.Where(a => a.ShopBaseInfo.IsFaceStreet ?? true);
            }
            if (condition?.HasFree == true)
            {
                q = q.Where(a => a.ShopBaseInfo.HasFree ?? true);
            }
            if (condition?.HasStreet == true)
            {
                q = q.Where(a => a.ShopBaseInfo.HasStreet ?? true);
            }
            if (!string.IsNullOrEmpty(condition.KeyWord))
            {
                q = q.Where(a => a.ShopBaseInfo.Name.Contains(condition.KeyWord));
            }
            if (!string.IsNullOrEmpty(condition.City))
            {
                q = q.Where(a => a.Buildings.BuildingBaseInfo.City == condition.City);
            }
            if (!string.IsNullOrEmpty(condition.Area))
            {
                q = q.Where(a => a.Buildings.BuildingBaseInfo.Area == condition.Area);
            }
            if (!string.IsNullOrEmpty(condition.District))
            {
                q = q.Where(a => a.Buildings.BuildingBaseInfo.District == condition.District);
            }
            if (condition?.LowArea != null)
            {
                q = q.Where(a => a.ShopBaseInfo.BuildingArea >= condition.LowArea);
            }
            if (condition?.HighArea != null)
            {
                q = q.Where(a => a.ShopBaseInfo.BuildingArea <= condition.HighArea);
            }
            if (condition?.LowPrice != null)
            {
                q = q.Where(a => a.ShopBaseInfo.TotalPrice >= condition.LowPrice);
            }
            if (condition?.HighPrice != null)
            {
                q = q.Where(a => a.ShopBaseInfo.TotalPrice <= condition.HighPrice);
            }
            if (condition?.PriceIsAscSort == null && condition?.AreaIsAscSort == null)
            {
                q = q.OrderBy(a => a.ShopLeaseInfo.StartDate);
            }
            if (condition?.PriceIsAscSort == true)
            {
                q = q.OrderBy(a => (a.ShopBaseInfo.Price ?? 0));
            }
            if (condition?.PriceIsAscSort == false)
            {
                q = q.OrderByDescending(a => (a.ShopBaseInfo.Price ?? 0));
            }
            if (condition?.AreaIsAscSort == true)
            {
                q = q.OrderBy(a => (a.ShopBaseInfo.BuildingArea ?? 0));
            }
            if (condition?.AreaIsAscSort == false)
            {
                q = q.OrderByDescending(a => (a.ShopBaseInfo.BuildingArea ?? 0));
            }
            if (condition?.MinPrice != null)
            {
                q = q.Where(a => (a.ShopBaseInfo.TotalPrice >= condition.MinPrice));
            }
            if (condition?.MaxPrice != null)
            {
                q = q.Where(a => (a.ShopBaseInfo.TotalPrice <= condition.MaxPrice));
            }
            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');

            pagingResponse.TotalCount = await q.CountAsync(cancellationToken);
            var qlist = await q.Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);
            var resulte = qlist.Select(a => new ShopListSearchResponse
            {
                Id = a.Id,
                BuildingId = a.BuildingId,
                BuildingName = a.Buildings.BuildingBaseInfo.Name,
                Address = a.Buildings.BuildingBaseInfo.Address,
                AreaFullName = a.Buildings.BuildingBaseInfo.CityDefine.Name + "-" + a.Buildings.BuildingBaseInfo.DistrictDefine.Name + "-" + a.Buildings.BuildingBaseInfo.AreaDefine.Name,
                Price = a.ShopBaseInfo.Price,
                Name = a.ShopBaseInfo.Name,
                Depth = a.ShopBaseInfo.Depth,
                Height = a.ShopBaseInfo.Height,
                Width = a.ShopBaseInfo.Width,
                BuildingArea = a.ShopBaseInfo.BuildingArea,
                BuildingNo = a.ShopBaseInfo.BuildingNo,
                FloorNo = a.ShopBaseInfo.FloorNo,
                Number = a.ShopBaseInfo.Number,
                SaleStatus = a.ShopBaseInfo.SaleStatus,
                Status = a.ShopBaseInfo.Status,
                Icon = string.IsNullOrEmpty(a.Icon) ? "" : fr + "/" + a.Icon.TrimStart('/'),
                UpperWater = a.ShopFacilities.UpperWater,
                DownWater = a.ShopFacilities.DownWater,
                Gas = a.ShopFacilities.Gas,
                Chimney = a.ShopFacilities.Chimney,
                Blowoff = a.ShopFacilities.Blowoff,
                Split = a.ShopFacilities.Split,
                Elevator = a.ShopFacilities.Elevator,
                Staircase = a.ShopFacilities.Staircase,
                Outside = a.ShopFacilities.Outside,
                OpenFloor = a.ShopFacilities.OpenFloor,
                ParkingSpace = a.ShopFacilities.ParkingSpace,
                IsCorner = a.ShopBaseInfo.IsCorner,
                IsFaceStreet = a.ShopBaseInfo.IsFaceStreet,
                HasFree = a.ShopBaseInfo.HasFree,
                HasStreet = a.ShopBaseInfo.HasStreet,
                IsHot = a.ShopBaseInfo.IsHot,
                TotalPrice = a.ShopBaseInfo.TotalPrice,
                LockTime = a.ShopBaseInfo.LockTime
            });


            if (condition.SequencingConditions != null)
            {
                foreach (var item in condition.SequencingConditions)
                {
                    if (item.Name == "BuildingNo")
                    {
                        resulte = item.Value ? resulte.OrderBy(x => x.BuildingNo).ToList() : resulte.OrderByDescending(x => x.BuildingNo).ToList();
                    }
                    else if (item.Name == "Number")
                    {
                        resulte = item.Value ? resulte.OrderBy(x => x.Number).ToList() : resulte.OrderByDescending(x => x.Number).ToList();
                    }
                }
            }

            pagingResponse.PageIndex = condition.PageIndex;
            pagingResponse.PageSize = condition.PageSize;
            pagingResponse.Extension = resulte.ToList();
            return pagingResponse;
        }

        public virtual async Task<PagingResponseMessage<ShopInfoResponse>> SimpleSearch(UserInfo user, ShopSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            PagingResponseMessage<ShopInfoResponse> pagingResponse = new PagingResponseMessage<ShopInfoResponse>();
            pagingResponse.Extension = new List<ShopInfoResponse>();
            var q = _shopsStore.GetSimpleQuery();
            q = q.Where(x => !x.IsDeleted);

            if (condition != null)
            {
                if (condition.BuildingIds != null && condition.BuildingIds.Count > 0)
                {
                    q = q.Where(s => condition.BuildingIds.Contains(s.BuildingId));
                }

                if (condition.ExamineStatus != null && condition.ExamineStatus.Count > 0)
                {
                    q = q.Where(s => condition.ExamineStatus.Contains(s.ExamineStatus));
                }
                if (condition.SaleStatus != null && condition.SaleStatus != null)
                {
                    q = q.Where(s => condition.SaleStatus.Contains(s.ShopBaseInfo.SaleStatus));
                }
            }
            q = q.OrderBy(x => x.ExamineStatus).ThenByDescending(x => x.CreateTime);
            pagingResponse.TotalCount = await q.CountAsync(cancellationToken);
            var result = await q.Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);
            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');
            if (result.Count > 0)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    result[i].Icon = string.IsNullOrEmpty(result[i].Icon) ? "" : fr + "/" + result[i].Icon.TrimStart('/');
                }
                pagingResponse.Extension.AddRange(_mapper.Map<List<ShopInfoResponse>>(result));
            }
            return pagingResponse;
        }


        /// <summary>
        /// 销控统计
        /// </summary>
        /// <param name="buildingid"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<SaleStatisticsResponse>> SaleStatistics(string buildingid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var Response = new List<SaleStatisticsResponse>();

            var res = await _shopsStore.GetSimpleQuery().Where(x => !x.IsDeleted && x.BuildingId == buildingid && x.ExamineStatus == (int)Models.ExamineStatusEnum.Approved).ToListAsync(cancellationToken);

            var resulte = from a in res select a.ShopBaseInfo.SaleStatus;


            //    res.Select(a => new ShopListSearchResponse
            //{
            //    SaleStatus = a.ShopBaseInfo.SaleStatus
            //});


            var query = resulte.GroupBy(x => x);
            foreach (var item in query)
            {
                Response.Add(new SaleStatisticsResponse
                {
                    Name = item.Key,
                    Value = item.Count()
                });
            }
            return Response;
        }


        public virtual async Task DeleteAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _shopsStore.DeleteAsync(_mapper.Map<SimpleUser>(user), new Shops() { Id = id }, cancellationToken);
            //var shops = await _shopsStore.GetAsync(a => a.Where(b => b.Id == id && !b.IsDeleted), cancellationToken);
            //if (shops == null)
            //{
            //    return;
            //}
            //shops.IsDeleted = true;
            //shops.DeleteTime = DateTime.Now;
            //shops.DeleteUser = userId;
            //await _shopsStore.UpdateAsync(shops, cancellationToken);
        }


        public virtual async Task<Hashtable> GetSaleStatusGroupCount(string userid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = new Hashtable();

            var shops = _shopBaseInfoStore.ShopBaseInfos;

            var query = from p in shops
                        group p by p.SaleStatus into g
                        select new
                        {
                            g.Key,
                            count = g.Count()
                        };
            var ss = await query.ToListAsync(cancellationToken);
            return response;
        }

        public virtual async Task DeleteListAsync(string userId, List<string> ids, CancellationToken cancellationToken = default(CancellationToken))
        {
            var shops = await _shopsStore.ListAsync(a => a.Where(b => ids.Contains(b.Id) && !b.IsDeleted), cancellationToken);
            if (shops == null || shops.Count == 0)
            {
                return;
            }
            for (int i = 0; i < shops.Count; i++)
            {
                shops[i].DeleteUser = userId;
                shops[i].DeleteTime = DateTime.Now;
                shops[i].IsDeleted = true;
            }
            await _shopsStore.UpdateListAsync(shops, cancellationToken);
        }

        /// <summary>
        /// 修改商铺在售状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="PutSaleStatus"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateSaleStatusAsync(string userId, SaleShopsStatusRquest PutSaleStatus, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (PutSaleStatus == null)
            {
                throw new ArgumentNullException(nameof(PutSaleStatus));
            }
            var shopbases = await _shopBaseInfoStore.GetSimpleShopBase().Where(b => PutSaleStatus.ShopsIds.Contains(b.Id)).ToListAsync(cancellationToken);
            if (shopbases == null)
            {
                return false;
            }
            var sellhistories = new List<SellHistory>();

            shopbases = shopbases.Select(x =>
            {
                x.SaleStatus = PutSaleStatus.SaleStatus;
                x.LockTime = PutSaleStatus.LockTime;
                sellhistories.Add(new SellHistory
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    BuildingId = x.BuildingId,
                    CreateTime = DateTime.Now,
                    CreateUser = userId,
                    SaleStatus = PutSaleStatus.SaleStatus,
                    ShopsId = x.Id,
                    LockTime = PutSaleStatus.LockTime,
                    Mark = PutSaleStatus.Mark,
                });
                return x;
            }).ToList();
            await _shopBaseInfoStore.UpdateSaleStatusAsync(shopbases, sellhistories, cancellationToken);
            return true;
        }

        public virtual async Task UpdateAsync(string userId, string id, ShopsRequest shopsRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopsRequest == null)
            {
                throw new ArgumentNullException(nameof(shopsRequest));
            }
            var building = await _shopsStore.GetAsync(a => a.Where(b => b.Id == id && !b.IsDeleted));
            if (building == null)
            {
                return;
            }
            var newShops = _mapper.Map<Shops>(shopsRequest);
            newShops.IsDeleted = building.IsDeleted;
            newShops.CreateTime = building.CreateTime;
            newShops.CreateUser = building.CreateUser;
            newShops.UpdateTime = DateTime.Now;
            newShops.UpdateUser = userId;
            await _shopsStore.UpdateAsync(newShops, cancellationToken);
        }


        public virtual async Task SaveSummaryAsync(UserInfo user, string buildingId, string shopId, string summary, string source, string sourceId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await _shopsStore.SaveSummaryAsync(_mapper.Map<SimpleUser>(user), buildingId, shopId, summary, source, sourceId, cancellationToken);
        }

        public virtual async Task SubmitAsync(string shopsId, Dto.ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _shopsStore.UpdateExamineStatus(shopsId, (Models.ExamineStatusEnum)(int)status, cancellationToken);
        }

        /// <summary>
        /// 新增用户不感兴趣商铺
        /// </summary>
        /// <param name="user"></param>
        /// <param name="customerNotShops"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<ResponseMessage<CustomerNotShops>> CreateCustomerNotShopAsync(UserInfo user, CustomerNotShops customerNotShops, CancellationToken cancellationToken = default(CancellationToken))
        {

            var response = new ResponseMessage<CustomerNotShops>();

            if (customerNotShops == null)
            {
                throw new ArgumentNullException(nameof(customerNotShops));
            }
            response.Extension = await _shopsStore.CreateCustomerNotShopAsync(customerNotShops, cancellationToken);


            return response;
        }

        public virtual async Task<PagingResponseMessage<CustomerNotShops>> GetCustomerNotShopAsync(UserInfo user, CancellationToken cancellationToken = default(CancellationToken))
        {

            var response = new PagingResponseMessage<CustomerNotShops>();


            response.Extension = await _shopsStore.ListNotShopsAsync<CustomerNotShops>((query) =>
           {
               query = query.Where(x => x.UserId == user.Id);
               return query;
           }, cancellationToken);


            return response;
        }
    }
}
