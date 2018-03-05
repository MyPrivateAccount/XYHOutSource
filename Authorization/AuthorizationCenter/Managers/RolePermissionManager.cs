using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Managers
{
    public class RolePermissionManager
    {
        public RolePermissionManager(IRolePermissionStore rolePermissionStore)
        {
            if (rolePermissionStore == null)
            {
                throw new ArgumentNullException(nameof(rolePermissionStore));
            }
            Store = rolePermissionStore;
        }

        protected IRolePermissionStore Store { get; }



        public virtual async Task<RolePermission> CreateAsync(RolePermission rolePermission, CancellationToken cancellationToken)
        {
            if (rolePermission == null)
            {
                throw new ArgumentNullException(nameof(rolePermission));
            }
            return await Store.CreateAsync(rolePermission, cancellationToken);
        }


        public virtual async Task CreateListAsync(string roleId, List<RolePermission> rolePermissionList, CancellationToken cancellationToken)
        {
            if (rolePermissionList == null)
            {
                throw new ArgumentNullException(nameof(rolePermissionList));
            }
            Store.DeleteByRoleIdAsync(roleId, CancellationToken.None).Wait();
            await Store.CreateListAsync(rolePermissionList, cancellationToken);
        }



        public virtual Task DeleteAsync(RolePermission rolePermission, CancellationToken cancellationToken)
        {
            if (rolePermission == null)
            {
                throw new ArgumentNullException(nameof(rolePermission));
            }
            return Store.DeleteAsync(rolePermission, cancellationToken);
        }

        public virtual async Task DeleteByRoleIdAsync(string id, CancellationToken cancellationToken)
        {
            await Store.DeleteByRoleIdAsync(id, cancellationToken);
        }
        public virtual async Task DeleteByRoleIdsAsync(List<string> ids, CancellationToken cancellationToken)
        {
            await Store.DeleteByRoleIdsAsync(ids, cancellationToken);
        }

        public virtual async Task DeleteByPermissionItemIdAsync(string id, CancellationToken cancellationToken)
        {
            await Store.DeleteByPermissionIdAsync(id, cancellationToken);
        }
        public virtual async Task DeleteByPermissionItemIdsAsync(List<string> ids, CancellationToken cancellationToken)
        {
            await Store.DeleteByPermissionIdsAsync(ids, cancellationToken);
        }

        public virtual Task<List<RolePermission>> FindByRoleIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return Store.ListAsync(a => a.Where(b => b.RoleId == roleId), cancellationToken);
        }

        public virtual async Task UpdateAsync(RolePermission rolePermission, CancellationToken cancellationToken)
        {
            if (rolePermission == null)
            {
                throw new ArgumentNullException(nameof(rolePermission));
            }
            await Store.UpdateAsync(rolePermission, cancellationToken);
        }

    }
}
