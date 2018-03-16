using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHBaseDataPlugin.Dto;
using XYHBaseDataPlugin.Models;
using XYHBaseDataPlugin.Stores;

namespace XYHBaseDataPlugin.Managers
{
    public class DictionaryDefineManager
    {
        public DictionaryDefineManager(IDictionaryDefineStore dictionaryDefineStore, IMapper mapper)
        {
            Store = dictionaryDefineStore ?? throw new ArgumentNullException(nameof(dictionaryDefineStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IDictionaryDefineStore Store { get; }
        protected IMapper _mapper { get; }


        public virtual async Task<DictionaryDefine> CreateAsync(DictionaryDefine dictionaryDefine, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dictionaryDefine == null)
            {
                throw new ArgumentNullException(nameof(dictionaryDefine));
            }
            return await Store.CreateAsync(dictionaryDefine, cancellationToken);
        }

        public virtual Task DeleteAsync(string userId, DictionaryDefine dictionaryDefine, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dictionaryDefine == null)
            {
                throw new ArgumentNullException(nameof(dictionaryDefine));
            }
            dictionaryDefine.IsDeleted = true;
            dictionaryDefine.DeleteTime = DateTime.Now;
            dictionaryDefine.DeleteUser = userId;
            dictionaryDefine.Key = dictionaryDefine.Key + "(已删除)";
            return Store.DeleteAsync(dictionaryDefine, cancellationToken);
        }

        public virtual async Task DeleteListAsync(string userId, List<DictionaryDefineDeleteRequest> dictionaryDefineDeleteRequestList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dictionaryDefineDeleteRequestList == null)
            {
                throw new ArgumentNullException(nameof(dictionaryDefineDeleteRequestList));
            }
            List<DictionaryDefine> list = new List<DictionaryDefine>();
            foreach (var item in dictionaryDefineDeleteRequestList)
            {
                list.Add(await Store.GetAsync(a => a.Where(b => b.GroupId == item.GroupId && b.Value == item.Value)));
            }
            for (int i = 0; i < list.Count; i++)
            {
                list[i].DeleteUser = userId;
                list[i].IsDeleted = true;
                list[i].DeleteTime = DateTime.Now;
                list[i].Key = list[i].Key + "(已删除)";
            }
            await Store.UpdateListAsync(list, cancellationToken);
        }

        public virtual async Task<List<DictionaryDefine>> FindByGroupIdAsync(string groupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Store.ListAsync(a => a.Where(b => b.GroupId == groupId && !b.IsDeleted));
        }

        public virtual async Task<DictionaryDefine> FindByGroupIdAndValueAsync(string groupId, string value, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Store.GetAsync(a => a.Where(b => b.GroupId == groupId && b.Value == value && !b.IsDeleted));
        }

        public virtual async Task<List<DictionaryDefineListResponse>> FindByGroupIdsAsync(List<string> groupIds, CancellationToken cancellationToken = default(CancellationToken))
        {
            List<DictionaryDefineListResponse> list = new List<DictionaryDefineListResponse>();
            var defines = await Store.ListAsync(a => a.Where(b => groupIds.Contains(b.GroupId)));
            var group = defines.GroupBy(a => a.GroupId);
            foreach (var item in group)
            {
                var dictionaryDefineListResponse = new DictionaryDefineListResponse { GroupId = item.Key, DictionaryDefines = new List<DictionaryDefineResponse>() };
                foreach (var item1 in item)
                {
                    dictionaryDefineListResponse.DictionaryDefines.Add(_mapper.Map<DictionaryDefineResponse>(item1));
                }
                list.Add(dictionaryDefineListResponse);
            }
            return list;
        }

        public virtual async Task<List<DictionaryDefineListResponse>> FindDeletedByGroupIdsAsync(List<string> groupIds, CancellationToken cancellationToken = default(CancellationToken))
        {
            List<DictionaryDefineListResponse> list = new List<DictionaryDefineListResponse>();
            var defines = await Store.ListAsync(a => a.Where(b => groupIds.Contains(b.GroupId) && b.IsDeleted));
            var group = defines.GroupBy(a => a.GroupId);
            foreach (var item in group)
            {
                var dictionaryDefineListResponse = new DictionaryDefineListResponse { GroupId = item.Key, DictionaryDefines = new List<DictionaryDefineResponse>() };
                foreach (var item1 in item)
                {
                    dictionaryDefineListResponse.DictionaryDefines.Add(_mapper.Map<DictionaryDefineResponse>(item1));
                }
                list.Add(dictionaryDefineListResponse);
            }
            return list;
        }


        public virtual async Task UpdateAsync(string groupId, string value, DictionaryDefineUpdateRequest dictionaryDefineUpdateRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dictionaryDefineUpdateRequest == null || string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(dictionaryDefineUpdateRequest));
            }
            var dicdefine = await Store.GetAsync(a => a.Where(b => b.GroupId == groupId && b.Value == value));
            if (dicdefine == null)
            {
                throw new Exception("未找到更新对象");
            }
            dicdefine.Ext1 = dictionaryDefineUpdateRequest.Ext1;
            dicdefine.Ext2 = dictionaryDefineUpdateRequest.Ext2;
            dicdefine.Key = dictionaryDefineUpdateRequest.Key;
            dicdefine.Order = dictionaryDefineUpdateRequest.Order;

            await Store.UpdateAsync(dicdefine, cancellationToken);
        }



        public virtual async Task ReuseAsync(string groupId, string value, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException();
            }
            var dicdefine = await Store.GetAsync(a => a.Where(b => b.GroupId == groupId && b.Value == value));
            if (dicdefine == null)
            {
                throw new Exception("未找到启用字典对象");
            }
            dicdefine.IsDeleted = false;
            await Store.UpdateAsync(dicdefine, cancellationToken);
        }

    }
}
