using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationCore.Stores
{
    public class PermissionExpansionStore<TContext> : IPermissionExpansionStore where TContext : CoreDbContext
    {
        public PermissionExpansionStore(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected virtual TContext Context { get; }

        public async Task<PermissionExpansion> CreateAsync(PermissionExpansion permissionExpansion)
        {
            if (permissionExpansion == null)
            {
                throw new ArgumentNullException(nameof(permissionExpansion));
            }
            Context.Add(permissionExpansion);
            await Context.SaveChangesAsync();
            return permissionExpansion;
        }


        public async Task<List<PermissionExpansion>> CreateListAsync(List<PermissionExpansion> permissionExpansionList)
        {
            if (permissionExpansionList == null)
            {
                throw new ArgumentNullException(nameof(permissionExpansionList));
            }
            Context.AddRange(permissionExpansionList);
            await Context.SaveChangesAsync();
            return permissionExpansionList;
        }

        public async Task DeleteAsync(PermissionExpansion permissionExpansion)
        {
            if (permissionExpansion == null)
            {
                throw new ArgumentNullException(nameof(permissionExpansion));
            }
            Context.Remove(permissionExpansion);
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<PermissionExpansion> permissionExpansionList)
        {
            if (permissionExpansionList == null)
            {
                throw new ArgumentNullException(nameof(permissionExpansionList));
            }
            Context.RemoveRange(permissionExpansionList);
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<PermissionExpansion>, IQueryable<TResult>> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return query.Invoke(Context.PermissionExpansions).SingleOrDefaultAsync();
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<PermissionExpansion>, IQueryable<TResult>> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return query.Invoke(Context.PermissionExpansions.AsNoTracking()).ToListAsync();
        }

        public async Task UpdateAsync(PermissionExpansion organizationExpansion)
        {
            if (organizationExpansion == null)
            {
                throw new ArgumentNullException(nameof(organizationExpansion));
            }

            Context.Attach(organizationExpansion);
            Context.Update(organizationExpansion);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task UpdateListAsync(List<PermissionExpansion> organizationExpansionList)
        {
            if (organizationExpansionList == null)
            {
                throw new ArgumentNullException(nameof(organizationExpansionList));
            }

            Context.AttachRange(organizationExpansionList);
            Context.UpdateRange(organizationExpansionList);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }






    }
}
