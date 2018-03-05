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
    public class BuildingNoticeFileScopeStore : IBuildingNoticeFileScopeStore
    {
        public BuildingNoticeFileScopeStore(ShopsDbContext shopsDbContext)
        {
            Context = shopsDbContext;
            BuildingNoticeFileScopes = Context.BuildingNoticeFileScopes;
        }

        protected ShopsDbContext Context { get; }

        public IQueryable<BuildingNoticeFileScope> BuildingNoticeFileScopes { get; set; }


        public async Task<BuildingNoticeFileScope> CreateAsync(BuildingNoticeFileScope buildingNoticeFileScope, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingNoticeFileScope == null)
            {
                throw new ArgumentNullException(nameof(buildingNoticeFileScope));
            }
            Context.Add(buildingNoticeFileScope);
            await Context.SaveChangesAsync(cancellationToken);
            return buildingNoticeFileScope;
        }


        public async Task DeleteAsync(BuildingNoticeFileScope buildingNoticeFileScope, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingNoticeFileScope == null)
            {
                throw new ArgumentNullException(nameof(buildingNoticeFileScope));
            }
            Context.Remove(buildingNoticeFileScope);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<BuildingNoticeFileScope> buildingNoticeFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingNoticeFileScopeList == null)
            {
                throw new ArgumentNullException(nameof(buildingNoticeFileScopeList));
            }
            Context.RemoveRange(buildingNoticeFileScopeList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingNoticeFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BuildingNoticeFileScopes).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BuildingNoticeFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BuildingNoticeFileScopes).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(BuildingNoticeFileScope buildingNoticeFileScope, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingNoticeFileScope == null)
            {
                throw new ArgumentNullException(nameof(buildingNoticeFileScope));
            }
            Context.Attach(buildingNoticeFileScope);
            Context.Update(buildingNoticeFileScope);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task UpdateListAsync(List<BuildingNoticeFileScope> buildingNoticeFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingNoticeFileScopeList == null)
            {
                throw new ArgumentNullException(nameof(buildingNoticeFileScopeList));
            }
            Context.AttachRange(buildingNoticeFileScopeList);
            Context.UpdateRange(buildingNoticeFileScopeList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public async Task SaveAsync(SimpleUser user, string buildingId, List<BuildingNoticeFileScope> buildingNoticeFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (buildingNoticeFileScopeList == null || buildingNoticeFileScopeList.Count == 0)
                return;

            foreach (BuildingNoticeFileScope file in buildingNoticeFileScopeList)
            {
                if (String.IsNullOrEmpty(file.FileGuid))
                {
                    file.FileGuid = Guid.NewGuid().ToString("N").ToLower();
                }
                //查看商铺是否存在
                if (!Context.Shops.Any(x => x.Id == file.BuildingNoticeId))
                {
                    BuildingNotice shops = new BuildingNotice()
                    {
                        Id = file.BuildingNoticeId,
                        //BuildingId = buildingId,
                        UserId = user.Id,
                        CreateTime = DateTime.Now,
                        OrganizationId = user.OrganizationId,
                        IsDeleted = false
                    };
                    Context.Add(shops);
                }
                //基本信息
                if (!Context.BuildingNoticeFileScopes.Any(x => x.FileGuid == file.FileGuid))
                {
                    file.CreateTime = DateTime.Now;
                    file.CreateUser = user.Id;
                    Context.Add(file);
                }
                else
                {
                    Context.Attach(file);
                    Context.Update(file);
                }
            }
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

        }
    }
}
