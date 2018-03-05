using AuthorizationCenter.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public class RolePermissionStore<TContext> : IRolePermissionStore where TContext : ApplicationDbContext
    {
        public RolePermissionStore(TContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            Context = context;
        }

        protected virtual TContext Context { get; }

        public async Task<RolePermission> CreateAsync(RolePermission rolePermission, CancellationToken cancellationToken)
        {
            if (rolePermission == null)
            {
                throw new ArgumentNullException(nameof(rolePermission));
            }
            Context.Add(rolePermission);
            await Context.SaveChangesAsync(cancellationToken);
            return rolePermission;
        }


        public async Task CreateListAsync(List<RolePermission> rolePermissionList, CancellationToken cancellationToken)
        {
            if (rolePermissionList == null)
            {
                throw new ArgumentNullException(nameof(rolePermissionList));
            }
            Context.AddRange(rolePermissionList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public async Task DeleteAsync(RolePermission rolePermission, CancellationToken cancellationToken)
        {
            if (rolePermission == null)
            {
                throw new ArgumentNullException(nameof(rolePermission));
            }
            Context.Remove(rolePermission);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }


        public async Task DeleteByRoleIdAsync(string roleId, CancellationToken cancellationToken)
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
        public async Task DeleteByRoleIdsAsync(List<string> roleIds, CancellationToken cancellationToken)
        {
            var rolePermissions = await ListAsync(a => a.Where(b => roleIds.Contains(b.RoleId)), cancellationToken);
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
        public async Task DeleteByPermissionIdAsync(string permissionItemId, CancellationToken cancellationToken)
        {
            var rolePermissions = await ListAsync(a => a.Where(b => b.PermissionId == permissionItemId), cancellationToken);
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
        public async Task DeleteByPermissionIdsAsync(List<string> permissionItemIds, CancellationToken cancellationToken)
        {
            var rolePermissions = await ListAsync(a => a.Where(b => permissionItemIds.Contains(b.PermissionId)), cancellationToken);
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

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<RolePermission>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return query.Invoke(Context.RolePermissions).SingleOrDefaultAsync(cancellationToken);
        }

        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<RolePermission>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return query.Invoke(Context.RolePermissions).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(RolePermission rolePermission, CancellationToken cancellationToken)
        {
            if (rolePermission == null)
            {
                throw new ArgumentNullException(nameof(rolePermission));
            }

            Context.Attach(rolePermission);
            Context.Update(rolePermission);

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }

            catch (DbUpdateConcurrencyException) { }
        }



    }
}
