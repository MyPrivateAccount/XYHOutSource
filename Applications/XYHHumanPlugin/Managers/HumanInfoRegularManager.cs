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
    public class HumanInfoRegularManager
    {
        public HumanInfoRegularManager(IHumanInfoRegularStore humanInfoRegularStore,
                                    PermissionExpansionManager permissionExpansionManager,
                                    IHumanInfoStore humanInfoStore,
                                    IMapper mapper)
        {
            Store = humanInfoRegularStore;
            _permissionExpansionManager = permissionExpansionManager;
            _humanInfoStore = humanInfoStore;
            _mapper = mapper;
        }

        protected IHumanInfoRegularStore Store { get; }
        protected PermissionExpansionManager _permissionExpansionManager;
        protected IHumanInfoStore _humanInfoStore;
        protected IMapper _mapper { get; }


        public async Task<ResponseMessage<HumanInfoRegularResponse>> FindByIdAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoRegularResponse> response = new ResponseMessage<HumanInfoRegularResponse>();
            var humanInfoRegular = await Store.SimpleQuery().SingleOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (humanInfoRegular == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "没有找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanRegular");
            if (org == null || org.Count == 0 || !org.Contains(humanInfoRegular.OrganizationId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            response.Extension = _mapper.Map<HumanInfoRegularResponse>(humanInfoRegular);
            return response;
        }


        public async Task<ResponseMessage<HumanInfoRegularResponse>> CreateAsync(UserInfo user, HumanInfoRegularRequest humanInfoRegularRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoRegularResponse> response = new ResponseMessage<HumanInfoRegularResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoRegularRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoRegularRequest));
            }
            var humaninfo = await _humanInfoStore.GetAsync(a => a.Where(b => b.Id == humanInfoRegularRequest.HumanId && !b.IsDeleted), cancellationToken);
            if (humaninfo == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "操作的人事信息未找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanRegular");
            if (org == null || org.Count == 0 || !org.Contains(humaninfo.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            response.Extension = _mapper.Map<HumanInfoRegularResponse>(await Store.CreateAsync(user, _mapper.Map<HumanInfoRegular>(humanInfoRegularRequest), cancellationToken));
            return response;
        }


        public async Task<ResponseMessage<HumanInfoRegularResponse>> UpdateAsync(UserInfo user, string id, HumanInfoRegularRequest humanInfoRegularRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoRegularResponse> response = new ResponseMessage<HumanInfoRegularResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoRegularRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoRegularRequest));
            }
            var humaninfo = await _humanInfoStore.GetAsync(a => a.Where(b => b.Id == humanInfoRegularRequest.HumanId && !b.IsDeleted), cancellationToken);
            if (humaninfo == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "操作的人事信息未找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanRegular");
            if (org == null || org.Count == 0 || !org.Contains(humaninfo.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            var humanInfoRegular = _mapper.Map<HumanInfoRegular>(humanInfoRegularRequest);
            humanInfoRegular.Id = id;
            response.Extension = _mapper.Map<HumanInfoRegularResponse>(await Store.UpdateAsync(user, humanInfoRegular, cancellationToken));
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
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanRegular");
            if (org == null || org.Count == 0 || !org.Contains(old.OrganizationId))
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
