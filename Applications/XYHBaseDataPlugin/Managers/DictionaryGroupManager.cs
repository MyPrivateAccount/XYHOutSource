using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHBaseDataPlugin.Dto;
using XYHBaseDataPlugin.Stores;
using Microsoft.EntityFrameworkCore;
using XYHBaseDataPlugin.Models;

namespace XYHBaseDataPlugin.Managers
{
    public class DictionaryGroupManager
    {
        public DictionaryGroupManager(IDictionaryGroupStore dictionaryGroupStore)
        {
            Store = dictionaryGroupStore ?? throw new ArgumentNullException(nameof(dictionaryGroupStore));
        }

        protected IDictionaryGroupStore Store { get; }


        public virtual async Task<DictionaryGroup> CreateAsync(DictionaryGroup dictionaryGroup, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dictionaryGroup == null)
            {
                throw new ArgumentNullException(nameof(dictionaryGroup));
            }
            return await Store.CreateAsync(dictionaryGroup, cancellationToken);
        }

        public virtual Task DeleteAsync(string userId, DictionaryGroup dictionaryGroup, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dictionaryGroup == null)
            {
                throw new ArgumentNullException(nameof(dictionaryGroup));
            }
            dictionaryGroup.IsDeleted = true;
            dictionaryGroup.DeleteUser = userId;
            dictionaryGroup.Name = dictionaryGroup.Name + "(已删除)";
            return Store.UpdateAsync(dictionaryGroup, cancellationToken);
        }

        public virtual Task DeleteListAsync(string userId, List<DictionaryGroup> dictionaryGroupList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dictionaryGroupList == null)
            {
                throw new ArgumentNullException(nameof(dictionaryGroupList));
            }
            for (int i = 0; i < dictionaryGroupList.Count; i++)
            {
                dictionaryGroupList[i].IsDeleted = true;
                dictionaryGroupList[i].DeleteUser = userId;
                dictionaryGroupList[i].Name = dictionaryGroupList[i].Name + "(已删除)";
            }
            return Store.UpdateListAsync(dictionaryGroupList, cancellationToken);
        }

        public virtual async Task DeleteListAsync(string userId, List<string> dictionaryGroupIds, CancellationToken cancellationToken = default(CancellationToken))
        {
            var list = await Store.ListAsync(a => a.Where(b => dictionaryGroupIds.Contains(b.Id) && !b.IsDeleted), cancellationToken);
            if (list?.Count == 0)
            {
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                list[i].IsDeleted = true;
                list[i].DeleteUser = userId;
                list[i].Name = list[i].Name + "(已删除)";
            }
            await Store.UpdateListAsync(list, cancellationToken);
        }

        public virtual Task<DictionaryGroup> FindByIdAsync(string groupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Store.GetAsync(a => a.Where(b => b.Id == groupId), cancellationToken);
        }

        public virtual Task<List<DictionaryGroup>> Search(DictionaryGroupSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            var q = Store.DictionaryGroups.Where(a => !a.IsDeleted);
            if (condition?.GroupIds?.Count > 0)
            {
                q = q.Where(a => condition.GroupIds.Contains(a.Id));
            }
            return q.ToListAsync(cancellationToken);
        }



        public virtual async Task UpdateAsync(string userId, DictionaryGroup dictionaryGroup, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dictionaryGroup == null)
            {
                throw new ArgumentNullException(nameof(dictionaryGroup));
            }
            dictionaryGroup.UpdateTime = DateTime.Now;
            dictionaryGroup.UpdateUser = userId;
            await Store.UpdateAsync(dictionaryGroup, cancellationToken);
        }

        public virtual async Task<List<DictionaryGroup>> GetList(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Store.ListAsync(a => a.Where(b => true), cancellationToken);
        }

    }
}
