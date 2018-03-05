using AuthorizationCenter.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public class PermissionOrganizationStore<TContext> : IPermissionOrganizationStore where TContext : ApplicationDbContext
    {
        public PermissionOrganizationStore(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Organizations = Context.Organizations;
        }

        protected virtual TContext Context { get; }
        public IQueryable<Organization> Organizations { get; set; }

        public async Task<PermissionOrganization> CreateAsync(PermissionOrganization permissionOrganization, CancellationToken cancellationToken)
        {
            if (permissionOrganization == null)
            {
                throw new ArgumentNullException(nameof(permissionOrganization));
            }
            Context.Add(permissionOrganization);
            await Context.SaveChangesAsync(cancellationToken);
            return permissionOrganization;
        }


        public async Task<List<PermissionOrganization>> CreateListAsync(List<PermissionOrganization> permissionOrganizationList, CancellationToken cancellationToken)
        {
            if (permissionOrganizationList == null)
            {
                throw new ArgumentNullException(nameof(permissionOrganizationList));
            }
            Context.AddRange(permissionOrganizationList);
            await Context.SaveChangesAsync(cancellationToken);
            return permissionOrganizationList;
        }

        public async Task DeleteAsync(PermissionOrganization permissionOrganization, CancellationToken cancellationToken)
        {
            if (permissionOrganization == null)
            {
                throw new ArgumentNullException(nameof(permissionOrganization));
            }
            Context.Remove(permissionOrganization);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public async Task DeleteListAsync(List<PermissionOrganization> permissionOrganizationList, CancellationToken cancellationToken)
        {
            if (permissionOrganizationList == null)
            {
                throw new ArgumentNullException(nameof(permissionOrganizationList));
            }
            Context.RemoveRange(permissionOrganizationList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public Task<TResult> GetAsync<TResult>(Func<IQueryable<PermissionOrganization>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return query.Invoke(Context.PermissionOrganizations).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<PermissionOrganization>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return query.Invoke(Context.PermissionOrganizations).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(PermissionOrganization permissionOrganization, CancellationToken cancellationToken)
        {
            if (permissionOrganization == null)
            {
                throw new ArgumentNullException(nameof(permissionOrganization));
            }

            Context.Attach(permissionOrganization);
            Context.Update(permissionOrganization);

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
