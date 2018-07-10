using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Managers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHHumanPlugin.Dto.Request;
using XYHHumanPlugin.Dto.Response;
using XYHHumanPlugin.Models;
using XYHHumanPlugin.Stores;

namespace XYHHumanPlugin.Managers
{
    public class HumanInfoLeaveManager
    {
        public HumanInfoLeaveManager(IHumanInfoLeaveStore humanInfoLeaveStore,
            IHumanInfoStore humanInfoStore,
            PermissionExpansionManager permissionExpansionManager,
            IMapper mapper)
        {
            Store = humanInfoLeaveStore;
            _permissionExpansionManager = permissionExpansionManager;
            _humanInfoStore = humanInfoStore;
            _mapper = mapper;
        }

        protected IHumanInfoLeaveStore Store { get; }
        protected PermissionExpansionManager _permissionExpansionManager;
        protected IHumanInfoStore _humanInfoStore;
        protected IMapper _mapper { get; }


        public async Task<ResponseMessage<HumanInfoLeaveResponse>> FindByIdAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoLeaveResponse> response = new ResponseMessage<HumanInfoLeaveResponse>();
            var humanInfoLeave = await Store.SimpleQuery().SingleOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (humanInfoLeave == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "没有找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanLeave");
            if (org == null || org.Count == 0 || !org.Contains(humanInfoLeave.OrganizationId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            response.Extension = _mapper.Map<HumanInfoLeaveResponse>(humanInfoLeave);
            return response;
        }


        public async Task<ResponseMessage<HumanInfoLeaveResponse>> CreateAsync(UserInfo user, HumanInfoLeaveRequest humanInfoLeaveRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoLeaveResponse> response = new ResponseMessage<HumanInfoLeaveResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoLeaveRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoLeaveRequest));
            }
            var humaninfo = await _humanInfoStore.GetAsync(a => a.Where(b => b.Id == humanInfoLeaveRequest.HumanId && !b.IsDeleted), cancellationToken);
            if (humaninfo == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "操作的人事信息未找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanLeave");
            if (org == null || org.Count == 0 || !org.Contains(humaninfo.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            response.Extension = _mapper.Map<HumanInfoLeaveResponse>(await Store.CreateAsync(user, _mapper.Map<HumanInfoLeave>(humanInfoLeaveRequest), cancellationToken));
            return response;
        }


        public async Task<ResponseMessage<HumanInfoLeaveResponse>> UpdateAsync(UserInfo user, string id, HumanInfoLeaveRequest humanInfoLeaveRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoLeaveResponse> response = new ResponseMessage<HumanInfoLeaveResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoLeaveRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoLeaveRequest));
            }
            var humaninfo = await _humanInfoStore.GetAsync(a => a.Where(b => b.Id == humanInfoLeaveRequest.HumanId && !b.IsDeleted), cancellationToken);
            if (humaninfo == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "操作的人事信息未找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanLeave");
            if (org == null || org.Count == 0 || !org.Contains(humaninfo.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            var humanInfoLeave = _mapper.Map<HumanInfoLeave>(humanInfoLeaveRequest);
            humanInfoLeave.Id = id;
            response.Extension = _mapper.Map<HumanInfoLeaveResponse>(await Store.UpdateAsync(user, humanInfoLeave, cancellationToken));

            return response;
        }

        /// <summary>
        /// 更新审核状态
        /// </summary>
        /// <param name="humanId"></param>
        /// <param name="status"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateExamineStatus(string humanId, ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Store.UpdateExamineStatus(humanId, status, cancellationToken);
        }

        public async Task<ResponseMessage> DeleteAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage response = new ResponseMessage();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            var old = await Store.GetAsync(a => a.Where(b => b.Id == id));
            if (old == null)
            {
                throw new Exception("删除的对象不存在");
            }
            var humaninfo = await _humanInfoStore.GetAsync(a => a.Where(b => b.Id == old.HumanId && !b.IsDeleted), cancellationToken);
            if (humaninfo == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "操作的人事信息未找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanLeave");
            if (org == null || org.Count == 0 || !org.Contains(humaninfo.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            await Store.DeleteAsync(user, old, cancellationToken);
            return response;
        }
    }
}
