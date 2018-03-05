using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Managers
{
    public class PermissionOrganizationManager
    {
        public PermissionOrganizationManager(IPermissionOrganizationStore permissionOrganizationStore, IRolePermissionStore rolePermissionStore)
        {
            Store = permissionOrganizationStore ?? throw new ArgumentNullException(nameof(permissionOrganizationStore));
            _rolePermissionStore = rolePermissionStore ?? throw new ArgumentNullException(nameof(rolePermissionStore));
        }

        protected IPermissionOrganizationStore Store { get; }
        protected IRolePermissionStore _rolePermissionStore { get; }


        public virtual async Task<PermissionOrganization> CreateAsync(PermissionOrganization permissionOrganization, CancellationToken cancellationToken)
        {
            if (permissionOrganization == null)
            {
                throw new ArgumentNullException(nameof(permissionOrganization));
            }
            return await Store.CreateAsync(permissionOrganization, cancellationToken);
        }

        public virtual async Task CreateListAsync(List<PermissionOrganization> permissionOrganizationList, CancellationToken cancellationToken)
        {
            if (permissionOrganizationList == null)
            {
                throw new ArgumentNullException(nameof(permissionOrganizationList));
            }
            await Store.CreateListAsync(permissionOrganizationList, cancellationToken);
        }

        public virtual Task DeleteAsync(PermissionOrganization permissionOrganization, CancellationToken cancellationToken)
        {
            if (permissionOrganization == null)
            {
                throw new ArgumentNullException(nameof(permissionOrganization));
            }
            return Store.DeleteAsync(permissionOrganization, cancellationToken);
        }

        public virtual Task DeleteListAsync(List<PermissionOrganization> permissionOrganizationList, CancellationToken cancellationToken)
        {
            if (permissionOrganizationList == null)
            {
                throw new ArgumentNullException(nameof(permissionOrganizationList));
            }
            return Store.DeleteListAsync(permissionOrganizationList, cancellationToken);
        }

        public virtual async Task DeleteByOrganizationIdsAsync(List<string> organizationIds, CancellationToken cancellationToken)
        {
            if (organizationIds == null)
            {
                throw new ArgumentNullException(nameof(organizationIds));
            }
            var organizations = await Store.ListAsync(a => a.Where(b => organizationIds.Contains(b.OrganizationId)), CancellationToken.None);
            if (organizations?.Count > 0)
            {
                await Store.DeleteListAsync(organizations, cancellationToken);
            }
        }

        public virtual async Task DeleteByPermissionIdsAsync(List<string> permissionIds, CancellationToken cancellationToken)
        {
            if (permissionIds == null)
            {
                throw new ArgumentNullException(nameof(permissionIds));
            }
            var rolePermissions = await _rolePermissionStore.ListAsync(a => a.Where(b => permissionIds.Contains(b.PermissionId)), CancellationToken.None);
            var organizations = await Store.ListAsync(a => a.Where(b => rolePermissions.Select(x => x.OrganizationScope).Contains(b.OrganizationScope)), CancellationToken.None);
            if (organizations?.Count > 0)
            {
                await Store.DeleteListAsync(organizations, cancellationToken);
            }
        }

        public virtual Task<List<PermissionOrganization>> FindByIdAsync(string id, CancellationToken cancellationToken)
        {
            return Store.ListAsync(a => a.Where(b => b.OrganizationScope == id), cancellationToken);
        }


        public virtual async Task UpdateAsync(PermissionOrganization permissionOrganization, CancellationToken cancellationToken)
        {
            if (permissionOrganization == null)
            {
                throw new ArgumentNullException(nameof(permissionOrganization));
            }
            await Store.UpdateAsync(permissionOrganization, cancellationToken);
        }

    }
}
