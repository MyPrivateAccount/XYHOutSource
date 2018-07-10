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
    public class PositionSalaryManager
    {

        public PositionSalaryManager(IPositionSalaryStore positionSalaryStore,
            PermissionExpansionManager permissionExpansionManager,
            IMapper mapper)
        {
            Store = positionSalaryStore;
            _permissionExpansionManager = permissionExpansionManager;
            _mapper = mapper;
        }

        protected IPositionSalaryStore Store { get; }
        protected PermissionExpansionManager _permissionExpansionManager;
        protected IMapper _mapper { get; }


        public async Task<ResponseMessage<PositionSalaryResponse>> FindByIdAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<PositionSalaryResponse> response = new ResponseMessage<PositionSalaryResponse>();
            var positionSalary = await Store.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);

            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanPositionSalary");
            if (org == null || org.Count == 0)
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            response.Extension = _mapper.Map<PositionSalaryResponse>(positionSalary);
            return response;
        }


        public async Task<ResponseMessage<PositionSalaryResponse>> CreateAsync(UserInfo user, PositionSalaryRequest positionSalaryRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<PositionSalaryResponse> response = new ResponseMessage<PositionSalaryResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (positionSalaryRequest == null)
            {
                throw new ArgumentNullException(nameof(positionSalaryRequest));
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanPositionSalary");
            if (org == null || org.Count == 0)
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            response.Extension = _mapper.Map<PositionSalaryResponse>(await Store.CreateAsync(user, _mapper.Map<PositionSalary>(positionSalaryRequest), cancellationToken));
            return response;
        }


        public async Task<ResponseMessage<PositionSalaryResponse>> UpdateAsync(UserInfo user, string id, PositionSalaryRequest positionSalaryRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<PositionSalaryResponse> response = new ResponseMessage<PositionSalaryResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (positionSalaryRequest == null)
            {
                throw new ArgumentNullException(nameof(positionSalaryRequest));
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanPositionSalary");
            if (org == null || org.Count == 0)
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            var positionSalary = _mapper.Map<PositionSalary>(positionSalaryRequest);
            positionSalary.Id = id;
            response.Extension = _mapper.Map<PositionSalaryResponse>(await Store.UpdateAsync(user, positionSalary, cancellationToken));

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
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanPositionSalary");
            if (org == null || org.Count == 0)
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
