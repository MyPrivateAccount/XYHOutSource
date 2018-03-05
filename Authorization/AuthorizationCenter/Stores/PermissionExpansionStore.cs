using AuthorizationCenter.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public class PermissionExpansionStore<TContext> : IPermissionExpansionStore where TContext : ApplicationDbContext
    {
        public PermissionExpansionStore(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            //PermissionExpansions = Context.PermissionExpansions;
        }

        protected virtual TContext Context { get; }

        //public IQueryable<PermissionExpansion> PermissionExpansions { get; set; }

        public async Task<PermissionExpansion> CreateAsync(PermissionExpansion permissionExpansion)
        {
            if (permissionExpansion == null)
            {
                throw new ArgumentNullException(nameof(permissionExpansion));
            }
            var entry = Context.Entry(permissionExpansion);
            if (entry == null)
            {
                Context.Add(permissionExpansion);
            }
            else
            {
                entry.CurrentValues.SetValues(permissionExpansion);
            }
            await Context.SaveChangesAsync();
            return permissionExpansion;
        }


        public async Task<List<PermissionExpansion>> CreateListAsync(List<PermissionExpansion> permissionExpansionList)
        {
            if (permissionExpansionList == null)
            {
                throw new ArgumentNullException(nameof(permissionExpansionList));
            }
            try
            {
                Context.AddRange(permissionExpansionList);
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw;
            }
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

            return query.Invoke(Context.PermissionExpansions.AsNoTracking()).SingleOrDefaultAsync();
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
            //var entry = Context.Entry(organizationExpansion);
            //if (entry == null)
            //{
            Context.Attach(organizationExpansion);
            //}
            //else
            //{
            //entry.CurrentValues.SetValues(organizationExpansion);
            //}
            try
            {
                Context.Update(organizationExpansion);
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
            //for (int i = 0; i < organizationExpansionList.Count; i++)
            //{
            //    var entry = Context.Entry(organizationExpansionList[i]);
            //    if (entry == null)
            //    {
            //        Context.Attach(organizationExpansionList[i]);
            //        Context.Update(organizationExpansionList[i]);
            //    }
            //    else
            //    {
            //        entry.CurrentValues.SetValues(organizationExpansionList[i]);
            //        Context.Update(organizationExpansionList[i]);
            //    }
            //}
            try
            {
                Context.AddRange(organizationExpansionList);
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public async Task<List<string>> GetApplicationOfPermission(string userId)
        {
            return await Context.PermissionExpansions.AsNoTracking().Where(a => a.UserId == userId).Select(b => b.ApplicationId).Distinct().ToListAsync();
        }



    }
}
