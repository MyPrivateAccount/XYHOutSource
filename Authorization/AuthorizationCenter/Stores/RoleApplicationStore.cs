using AuthorizationCenter.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public class RoleApplicationStore<TContext> : IRoleApplicationStore where TContext : ApplicationDbContext
    {
        public RoleApplicationStore(TContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            Context = context;
            RoleApplications = Context.RoleApplications;
        }

        protected virtual TContext Context { get; }

        public IQueryable<RoleApplication> RoleApplications { get; set; }

        public async Task<RoleApplication> CreateAsync(RoleApplication roleApplication, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (roleApplication == null)
            {
                throw new ArgumentNullException(nameof(roleApplication));
            }
            Context.Add(roleApplication);
            await Context.SaveChangesAsync(cancellationToken);
            return roleApplication;
        }

        public async Task UpdateListAsync(string roleId, List<RoleApplication> roleApplicationList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (roleApplicationList == null)
            {
                throw new ArgumentNullException(nameof(roleApplicationList));
            }
            var roleApplication = Context.RoleApplications.Where(b => b.RoleId == roleId);
            Context.RemoveRange(roleApplication);
            Context.SaveChanges();
            Context.AddRange(roleApplicationList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteAsync(RoleApplication roleApplication, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (roleApplication == null)
            {
                throw new ArgumentNullException(nameof(roleApplication));
            }
            Context.Remove(roleApplication);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }


        public async Task DeleteByRoleIdAsync(string roleId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var rolePermissions = await ListAsync(a => a.Where(b => b.RoleId == roleId), cancellationToken);
            if (rolePermissions.Count() == 0)
            {
                return;
            }
            Context.RemoveRange(rolePermissions);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }
        public async Task DeleteByRoleIdsAsync(List<string> roleIds, CancellationToken cancellationToken = default(CancellationToken))
        {
            var roleApplications = await ListAsync(a => a.Where(b => roleIds.Contains(b.RoleId)), cancellationToken);
            if (roleApplications.Count() == 0)
            {
                return;
            }
            Context.RemoveRange(roleApplications);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<RoleApplication>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return query.Invoke(Context.RoleApplications).SingleOrDefaultAsync(cancellationToken);
        }

        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<RoleApplication>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return query.Invoke(Context.RoleApplications).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(RoleApplication roleApplication, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (roleApplication == null)
            {
                throw new ArgumentNullException(nameof(roleApplication));
            }
            Context.Attach(roleApplication);
            Context.Update(roleApplication);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }

            catch (DbUpdateConcurrencyException) { }
        }

    }
}
