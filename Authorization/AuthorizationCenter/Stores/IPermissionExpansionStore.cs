using AuthorizationCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public interface IPermissionExpansionStore
    {
        //IQueryable<PermissionExpansion> PermissionExpansions { get; set; }

        Task<PermissionExpansion> CreateAsync(PermissionExpansion permissionExpansion);

        Task<List<PermissionExpansion>> CreateListAsync(List<PermissionExpansion> permissionExpansionList);

        Task DeleteAsync(PermissionExpansion permissionExpansion);

        Task DeleteListAsync(List<PermissionExpansion> permissionExpansionList);

        Task<TResult> GetAsync<TResult>(Func<IQueryable<PermissionExpansion>, IQueryable<TResult>> query);

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<PermissionExpansion>, IQueryable<TResult>> query);

        Task UpdateAsync(PermissionExpansion organizationExpansion);
        Task UpdateListAsync(List<PermissionExpansion> organizationExpansionList);

        Task<List<string>> GetApplicationOfPermission(string userId);

    }
}
