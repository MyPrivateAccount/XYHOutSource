using AuthorizationCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public interface IPermissionItemStore
    {
        Task<PermissionItem> CreateAsync(PermissionItem permissionItem, CancellationToken cancellationToken);

        Task DeleteAsync(PermissionItem permissionItem, CancellationToken cancellationToken);
        Task DeleteListAsync(List<PermissionItem> permissionItemList, CancellationToken cancellationToken);

        Task<TResult> GetAsync<TResult>(Func<IQueryable<PermissionItem>, IQueryable<TResult>> query, CancellationToken cancellationToken);

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<PermissionItem>, IQueryable<TResult>> query, CancellationToken cancellationToken);

        Task UpdateAsync(PermissionItem permissionItem, CancellationToken cancellationToken);
    }
}
