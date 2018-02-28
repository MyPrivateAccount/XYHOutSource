using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    public class AreaDefineManager
    {
        public AreaDefineManager(IAreaDefineStore areaDefineStore, IMapper mapper)
        {
            Store = areaDefineStore ?? throw new ArgumentNullException(nameof(areaDefineStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IAreaDefineStore Store { get; }
        protected IMapper _mapper { get; }


        public virtual async Task<List<AreaDefineResponse>> Search(string userId, AreaDefineSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            var q = Store.AreaDefines.Where(a => !a.IsDeleted);
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            if (condition?.Codes?.Count > 0)
            {
                q = q.Where(a => condition.Codes.Contains(a.Code));
            }
            if (condition?.Levels?.Count > 0)
            {
                q = q.Where(a => condition.Levels.Contains(a.Level));
            }
            var areaDefines = _mapper.Map<List<AreaDefineResponse>>(await q.ToListAsync(cancellationToken));
            return areaDefines;
        }



        public virtual async Task<AreaDefineResponse> CreateAsync(string userId, AreaDefineRequest areaDefineRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (areaDefineRequest == null)
            {
                throw new ArgumentNullException(nameof(areaDefineRequest));
            }
            var areadefine = _mapper.Map<AreaDefine>(areaDefineRequest);
            areadefine.CreateTime = DateTime.Now;
            areadefine.CreateUser = userId;
            areadefine.IsDeleted = false;
            await Store.CreateAsync(areadefine, cancellationToken);
            return _mapper.Map<AreaDefineResponse>(areadefine);
        }

        public virtual Task DeleteAsync(string userId, AreaDefineRequest areaDefineRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (areaDefineRequest == null)
            {
                throw new ArgumentNullException(nameof(areaDefineRequest));
            }
            var areaDefine = _mapper.Map<AreaDefine>(areaDefineRequest);
            areaDefine.IsDeleted = true;
            areaDefine.DeleteUser = userId;
            areaDefine.Name = areaDefineRequest.Name + "(已删除)";
            return Store.DeleteAsync(areaDefine, cancellationToken);
        }

        public virtual async Task DeleteAsync(string userId, string code, CancellationToken cancellationToken = default(CancellationToken))
        {
            var areaDefines = await Store.GetAsync(a => a.Where(b => b.Code == code && !b.IsDeleted));
            if (areaDefines == null)
            {
                return;
            }
            areaDefines.IsDeleted = true;
            areaDefines.DeleteUser = userId;
            areaDefines.Name = areaDefines.Name + "(已删除)";
            await Store.DeleteAsync(areaDefines, cancellationToken);
        }



        public virtual async Task DeleteListAsync(string userId, List<string> codes, CancellationToken cancellationToken = default(CancellationToken))
        {
            var areaDefines = await Store.ListAsync(a => a.Where(b => codes.Contains(b.Code) && !b.IsDeleted));

            if (areaDefines == null)
            {
                throw new ArgumentNullException(nameof(areaDefines));
            }
            for (int i = 0; i < areaDefines.Count; i++)
            {
                areaDefines[i].DeleteUser = userId;
                areaDefines[i].IsDeleted = true;
                areaDefines[i].Name = areaDefines[i].Name + "(已删除)";
            }
            await Store.UpdateListAsync(areaDefines, cancellationToken);
        }




        public virtual async Task DeleteListAsync(string userId, List<AreaDefineRequest> areaDefineRequestList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (areaDefineRequestList == null)
            {
                throw new ArgumentNullException(nameof(areaDefineRequestList));
            }
            List<AreaDefine> list = _mapper.Map<List<AreaDefine>>(areaDefineRequestList);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].DeleteUser = userId;
                list[i].IsDeleted = true;
                list[i].Name = list[i].Name + "(已删除)";
            }
            await Store.UpdateListAsync(list, cancellationToken);
        }

        public virtual async Task<List<AreaDefineResponse>> FindByParentCodeAsync(string parentCode, CancellationToken cancellationToken = default(CancellationToken))
        {
            var areaDefines = await Store.ListAsync(a => a.Where(b => b.ParentId == parentCode && !b.IsDeleted));
            return _mapper.Map<List<AreaDefineResponse>>(areaDefines);
        }

        //public virtual async Task<AreaDefineResponse> FindByIdAsync(string Id, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    var areaDefines = await Store.GetAsync(a => a.Where(b => b.Id == Id && !b.IsDeleted));
        //    return _mapper.Map<AreaDefineResponse>(areaDefines);
        //}

        public virtual async Task<AreaDefineResponse> FindByCodeAsync(string code, CancellationToken cancellationToken = default(CancellationToken))
        {
            var areaDefines = await Store.GetAsync(a => a.Where(b => b.Code == code && !b.IsDeleted));
            return _mapper.Map<AreaDefineResponse>(areaDefines);
        }

        public virtual async Task UpdateAsync(string userId, string code, AreaDefineRequest areaDefineRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (areaDefineRequest == null)
            {
                throw new ArgumentNullException(nameof(areaDefineRequest));
            }
            var areaDefine = await Store.GetAsync(a => a.Where(b => b.Code == code && !b.IsDeleted));//.GetAsync(a => a.Where(b => b.Code == code && !b.IsDeleted));
            if (areaDefine == null)
            {
                return;
            }
            areaDefine.Name = areaDefineRequest.Name;
            areaDefine.Abbreviation = areaDefineRequest.Abbreviation;
            areaDefine.Order = areaDefineRequest.Order;
            areaDefine.Desc = areaDefineRequest.Desc;

            //var newAreaDefine = _mapper.Map<AreaDefine>(areaDefineRequest);
            //    newAreaDefine.CreateUser = areaDefine.CreateUser;
            //    newAreaDefine.CreateTime = areaDefine.CreateTime;
            //    newAreaDefine.IsDeleted = areaDefine.IsDeleted;
            //    newAreaDefine.ParentId = areaDefine.ParentId;
            //    newAreaDefine.UpdateUser = userId;
            //    newAreaDefine.UpdateTime = DateTime.Now;

            await Store.UpdateAsync(areaDefine, cancellationToken);
        }
    }
}
