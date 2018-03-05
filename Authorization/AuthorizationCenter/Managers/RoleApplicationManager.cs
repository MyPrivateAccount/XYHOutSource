using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Managers
{
    public class RoleApplicationManager
    {
        public RoleApplicationManager(IRoleApplicationStore roleApplicationStore,
            IUserRoleStore userRoleStore,
            ExtendUserManager<Users> extendUserManager)
        {
            Store = roleApplicationStore ?? throw new ArgumentNullException(nameof(roleApplicationStore));
            _extendUserManager = extendUserManager ?? throw new ArgumentNullException(nameof(extendUserManager));
            _userRoleStore = userRoleStore ?? throw new ArgumentNullException(nameof(userRoleStore));
        }

        protected IRoleApplicationStore Store { get; }

        protected ExtendUserManager<Users> _extendUserManager { get; }
        protected IUserRoleStore _userRoleStore { get; }


        public virtual async Task<RoleApplication> CreateAsync(RoleApplication roleApplication, CancellationToken cancellationToken)
        {
            if (roleApplication == null)
            {
                throw new ArgumentNullException(nameof(roleApplication));
            }
            return await Store.CreateAsync(roleApplication, cancellationToken);
        }


        public virtual async Task UpdateListAsync(string roleId, List<RoleApplication> rolePermissionList, CancellationToken cancellationToken)
        {
            if (rolePermissionList == null)
            {
                throw new ArgumentNullException(nameof(rolePermissionList));
            }
            await Store.UpdateListAsync(roleId, rolePermissionList, cancellationToken);
        }

        public virtual Task DeleteAsync(RoleApplication roleApplication, CancellationToken cancellationToken)
        {
            if (roleApplication == null)
            {
                throw new ArgumentNullException(nameof(roleApplication));
            }
            return Store.DeleteAsync(roleApplication, cancellationToken);
        }

        public virtual async Task DeleteByRoleIdAsync(string roleId, CancellationToken cancellationToken)
        {
            await Store.DeleteByRoleIdAsync(roleId, cancellationToken);
        }
        public virtual async Task DeleteByRoleIdsAsync(List<string> roleIds, CancellationToken cancellationToken)
        {
            await Store.DeleteByRoleIdsAsync(roleIds, cancellationToken);
        }

        public Task<List<RoleApplication>> FindByRoleIdsAsync(List<string> roleIds, CancellationToken cancellationToken)
        {
            return Store.ListAsync(a => a.Where(b => roleIds.Contains(b.RoleId)), cancellationToken);
        }

        public async Task<IEnumerable<string>> FindApplicationIdsByRoleIdsAsync(List<string> roleIds, CancellationToken cancellationToken)
        {
            var roleApplications = await Store.ListAsync(a => a.Where(b => roleIds.Contains(b.RoleId)), cancellationToken);
            return roleApplications.Select(a => a.ApplicationId).Distinct();
        }


        public async Task<IEnumerable<string>> FindApplicationIdsByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            var roleIds = _userRoleStore.GetRoleIdsAsync(userId, cancellationToken);
            var roleApplications = await Store.ListAsync(a => a.Where(b => roleIds.Contains(b.RoleId)), cancellationToken);
            return roleApplications.Select(a => a.ApplicationId).Distinct();
        }



        public async Task UpdateAsync(RoleApplication roleApplication, CancellationToken cancellationToken)
        {
            if (roleApplication == null)
            {
                throw new ArgumentNullException(nameof(roleApplication));
            }
            await Store.UpdateAsync(roleApplication, cancellationToken);
        }




    }
}
