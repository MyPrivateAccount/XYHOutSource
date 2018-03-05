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
    public class BuildingBaseInfoStore : IBuildingBaseInfoStore
    {
        public BuildingBaseInfoStore(ShopsDbContext baseDataDbContext)
        {
            Context = baseDataDbContext;
            BuildingBaseInfos = Context.BuildingBaseInfos;
        }

        protected ShopsDbContext Context { get; }

        public IQueryable<BuildingBaseInfo> BuildingBaseInfos { get; set; }


        public async Task<BuildingBaseInfo> CreateAsync(BuildingBaseInfo buildingBaseInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfo == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfo));
            }
            Context.Add(buildingBaseInfo);
            await Context.SaveChangesAsync(cancellationToken);
            return buildingBaseInfo;
        }


        public async Task DeleteAsync(BuildingBaseInfo buildingBaseInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfo == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfo));
            }
            Context.Remove(buildingBaseInfo);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<BuildingBaseInfo> buildingBaseInfoList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfoList == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoList));
            }
            Context.RemoveRange(buildingBaseInfoList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingBaseInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BuildingBaseInfos).SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> CheckDuplicateBuilding(string buildingId, string name, string city, string district, string area, CancellationToken cancellationToken = default(CancellationToken))
        {
            var q = from b in Context.Buildings.AsNoTracking()
                    join bi in Context.BuildingBaseInfos.AsNoTracking() on b.Id equals bi.Id
                    where b.Id != buildingId && b.IsDeleted == false && bi.Name == name && bi.City == city && bi.District == district && bi.Area == area
                    select b;
            int count = await q.CountAsync();

            return count > 0;

        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BuildingBaseInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BuildingBaseInfos).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(BuildingBaseInfo buildingBaseInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfo == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfo));
            }
            Context.Attach(buildingBaseInfo);
            Context.Update(buildingBaseInfo);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task UpdateListAsync(List<BuildingBaseInfo> buildingBaseInfoList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfoList == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoList));
            }
            Context.AttachRange(buildingBaseInfoList);
            Context.UpdateRange(buildingBaseInfoList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }


        public async Task SaveAsync(SimpleUser user, BuildingBaseInfo buildingBaseInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (buildingBaseInfo == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfo));
            }

            //查看楼盘是否存在
            if (!Context.Buildings.Any(x => x.Id == buildingBaseInfo.Id))
            {
                Buildings buildings = new Buildings()
                {
                    Id = buildingBaseInfo.Id,
                    CreateUser = user.Id,
                    ResidentUser1 = user.Id,
                    CreateTime = DateTime.Now,
                    OrganizationId = user.OrganizationId,
                    ExamineStatus = 0
                };

                Context.Add(buildings);
            }
            //基本信息
            if (!Context.BuildingBaseInfos.Any(x => x.Id == buildingBaseInfo.Id))
            {
                Context.Add(buildingBaseInfo);
            }
            else
            {
                Context.Attach(buildingBaseInfo);
                Context.Update(buildingBaseInfo);
            }
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

    }
}
