using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Models;
using XYHShopsPlugin.Stores;

namespace XYHShopsPlugin.Managers
{
    public class BuildingTimingManager
    {
        #region 成员
        protected IBuildingRecommendStore _ibuildingRecommendStore { get; }

        protected IShopBaseInfoStore _ishopBaseInfoStore { get; }

        protected IShopsStore _ishopsStore { get; }

        protected IMapper _mapper { get; }

        #endregion

        public BuildingTimingManager(
            IShopsStore shopsStore,
             IShopBaseInfoStore shopBaseInfoStore,
            IBuildingRecommendStore ibuildingRecommendStore,
            IMapper mapper)
        {
            _ishopsStore = shopsStore ?? throw new ArgumentNullException(nameof(shopsStore));
            _ishopBaseInfoStore = shopBaseInfoStore ?? throw new ArgumentNullException(nameof(shopBaseInfoStore));
            _ibuildingRecommendStore = ibuildingRecommendStore ?? throw new ArgumentNullException(nameof(ibuildingRecommendStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 定时任务删除楼盘推荐
        /// </summary>
        /// <returns></returns>
        public virtual async Task TimedTaskDeletedBuildingRecommend(CancellationToken cancellationToken = default(CancellationToken))
        {
            var buildingRecommends = await _ibuildingRecommendStore.ListAsync(a => a.Where(b => !b.IsDeleted), cancellationToken);
            buildingRecommends = buildingRecommends.Where(b => b.RecommendTime.AddDays(b.RecommendDays) > DateTime.Now).ToList();
            if (buildingRecommends == null) return;


            buildingRecommends = buildingRecommends.Select(q =>
            {
                q.IsOutDate = true;
                q.UpdateTime = DateTime.Now;
                q.UpdateUser = "定时任务删除";

                return q;
            }).ToList();
            await _ibuildingRecommendStore.UpdateListAsync(buildingRecommends, cancellationToken);
        }

        /// <summary>
        /// 定时任务修改商铺销售状态
        /// </summary>
        /// <returns></returns>
        public virtual async Task TimedTaskLockShops(CancellationToken cancellationToken = default(CancellationToken))
        {
            var shopbaseinfos = await _ishopBaseInfoStore.ListAsync(x => x.Where(y => y.LockTime.Value.Date == DateTime.Now.Date && y.SaleStatus == "3"));
            var sellhistories = new List<SellHistory>();
            shopbaseinfos = shopbaseinfos.Select(x =>
            {
                x.LockTime = null;
                x.SaleStatus = "2";
                sellhistories.Add(new SellHistory
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = "定时任务",
                    BuildingId = x.BuildingId,
                    CreateTime = DateTime.Now,
                    CreateUser = "定时任务",
                    SaleStatus = "2",
                    ShopsId = x.Id,
                    LockTime = null,
                    Mark = "定时任务更改商铺状态"
                });


                return x;
            }).ToList();

            await _ishopBaseInfoStore.UpdateSaleStatusAsync(shopbaseinfos, sellhistories, cancellationToken);
        }
    }
}
