using AuthorizationCenter.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public class PermissionItemStore<TContext> : IPermissionItemStore where TContext : ApplicationDbContext
    {
        public PermissionItemStore(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected virtual TContext Context { get; }

        public async Task<PermissionItem> CreateAsync(PermissionItem permissionItem, CancellationToken cancellationToken)
        {
            if (permissionItem == null)
            {
                throw new ArgumentNullException(nameof(permissionItem));
            }
            Context.Add(permissionItem);
            await Context.SaveChangesAsync(cancellationToken);
            return permissionItem;
        }


        public async Task DeleteAsync(PermissionItem permissionItem, CancellationToken cancellationToken)
        {
            if (permissionItem == null)
            {
                throw new ArgumentNullException(nameof(permissionItem));
            }
            Context.Remove(permissionItem);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<PermissionItem> permissionItemList, CancellationToken cancellationToken)
        {
            if (permissionItemList == null)
            {
                throw new ArgumentNullException(nameof(permissionItemList));
            }
            Context.RemoveRange(permissionItemList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<PermissionItem>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return query.Invoke(Context.PermissionItems).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<PermissionItem>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return query.Invoke(Context.PermissionItems).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(PermissionItem permissionItem, CancellationToken cancellationToken)
        {
            if (permissionItem == null)
            {
                throw new ArgumentNullException(nameof(permissionItem));
            }

            Context.Attach(permissionItem);
            Context.Update(permissionItem);

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }
    }
}
