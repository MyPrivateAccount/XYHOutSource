using ApplicationCore.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationCore.Managers
{
    public class PermissionExpansionManager
    {
        public PermissionExpansionManager(IPermissionExpansionStore permissionExpansionStore, IOrganizationExpansionStore organizationExpansionStore)
        {
            Store = permissionExpansionStore ?? throw new ArgumentNullException(nameof(permissionExpansionStore));
            _organizationExpansionStore = organizationExpansionStore ?? throw new ArgumentNullException(nameof(organizationExpansionStore));
        }

        protected IPermissionExpansionStore Store { get; }
        protected IOrganizationExpansionStore _organizationExpansionStore { get; }


        public virtual async Task<bool> HavePermission(string userId, string permissionId)
        {
            var permissions = await Store.ListAsync(a => a.Where(b => b.UserId == userId && b.PermissionId == permissionId));
            if (permissions?.Count > 0)
            {
                return true;
            }
            return false;
        }


        public virtual async Task<bool> HavePermission(string userId, string permissionId, string organizationId)
        {
            var permissions = await Store.ListAsync(a => a.Where(b => b.UserId == userId && b.PermissionId == permissionId));
            if (permissions?.Count == 0)
            {
                return false;
            }
            var Ids = permissions.Select(a => a.OrganizationId);
            var organizationIds = (await _organizationExpansionStore.ListAsync(a => a.Where(b => Ids.Contains(b.OrganizationId)))).Select(o => o.SonId).Distinct().ToList();
            organizationIds.AddRange(Ids);
            if (organizationIds.Contains(organizationId))
            {
                return true;
            }
            return false;
        }

        public virtual async Task<List<string>> GetOrganizationOfPermission(string userId, string permissionId)
        {
            List<string> list = new List<string>();
            var permissions = await Store.ListAsync(a => a.Where(b => b.UserId == userId && b.PermissionId == permissionId));
            if (permissions?.Count == 0)
            {
                return list;
            }
            var Ids = permissions.Select(a => a.OrganizationId);
            var organizationIds = (await _organizationExpansionStore.ListAsync(a => a.Where(b => Ids.Contains(b.OrganizationId)))).Select(o => o.SonId).Distinct().ToList();
            organizationIds.AddRange(Ids);
            return organizationIds;
        }


        public virtual async Task<List<string>> GetPermissionUserIds(string permissionItemId)
        {
            var permissions = await Store.ListAsync(a => a.Where(b => b.PermissionId == permissionItemId));
            if (permissions?.Count == 0)
            {
                return new List<string>();
            }
            return permissions.Select(a => a.UserId).ToList();
        }

        /// <summary>
        /// 获取部门下拥有指定权限的Userid
        /// </summary>
        /// <param name="permissionId"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public virtual async Task<List<string>> GetUseridsHaveOrganPermission(string organizationId, string permissionId)
        {
            var permissions = await Store.ListAsync(a => a.Where(b => b.OrganizationId == organizationId && b.PermissionId == permissionId));
            if (permissions?.Count == 0)
            {
                return new List<string>();
            }
            return permissions.Select(a => a.UserId).ToList();
        }

        /// <summary>
        /// 获取大哥部门
        /// </summary>
        /// <param name="permissionItemId"></param>
        /// <returns></returns>
        public virtual async Task<List<string>> GetParentDepartments(string organizationId)
        {
            var organizationIds = (await _organizationExpansionStore.ListAsync(a => a.Where(b => b.SonId == organizationId))).Select(o => o.OrganizationId).Distinct().ToList();
            organizationIds.Add(organizationId);
            return organizationIds;
        }

        /// <summary>
        /// 获取小弟部门
        /// </summary>
        /// <param name="permissionItemId"></param>
        /// <returns></returns>
        public virtual async Task<List<string>> GetLowerDepartments(string organizationId)
        {
            var organizationIds = (await _organizationExpansionStore.ListAsync(a => a.Where(b => b.OrganizationId == organizationId))).Select(o => o.SonId).Distinct().ToList();
            organizationIds.Add(organizationId);
            return organizationIds;
        }
    }




}
