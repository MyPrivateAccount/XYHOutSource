using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationCore.Stores
{
    public interface IPermissionExpansionStore
    {
        Task<PermissionExpansion> CreateAsync(PermissionExpansion permissionExpansion);

        Task<List<PermissionExpansion>> CreateListAsync(List<PermissionExpansion> permissionExpansionList);

        Task DeleteAsync(PermissionExpansion permissionExpansion);

        Task DeleteListAsync(List<PermissionExpansion> permissionExpansionList);

        Task<TResult> GetAsync<TResult>(Func<IQueryable<PermissionExpansion>, IQueryable<TResult>> query);

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<PermissionExpansion>, IQueryable<TResult>> query);

        Task UpdateAsync(PermissionExpansion organizationExpansion);
        Task UpdateListAsync(List<PermissionExpansion> organizationExpansionList);

    }
}
