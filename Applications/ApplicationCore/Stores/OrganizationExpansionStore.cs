using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationCore.Stores
{
    public class OrganizationExpansionStore<TContext> : IOrganizationExpansionStore where TContext : CoreDbContext
    {

        public OrganizationExpansionStore(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            OrganizationExpansions = Context.OrganizationExpansions;
        }

        protected virtual TContext Context { get; }

        public DbSet<OrganizationExpansion> OrganizationExpansions { get; }

        public async Task<OrganizationExpansion> CreateAsync(OrganizationExpansion organizationExpansion)
        {
            if (organizationExpansion == null)
            {
                throw new ArgumentNullException(nameof(organizationExpansion));
            }
            Context.Add(organizationExpansion);
            await Context.SaveChangesAsync();
            return organizationExpansion;
        }


        public async Task<List<OrganizationExpansion>> CreateListAsync(List<OrganizationExpansion> organizationExpansionList)
        {
            if (organizationExpansionList == null)
            {
                throw new ArgumentNullException(nameof(organizationExpansionList));
            }
            Context.AddRange(organizationExpansionList);
            await Context.SaveChangesAsync();
            return organizationExpansionList;
        }

        public async Task DeleteAsync(OrganizationExpansion organizationExpansion)
        {
            if (organizationExpansion == null)
            {
                throw new ArgumentNullException(nameof(organizationExpansion));
            }
            Context.Remove(organizationExpansion);
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<OrganizationExpansion> organizationExpansionList)
        {
            if (organizationExpansionList == null)
            {
                throw new ArgumentNullException(nameof(organizationExpansionList));
            }
            Context.RemoveRange(organizationExpansionList);
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<OrganizationExpansion>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return query.Invoke(Context.OrganizationExpansions).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<OrganizationExpansion>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return query.Invoke(Context.OrganizationExpansions).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(OrganizationExpansion organizationExpansion)
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

        public async Task UpdateListAsync(List<OrganizationExpansion> organizationExpansionList)
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
