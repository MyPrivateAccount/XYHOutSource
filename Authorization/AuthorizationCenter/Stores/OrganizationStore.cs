using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using System.Threading;
using AuthorizationCenter.Models;

namespace AuthorizationCenter.Stores
{
    public class OrganizationStore<TContext> : IOrganizationStore where TContext : ApplicationDbContext
    {
        public OrganizationStore(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Organizations = Context.Organizations;
        }

        protected virtual TContext Context { get; }
        public IQueryable<Organization> Organizations { get; set; }


        public async Task<Organization> CreateAsync(Organization organization, CancellationToken cancellationToken)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }
            Context.Add(organization);
            await Context.SaveChangesAsync(cancellationToken);
            return organization;
        }


        public async Task DeleteAsync(Organization organization, CancellationToken cancellationToken)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }
            Context.Remove(organization);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public async Task DeleteListAsync(List<Organization> organizationList, CancellationToken cancellationToken)
        {
            if (organizationList == null)
            {
                throw new ArgumentNullException(nameof(organizationList));
            }
            Context.RemoveRange(organizationList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<Organization>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.Organizations).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="query">The query to execute.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>
        /// A <see cref="Task"/> that can be used to monitor the asynchronous operation,
        /// whose result returns all the elements returned when executing the specified query.
        /// </returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<Organization>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.Organizations).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(Organization organization, CancellationToken cancellationToken)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }

            Context.Attach(organization);
            Context.Update(organization);

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }

            catch (DbUpdateConcurrencyException) { }
        }


        public async Task<bool> OrganizationExists(string id, CancellationToken cancellationToken)
        {
            return await Context.Organizations.AnyAsync(e => e.Id == id, cancellationToken);
        }
    }
}
