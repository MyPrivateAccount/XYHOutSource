using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Managers;
using AutoMapper;
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
    public class HumanPositionManager
    {
        public HumanPositionManager(IHumanPositionStore humanPositionStore,
            PermissionExpansionManager permissionExpansionManager,
            IMapper mapper)
        {
            Store = humanPositionStore;
            _permissionExpansionManager = permissionExpansionManager;
            _mapper = mapper;
        }

        protected IHumanPositionStore Store { get; }
        protected PermissionExpansionManager _permissionExpansionManager;
        protected IMapper _mapper { get; }


        public async Task<ResponseMessage<HumanPositionResponse>> FindByIdAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanPositionResponse> response = new ResponseMessage<HumanPositionResponse>();
            var humanPosition = await Store.GetAsync(a => a.Where(b => b.Id == id));
            if (humanPosition == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "没有找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanPosition");
            if (org == null || org.Count == 0 || !org.Contains(humanPosition.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            response.Extension = _mapper.Map<HumanPositionResponse>(humanPosition);
            return response;
        }


        public async Task<ResponseMessage<HumanPositionResponse>> CreateAsync(UserInfo user, HumanPositionRequest humanPositionRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanPositionResponse> response = new ResponseMessage<HumanPositionResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanPositionRequest == null)
            {
                throw new ArgumentNullException(nameof(humanPositionRequest));
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanPosition");
            if (org == null || org.Count == 0 || !org.Contains(humanPositionRequest.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            response.Extension = _mapper.Map<HumanPositionResponse>(await Store.CreateAsync(user, _mapper.Map<HumanPosition>(humanPositionRequest), cancellationToken));
            return response;
        }


        public async Task<ResponseMessage<HumanPositionResponse>> UpdateAsync(UserInfo user, string id, HumanPositionRequest humanPositionRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanPositionResponse> response = new ResponseMessage<HumanPositionResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanPositionRequest == null)
            {
                throw new ArgumentNullException(nameof(humanPositionRequest));
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanPosition");
            if (org == null || org.Count == 0 || !org.Contains(humanPositionRequest.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            var humanPosition = _mapper.Map<HumanPosition>(humanPositionRequest);
            humanPosition.Id = id;
            response.Extension = _mapper.Map<HumanPositionResponse>(await Store.UpdateAsync(user, humanPosition, cancellationToken));

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
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanPosition");
            if (org == null || org.Count == 0 || !org.Contains(old.DepartmentId))
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
