using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
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
    public class ShopsFavoriteManager
    {
        #region 成员

        protected IShopsFavoriteStore _ishopsFavoritesStore { get; }

        protected IShopsStore _iShopsStore { get; }

        protected IMapper _mapper { get; }

        #endregion

        public ShopsFavoriteManager(
            IShopsFavoriteStore ishopsFavoritesStore,
            IShopsStore iShopsStore,
            IMapper mapper)
        {
            _ishopsFavoritesStore = ishopsFavoritesStore ?? throw new ArgumentNullException(nameof(ishopsFavoritesStore));
            _iShopsStore = iShopsStore ?? throw new ArgumentNullException(nameof(iShopsStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 新增商铺收藏信息
        /// </summary>
        /// <param name="userId">创建者</param>
        /// <param name="shopsFavoriteResponse">请求实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<ShopsFavoriteResponse> CreateAsync(UserInfo user, ShopsFavoriteRequest shopsFavoriteRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopsFavoriteRequest == null)
            {
                throw new ArgumentNullException(nameof(shopsFavoriteRequest));
            }
            var shopsFavorite = _mapper.Map<ShopsFavorite>(shopsFavoriteRequest);

            shopsFavorite.Id = Guid.NewGuid().ToString();
            shopsFavorite.CreateUser = user.Id;
            shopsFavorite.CreateTime = DateTime.Now;
            shopsFavorite.UserId = user.Id;
            try
            {
                await _ishopsFavoritesStore.CreateAsync(shopsFavorite, cancellationToken);
            }
            catch
            {
            }
            return _mapper.Map<ShopsFavoriteResponse>(shopsFavorite);
        }

        /// <summary>
        /// 根据UserId、商铺Id获取商铺收藏信息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="shopid"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<ShopsFavoriteResponse> FindByUserIdAndShopsIdAsync(string userid, string shopid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _ishopsFavoritesStore.GetAsync(a => a.Where(b => b.UserId == userid && b.ShopsId == shopid && !b.IsDeleted), cancellationToken);
            return _mapper.Map<ShopsFavoriteResponse>(response);
        }

        /// <summary>
        /// 根据Id获取商铺收藏信息
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<ShopsFavoriteResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _ishopsFavoritesStore.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<ShopsFavoriteResponse>(response);
        }

        /// <summary>
        /// 获取我的商铺收藏信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="condition"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<ShopsFavoriteResponse>> FindMyShopsFavoriteAsync(string userId, PageCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            var resulte = new PagingResponseMessage<ShopsFavoriteResponse>();

            var response = _ishopsFavoritesStore.ShopsFavoriteAll().Where(b => b.UserId == userId && !b.IsDeleted && !b.Shops.IsDeleted);

            if (!string.IsNullOrEmpty(condition.KeyWord))
            {
                response = response.Where(x => x.Shops.ShopBaseInfo.Name.Contains(condition.KeyWord));
            }

            var query = await response.OrderByDescending(x => x.FavoriteTime).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);

            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');


            var aasd = query.Select(a => new ShopsFavoriteResponse
            {
                Id = a.Id,
                ShopsId = a.ShopsId,
                UserId = a.UserId,
                FavoriteTime = a.FavoriteTime,
                UserNikeName = a.UserNikeName,
                ShopListSearchResponse = new ShopListSearchResponse
                {
                    Id = a.Shops.Id,
                    Address = a.Shops.Buildings.BuildingBaseInfo.Address,
                    AreaFullName = a.Shops.Buildings.BuildingBaseInfo.CityDefine.Name + "-" + a.Shops.Buildings.BuildingBaseInfo.DistrictDefine.Name + "-" + a.Shops.Buildings.BuildingBaseInfo.AreaDefine.Name,
                    Price = a.Shops.ShopBaseInfo.Price,
                    Name = a.Shops.ShopBaseInfo.Name,
                    Depth = a.Shops.ShopBaseInfo.Depth,
                    Height = a.Shops.ShopBaseInfo.Height,
                    Width = a.Shops.ShopBaseInfo.Width,
                    BuildingArea = a.Shops.ShopBaseInfo.BuildingArea,
                    BuildingName = a.Shops.Buildings.BuildingBaseInfo.Name,
                    BuildingNo = a.Shops.ShopBaseInfo.BuildingNo,
                    FloorNo = a.Shops.ShopBaseInfo.FloorNo,
                    Number = a.Shops.ShopBaseInfo.Number,
                    SaleStatus = a.Shops.ShopBaseInfo.SaleStatus,
                    Status = a.Shops.ShopBaseInfo.Status,
                    Icon = string.IsNullOrEmpty(a.Shops.Icon) ? "" : fr + "/" + a.Shops.Icon.TrimStart('/'),
                    UpperWater = a.Shops.ShopFacilities.UpperWater,
                    DownWater = a.Shops.ShopFacilities.DownWater,
                    Gas = a.Shops.ShopFacilities.Gas,
                    Chimney = a.Shops.ShopFacilities.Chimney,
                    Blowoff = a.Shops.ShopFacilities.Blowoff,
                    Split = a.Shops.ShopFacilities.Split,
                    Elevator = a.Shops.ShopFacilities.Elevator,
                    Staircase = a.Shops.ShopFacilities.Staircase,
                    Outside = a.Shops.ShopFacilities.Outside,
                    OpenFloor = a.Shops.ShopFacilities.OpenFloor,
                    ParkingSpace = a.Shops.ShopFacilities.ParkingSpace,
                    IsCorner = a.Shops.ShopBaseInfo.IsCorner,
                    IsFaceStreet = a.Shops.ShopBaseInfo.IsFaceStreet,
                    HasFree = a.Shops.ShopBaseInfo.HasFree,
                    HasStreet = a.Shops.ShopBaseInfo.HasStreet
                }
            });


            resulte.PageIndex = condition.PageIndex;
            resulte.PageSize = condition.PageSize;
            resulte.TotalCount = await response.CountAsync(cancellationToken);
            resulte.Extension = aasd.ToList();
            return resulte;
        }

        /// <summary>
        /// 修改单个商铺收藏信息
        /// </summary>
        /// <param name="userId">请求者Id</param>
        /// <param name="shopsFavoriteRequest">请求数据</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(string userId, ShopsFavoriteRequest shopsFavoriteRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopsFavoriteRequest == null)
            {
                throw new ArgumentNullException(nameof(shopsFavoriteRequest));
            }
            var shopsFavorite = await _ishopsFavoritesStore.GetAsync(a => a.Where(b => b.Id == shopsFavoriteRequest.Id && !b.IsDeleted));
            if (shopsFavorite == null)
            {
                return;
            }
            var newshopsFavorite = _mapper.Map<ShopsFavorite>(shopsFavoriteRequest);

            newshopsFavorite.IsDeleted = shopsFavorite.IsDeleted;
            newshopsFavorite.CreateTime = shopsFavorite.CreateTime;
            newshopsFavorite.CreateUser = shopsFavorite.CreateUser;
            newshopsFavorite.UpdateTime = DateTime.Now;
            newshopsFavorite.UpdateUser = userId;
            newshopsFavorite.UserId = shopsFavorite.UserId;
            try
            {
                await _ishopsFavoritesStore.UpdateAsync(newshopsFavorite, cancellationToken);
            }
            catch { }
        }

        /// <summary>
        /// 删除商铺收藏信息
        /// </summary>
        /// <param name="user">删除人基本信息</param>
        /// <param name="id">删除商铺收藏Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await _ishopsFavoritesStore.DeleteAsync(_mapper.Map<SimpleUser>(user), await _ishopsFavoritesStore.GetAsync(x => x.Where(y => y.ShopsId == id && y.UserId == user.Id && !y.IsDeleted)));
            }
            catch { }
        }

        /// <summary>
        /// 批量删除商铺收藏信息
        /// </summary>
        /// <param name="userId">删除人Id</param>
        /// <param name="ids">删除Id数组</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task DeleteListAsync(string userId, List<string> ids, CancellationToken cancellationToken = default(CancellationToken))
        {
            var shopsFavorite = await _ishopsFavoritesStore.ListAsync(a => a.Where(b => ids.Contains(b.Id) && !b.IsDeleted), cancellationToken);
            if (shopsFavorite == null || shopsFavorite.Count == 0)
            {
                return;
            }
            for (int i = 0; i < shopsFavorite.Count; i++)
            {
                shopsFavorite[i].DeleteUser = userId;
                shopsFavorite[i].DeleteTime = DateTime.Now;
                shopsFavorite[i].IsDeleted = true;
            }
            try
            {
                await _ishopsFavoritesStore.UpdateListAsync(shopsFavorite, cancellationToken);
            }
            catch { }
        }
    }
}
