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
    public class HumanInfoAdjustmentManager
    {
        public HumanInfoAdjustmentManager(IHumanInfoAdjustmentStore humanInfoAdjustmentStore,
            IHumanInfoStore humanInfoStore,
            PermissionExpansionManager permissionExpansionManager,
            IMapper mapper)
        {
            Store = humanInfoAdjustmentStore;
            _permissionExpansionManager = permissionExpansionManager;
            _humanInfoStore = humanInfoStore;
            _mapper = mapper;
        }

        protected IHumanInfoAdjustmentStore Store { get; }
        protected PermissionExpansionManager _permissionExpansionManager;
        protected IHumanInfoStore _humanInfoStore;
        protected IMapper _mapper { get; }


        public async Task<ResponseMessage<HumanInfoAdjustmentResponse>> FindByIdAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoAdjustmentResponse> response = new ResponseMessage<HumanInfoAdjustmentResponse>();
            var humanInfoAdjustment = await Store.SimpleQuery().SingleOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (humanInfoAdjustment == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "没有找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanAdjustment");
            if (org == null || org.Count == 0 || !org.Contains(humanInfoAdjustment.DepartmentId) || !org.Contains(humanInfoAdjustment.OrganizationId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            response.Extension = _mapper.Map<HumanInfoAdjustmentResponse>(humanInfoAdjustment);
            return response;
        }


        public async Task<ResponseMessage<HumanInfoAdjustmentResponse>> CreateAsync(UserInfo user, HumanInfoAdjustmentRequest humanInfoAdjustmentRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoAdjustmentResponse> response = new ResponseMessage<HumanInfoAdjustmentResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoAdjustmentRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoAdjustmentRequest));
            }
            var humaninfo = await _humanInfoStore.GetAsync(a => a.Where(b => b.Id == humanInfoAdjustmentRequest.HumanId && !b.IsDeleted), cancellationToken);
            if (humaninfo == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "操作的人事信息未找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanAdjustment");
            if (org == null || org.Count == 0 || !org.Contains(humanInfoAdjustmentRequest.DepartmentId) || !org.Contains(humaninfo.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            response.Extension = _mapper.Map<HumanInfoAdjustmentResponse>(await Store.CreateAsync(user, _mapper.Map<HumanInfoAdjustment>(humanInfoAdjustmentRequest), cancellationToken));
            return response;
        }


        public async Task<ResponseMessage<HumanInfoAdjustmentResponse>> UpdateAsync(UserInfo user, string id, HumanInfoAdjustmentRequest humanInfoAdjustmentRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoAdjustmentResponse> response = new ResponseMessage<HumanInfoAdjustmentResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoAdjustmentRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoAdjustmentRequest));
            }
            var humaninfo = await _humanInfoStore.GetAsync(a => a.Where(b => b.Id == humanInfoAdjustmentRequest.HumanId && !b.IsDeleted), cancellationToken);
            if (humaninfo == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "操作的人事信息未找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanAdjustment");
            if (org == null || org.Count == 0 || !org.Contains(humanInfoAdjustmentRequest.DepartmentId) || !org.Contains(humaninfo.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            var humanInfoAdjustment = _mapper.Map<HumanInfoAdjustment>(humanInfoAdjustmentRequest);
            humanInfoAdjustment.Id = id;
            response.Extension = _mapper.Map<HumanInfoAdjustmentResponse>(await Store.UpdateAsync(user, humanInfoAdjustment, cancellationToken));
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
            var old = await Store.SimpleQuery().Where(a => a.Id == id).SingleOrDefaultAsync(cancellationToken);
            if (old == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "删除的对象不存在";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanAdjustment");
            if (org == null || org.Count == 0 || !org.Contains(old.DepartmentId) || !org.Contains(old.OrganizationId))
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
