using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationCore.Stores
{
    public interface IOrganizationExpansionStore
    {
        DbSet<OrganizationExpansion> OrganizationExpansions { get; }

        Task<OrganizationExpansion> CreateAsync(OrganizationExpansion organizationExpansion);
        Task<List<OrganizationExpansion>> CreateListAsync(List<OrganizationExpansion> organizationExpansionList);
        Task DeleteAsync(OrganizationExpansion organizationExpansion);
        Task DeleteListAsync(List<OrganizationExpansion> organizationExpansionList);

        Task<TResult> GetAsync<TResult>(Func<IQueryable<OrganizationExpansion>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<OrganizationExpansion>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task UpdateAsync(OrganizationExpansion organizationExpansion);
        Task UpdateListAsync(List<OrganizationExpansion> organizationExpansionList);

    }
}
