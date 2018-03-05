using AuthorizationCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public interface IRoleApplicationStore
    {
        IQueryable<RoleApplication> RoleApplications { get; set; }

        Task<RoleApplication> CreateAsync(RoleApplication roleApplication, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(string roleId, List<RoleApplication> roleApplicationList, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(RoleApplication roleApplication, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteByRoleIdAsync(string roleId, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteByRoleIdsAsync(List<string> roleIds, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<RoleApplication>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<RoleApplication>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(RoleApplication roleApplication, CancellationToken cancellationToken = default(CancellationToken));
    }
}
