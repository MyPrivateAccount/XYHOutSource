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
    public class HumanInfoPartPositionManager
    {
        public HumanInfoPartPositionManager(IHumanInfoPartPositionStore humanInfoPartPostionStore,
            IHumanInfoStore humanInfoStore,
            PermissionExpansionManager permissionExpansionManager,
            IMapper mapper)
        {
            Store = humanInfoPartPostionStore;
            _permissionExpansionManager = permissionExpansionManager;
            _humanInfoStore = humanInfoStore;
            _mapper = mapper;
        }

        protected IHumanInfoPartPositionStore Store { get; }
        protected PermissionExpansionManager _permissionExpansionManager;
        protected IHumanInfoStore _humanInfoStore;
        protected IMapper _mapper { get; }


        public async Task<ResponseMessage<HumanInfoPartPositionResponse>> FindByIdAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoPartPositionResponse> response = new ResponseMessage<HumanInfoPartPositionResponse>();
            var humanInfoPartPostion = await Store.SimpleQuery().SingleOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (humanInfoPartPostion == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "没有找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanPartPosition");
            if (org == null || org.Count == 0 || !org.Contains(humanInfoPartPostion.DepartmentId) || !org.Contains(humanInfoPartPostion.OrganizationId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            response.Extension = _mapper.Map<HumanInfoPartPositionResponse>(humanInfoPartPostion);
            return response;
        }


        public async Task<ResponseMessage<HumanInfoPartPositionResponse>> CreateAsync(UserInfo user, HumanInfoPartPositionRequest humanInfoPartPostionRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoPartPositionResponse> response = new ResponseMessage<HumanInfoPartPositionResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoPartPostionRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoPartPostionRequest));
            }
            var humaninfo = await _humanInfoStore.GetAsync(a => a.Where(b => b.Id == humanInfoPartPostionRequest.HumanId && !b.IsDeleted), cancellationToken);
            if (humaninfo == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "操作的人事信息未找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanPartPosition");
            if (org == null || org.Count == 0 || !org.Contains(humanInfoPartPostionRequest.DepartmentId) || !org.Contains(humaninfo.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            response.Extension = _mapper.Map<HumanInfoPartPositionResponse>(await Store.CreateAsync(user, _mapper.Map<HumanInfoPartPosition>(humanInfoPartPostionRequest), cancellationToken));
            return response;
        }


        public async Task<ResponseMessage<HumanInfoPartPositionResponse>> UpdateAsync(UserInfo user, string id, HumanInfoPartPositionRequest humanInfoPartPostionRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoPartPositionResponse> response = new ResponseMessage<HumanInfoPartPositionResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoPartPostionRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoPartPostionRequest));
            }
            var humaninfo = await _humanInfoStore.GetAsync(a => a.Where(b => b.Id == humanInfoPartPostionRequest.HumanId && !b.IsDeleted), cancellationToken);
            if (humaninfo == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "操作的人事信息未找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanPartPosition");
            if (org == null || org.Count == 0 || !org.Contains(humanInfoPartPostionRequest.DepartmentId) || !org.Contains(humaninfo.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            var humanInfoPartPostion = _mapper.Map<HumanInfoPartPosition>(humanInfoPartPostionRequest);
            humanInfoPartPostion.Id = id;
            response.Extension = _mapper.Map<HumanInfoPartPositionResponse>(await Store.UpdateAsync(user, humanInfoPartPostion, cancellationToken));
            return response;
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
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanPartPosition");
            if (org == null || org.Count == 0 || !org.Contains(old.DepartmentId) || !org.Contains(humaninfo.DepartmentId))
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
