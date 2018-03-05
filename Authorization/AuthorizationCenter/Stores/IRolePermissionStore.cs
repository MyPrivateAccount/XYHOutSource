using AuthorizationCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public interface IRolePermissionStore
    {
        Task<RolePermission> CreateAsync(RolePermission rolePermission, CancellationToken cancellationToken);

        Task CreateListAsync(List<RolePermission> rolePermissionList, CancellationToken cancellationToken);

        Task DeleteAsync(RolePermission rolePermission, CancellationToken cancellationToken);
        Task DeleteByRoleIdsAsync(List<string> roleIds, CancellationToken cancellationToken);

        Task DeleteByRoleIdAsync(string id, CancellationToken cancellationToken);

        Task DeleteByPermissionIdAsync(string permissionItemId, CancellationToken cancellationToken);
        Task DeleteByPermissionIdsAsync(List<string> permissionItemIds, CancellationToken cancellationToken);

        Task<TResult> GetAsync<TResult>(Func<IQueryable<RolePermission>, IQueryable<TResult>> query, CancellationToken cancellationToken);

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<RolePermission>, IQueryable<TResult>> query, CancellationToken cancellationToken);

        Task UpdateAsync(RolePermission rolePermission, CancellationToken cancellationToken);


    }
}
