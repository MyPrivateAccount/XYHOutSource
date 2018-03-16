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
    public class BuildingFileScopeStore : IBuildingFileScopeStore
    {
        public BuildingFileScopeStore(ShopsDbContext shopsDbContext)
        {
            Context = shopsDbContext;
            BuildingFileScopes = Context.BuildingFileScopes;
        }
        protected ShopsDbContext Context { get; }
        public IQueryable<BuildingFileScope> BuildingFileScopes { get; set; }

        public async Task<BuildingFileScope> CreateAsync(BuildingFileScope buildingFileScope, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingFileScope == null)
            {
                throw new ArgumentNullException(nameof(buildingFileScope));
            }
            Context.Add(buildingFileScope);
            await Context.SaveChangesAsync(cancellationToken);
            return buildingFileScope;
        }


        public async Task DeleteAsync(BuildingFileScope buildingFileScope, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingFileScope == null)
            {
                throw new ArgumentNullException(nameof(buildingFileScope));
            }
            Context.Remove(buildingFileScope);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<BuildingFileScope> buildingFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingFileScopeList == null)
            {
                throw new ArgumentNullException(nameof(buildingFileScopeList));
            }
            Context.RemoveRange(buildingFileScopeList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BuildingFileScopes).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BuildingFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BuildingFileScopes).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(BuildingFileScope shopsFileScope, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopsFileScope == null)
            {
                throw new ArgumentNullException(nameof(shopsFileScope));
            }
            Context.Attach(shopsFileScope);
            Context.Update(shopsFileScope);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task UpdateListAsync(List<BuildingFileScope> buildingFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingFileScopeList == null)
            {
                throw new ArgumentNullException(nameof(buildingFileScopeList));
            }
            Context.AttachRange(buildingFileScopeList);
            Context.UpdateRange(buildingFileScopeList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public async Task SaveAsync(SimpleUser user, string buildingId, List<BuildingFileScope> shopsFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (shopsFileScopeList == null || shopsFileScopeList.Count == 0)
                return;

            foreach (BuildingFileScope file in shopsFileScopeList)
            {
                if (String.IsNullOrEmpty(file.FileGuid))
                {
                    file.FileGuid = Guid.NewGuid().ToString("N").ToLower();
                }
                //查看楼盘是否存在
                if (!Context.Buildings.Any(x => x.Id == file.BuildingId))
                {
                    Buildings buildings = new Buildings()
                    {
                        Id = file.BuildingId,
                        CreateUser = user.Id,
                        ResidentUser1 = user.Id,
                        CreateTime = DateTime.Now,
                        OrganizationId = user.OrganizationId,
                        ExamineStatus = 0
                    };
                    Context.Add(buildings);
                }
                //基本信息
                if (!Context.BuildingFileScopes.Any(x => x.FileGuid == file.FileGuid))
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
