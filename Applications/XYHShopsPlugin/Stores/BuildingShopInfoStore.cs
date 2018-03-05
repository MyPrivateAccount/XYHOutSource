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
    public class BuildingShopInfoStore : IBuildingShopInfoStore
    {
        public BuildingShopInfoStore(ShopsDbContext baseDataDbContext)
        {
            Context = baseDataDbContext;
            BuildingShopInfos = Context.BuildingShopInfos;
        }

        protected ShopsDbContext Context { get; }

        public IQueryable<BuildingShopInfo> BuildingShopInfos { get; set; }


        public async Task<BuildingShopInfo> CreateAsync(BuildingShopInfo buildingShopInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingShopInfo == null)
            {
                throw new ArgumentNullException(nameof(buildingShopInfo));
            }
            Context.Add(buildingShopInfo);
            await Context.SaveChangesAsync(cancellationToken);
            return buildingShopInfo;
        }


        public async Task DeleteAsync(BuildingShopInfo buildingShopInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingShopInfo == null)
            {
                throw new ArgumentNullException(nameof(buildingShopInfo));
            }
            Context.Remove(buildingShopInfo);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<BuildingShopInfo> buildingShopInfoList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingShopInfoList == null)
            {
                throw new ArgumentNullException(nameof(buildingShopInfoList));
            }
            Context.RemoveRange(buildingShopInfoList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingShopInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BuildingShopInfos).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BuildingShopInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BuildingShopInfos).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(BuildingShopInfo buildingShopInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingShopInfo == null)
            {
                throw new ArgumentNullException(nameof(buildingShopInfo));
            }
            Context.Attach(buildingShopInfo);
            Context.Update(buildingShopInfo);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task UpdateListAsync(List<BuildingShopInfo> buildingShopInfoList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingShopInfoList == null)
            {
                throw new ArgumentNullException(nameof(buildingShopInfoList));
            }
            Context.AttachRange(buildingShopInfoList);
            Context.UpdateRange(buildingShopInfoList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }


        public async Task SaveAsync(SimpleUser user, BuildingShopInfo buildingShopInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (buildingShopInfo == null)
            {
                throw new ArgumentNullException(nameof(buildingShopInfo));
            }

            //查看楼盘是否存在
            if (!Context.Buildings.Any(x => x.Id == buildingShopInfo.Id))
            {
                Buildings buildings = new Buildings()
                {
                    Id = buildingShopInfo.Id,
                    CreateUser = user.Id,
                    ResidentUser1 = user.Id,
                    CreateTime = DateTime.Now,
                    OrganizationId = user.OrganizationId,
                    ExamineStatus = 0
                };

                Context.Add(buildings);
            }

            List<string> tradePlannings = new List<string>();
            //写业态到独立表
            if (!String.IsNullOrEmpty(buildingShopInfo.TradeMixPlanning))
            {
                tradePlannings = buildingShopInfo.TradeMixPlanning.Split(',').Where(x => !String.IsNullOrWhiteSpace(x)).ToList();
            }
            //基本信息
            if (!Context.BuildingShopInfos.Any(x => x.Id == buildingShopInfo.Id))
            {
                Context.Add(buildingShopInfo);
                if (tradePlannings.Count > 0)
                {
                    tradePlannings.ForEach(x =>
                    {
                        Context.Add(new BuildingTradeMixPlanning()
                        {
                            Id = buildingShopInfo.Id,
                            TradeMixPlanning = x
                        });
                    });
                }
                       
            }
            else
            {
                Context.Attach(buildingShopInfo);
                Context.Update(buildingShopInfo);

                var list = await Context.BuildingTradeMixPlanning.Where(x => x.Id == buildingShopInfo.Id).ToListAsync();
                tradePlannings.Where(x => !list.Any(y => y.TradeMixPlanning == x)).ToList().ForEach(t =>
                   {
                       Context.Add(new BuildingTradeMixPlanning() { Id = buildingShopInfo.Id, TradeMixPlanning = t });
                   });
                list.Where(x => !tradePlannings.Any(y => y == x.TradeMixPlanning)).ToList().ForEach(t =>
                {
                    Context.Remove(t);
                });
            }
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

    }
}
