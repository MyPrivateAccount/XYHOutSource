using AuthorizationCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public interface IOrganizationStore
    {
        IQueryable<Organization> Organizations { get; set; }
        Task<Organization> CreateAsync(Organization organization, CancellationToken cancellationToken);
        Task DeleteAsync(Organization application, CancellationToken cancellationToken);
        Task DeleteListAsync(List<Organization> organizationList, CancellationToken cancellationToken);
        Task<TResult> GetAsync<TResult>(Func<IQueryable<Organization>, IQueryable<TResult>> query, CancellationToken cancellationToken);
        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<Organization>, IQueryable<TResult>> query, CancellationToken cancellationToken);
        Task UpdateAsync(Organization organization, CancellationToken cancellationToken);
        Task<bool> OrganizationExists(string id, CancellationToken cancellationToken);

    }
}
