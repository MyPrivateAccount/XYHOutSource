using AuthorizationCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public interface IPermissionOrganizationStore
    {
        IQueryable<Organization> Organizations { get; set; }

        Task<PermissionOrganization> CreateAsync(PermissionOrganization permissionOrganization, CancellationToken cancellationToken);

        Task<List<PermissionOrganization>> CreateListAsync(List<PermissionOrganization> permissionOrganizationList, CancellationToken cancellationToken);

        Task DeleteAsync(PermissionOrganization permissionOrganization, CancellationToken cancellationToken);

        Task DeleteListAsync(List<PermissionOrganization> permissionOrganizationList, CancellationToken cancellationToken);

        Task<TResult> GetAsync<TResult>(Func<IQueryable<PermissionOrganization>, IQueryable<TResult>> query, CancellationToken cancellationToken);

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<PermissionOrganization>, IQueryable<TResult>> query, CancellationToken cancellationToken);

        Task UpdateAsync(PermissionOrganization permissionOrganization, CancellationToken cancellationToken);
    }
}
