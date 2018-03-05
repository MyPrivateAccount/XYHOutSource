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
    public class ShopsFileScopeStore : IShopsFileScopeStore
    {
        public ShopsFileScopeStore(ShopsDbContext baseDataDbContext)
        {
            Context = baseDataDbContext;
            ShopsFileScopes = Context.ShopsFileScopes;
        }

        protected ShopsDbContext Context { get; }

        public IQueryable<ShopsFileScope> ShopsFileScopes { get; set; }


        public async Task<ShopsFileScope> CreateAsync(ShopsFileScope shopsFileScope, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopsFileScope == null)
            {
                throw new ArgumentNullException(nameof(shopsFileScope));
            }
            Context.Add(shopsFileScope);
            await Context.SaveChangesAsync(cancellationToken);
            return shopsFileScope;
        }


        public async Task DeleteAsync(ShopsFileScope shopsFileScope, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopsFileScope == null)
            {
                throw new ArgumentNullException(nameof(shopsFileScope));
            }
            Context.Remove(shopsFileScope);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<ShopsFileScope> shopsFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopsFileScopeList == null)
            {
                throw new ArgumentNullException(nameof(shopsFileScopeList));
            }
            Context.RemoveRange(shopsFileScopeList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<ShopsFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ShopsFileScopes).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<ShopsFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ShopsFileScopes).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(ShopsFileScope shopsFileScope, CancellationToken cancellationToken = default(CancellationToken))
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

        public async Task UpdateListAsync(List<ShopsFileScope> shopsFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopsFileScopeList == null)
            {
                throw new ArgumentNullException(nameof(shopsFileScopeList));
            }
            Context.AttachRange(shopsFileScopeList);
            Context.UpdateRange(shopsFileScopeList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public async Task SaveAsync(SimpleUser user, string buildingId, List<ShopsFileScope> shopsFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (shopsFileScopeList == null || shopsFileScopeList.Count == 0)
                return;

            foreach (ShopsFileScope file in shopsFileScopeList)
            {
                if (String.IsNullOrEmpty(file.FileGuid))
                {
                    file.FileGuid = Guid.NewGuid().ToString("N").ToLower();
                }
                //查看商铺是否存在
                if (!Context.Shops.Any(x => x.Id == file.ShopsId))
                {
                    Shops shops = new Shops()
                    {
                        Id = file.ShopsId,
                        BuildingId = buildingId,
                        CreateUser = user.Id,
                        CreateTime = DateTime.Now,
                        OrganizationId = user.OrganizationId,
                        ExamineStatus = 0
                    };
                    Context.Add(shops);
                }
                //基本信息
                if (!Context.ShopsFileScopes.Any(x => x.FileGuid == file.FileGuid))
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
