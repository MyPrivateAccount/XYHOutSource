using AuthorizationCenter.Interface;
using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Managers
{
    public class PermissionItemManager
    {
        public PermissionItemManager(IPermissionItemStore permissionItemStore,
            PermissionExpansionManager PermissionExpansionManager)
        {
            Store = permissionItemStore ?? throw new ArgumentNullException(nameof(permissionItemStore));
            _PermissionExpansionManager = PermissionExpansionManager ?? throw new ArgumentNullException(nameof(PermissionExpansionManager));
        }

        protected IPermissionItemStore Store { get; }
        protected PermissionExpansionManager _PermissionExpansionManager { get; }


        public virtual async Task<PermissionItem> CreateAsync(PermissionItem permissionItem, CancellationToken cancellationToken)
        {
            if (permissionItem == null)
            {
                throw new ArgumentNullException(nameof(permissionItem));
            }
            return await Store.CreateAsync(permissionItem, cancellationToken);
        }

        public virtual async Task DeleteAsync(PermissionItem permissionItem, CancellationToken cancellationToken)
        {
            if (permissionItem == null)
            {
                throw new ArgumentNullException(nameof(permissionItem));
            }
            Store.DeleteAsync(permissionItem, cancellationToken).Wait();
            await _PermissionExpansionManager.RemovePermissionAsync(permissionItem.Id);
        }

        public virtual async Task DeleteListAsync(List<PermissionItem> permissionItemList, CancellationToken cancellationToken)
        {
            if (permissionItemList == null)
            {
                throw new ArgumentNullException(nameof(permissionItemList));
            }
            Store.DeleteListAsync(permissionItemList, cancellationToken).Wait();
            await _PermissionExpansionManager.RemovePermissionsAsync(permissionItemList.Select(a => a.Id).ToList());
        }

        public virtual async Task DeleteListAsync(List<string> permissionItemIds, CancellationToken cancellationToken)
        {

            var list = await Store.ListAsync(a => a.Where(b => permissionItemIds.Contains(b.Id)), cancellationToken);
            if (list?.Count == 0)
            {
                return;
            }
            Store.DeleteListAsync(list, cancellationToken).Wait();
            await _PermissionExpansionManager.RemovePermissionsAsync(permissionItemIds);
        }



        public virtual Task<PermissionItem> FindByIdAsync(string id, CancellationToken cancellationToken)
        {
            return Store.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
        }

        public virtual Task<List<PermissionItem>> FindByApplicationAsync(string id, CancellationToken cancellationToken)
        {
            return Store.ListAsync(a => a.Where(b => b.ApplicationId == id), cancellationToken);
        }

        public virtual async Task UpdateAsync(PermissionItem permissionItem, CancellationToken cancellationToken)
        {
            if (permissionItem == null)
            {
                throw new ArgumentNullException(nameof(permissionItem));
            }
            await Store.UpdateAsync(permissionItem, cancellationToken);
        }

        public virtual async Task<List<PermissionItem>> GetList()
        {
            return await Store.ListAsync(a => a.Where(b => true), CancellationToken.None);
        }


    }
}
