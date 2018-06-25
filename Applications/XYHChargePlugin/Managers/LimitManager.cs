using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Managers;
using ApplicationCore.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYHChargePlugin.Dto;
using XYHChargePlugin.Models;
using XYHChargePlugin.Stores;

namespace XYHChargePlugin.Managers
{
    public class LimitManager
    {
        
        private const string PERMISSION_BXXE = "FY_BXXE";

        public LimitManager(ILimitInfoStore store,
            ITransaction<ChargeDbContext> transaction,
            PermissionExpansionManager permissionExpansion,
            OrganizationExpansionManager organizationExpansion,
            IMapper mapper)
        {
            _Store = store;
            _transaction = transaction;
            _permissionExpansion = permissionExpansion;
            _organizationExpansion = organizationExpansion;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected ILimitInfoStore _Store { get; }

        protected PermissionExpansionManager _permissionExpansion { get; }
        protected OrganizationExpansionManager _organizationExpansion { get; }
        protected ITransaction<ChargeDbContext> _transaction { get; }
        protected IMapper _mapper { get; }

        public async Task<ResponseMessage<LimitInfoResponse>> Save(UserInfo user, LimitInfoRequest request)
        {
            ResponseMessage<LimitInfoResponse> r = new ResponseMessage<LimitInfoResponse>();

            HumanInfo ui = await _Store.GetUserInfo(request.UserId);
            if (ui == null)
            {
                r.Code = "404";
                r.Message = "人员不存在";
                return r;
            }

            await checkPermission(r, user.Id, PERMISSION_BXXE, ui.DepartmentId);

            if (!r.IsSuccess())
            {
                return r;
            }

            LimitInfo li = await _Store.Save(_mapper.Map<SimpleUser>(user), _mapper.Map<LimitInfo>(request));

            r.Extension = _mapper.Map<LimitInfoResponse>(li);

            return r;
        }

        public async Task<ResponseMessage> Delete(UserInfo user, string userId)
        {
            ResponseMessage r = new ResponseMessage();
            HumanInfo hi = await _Store.GetUserInfo(userId);
            if (hi == null)
            {
                r.Code = "404";
                r.Message = "不存在该员工";
                return r;
            }
            
            await checkPermission(r, user.Id, PERMISSION_BXXE, hi.DepartmentId);
            if (!r.IsSuccess())
            {
                return r;
            }

            await _Store.Delete(_mapper.Map<SimpleUser>(user), userId);

            return r;
        }

        public async Task<ResponseMessage<LimitInfoResponse>> GetDetail(UserInfo user,string userId)
        {
            ResponseMessage<LimitInfoResponse> r = new ResponseMessage<LimitInfoResponse>();
            LimitInfo li = await _Store.Get(_mapper.Map<SimpleUser>(user), userId);
            if (li == null)
            {
                return r;
            }
            await checkPermission(r, user.Id, PERMISSION_BXXE, li.UserInfo.DepartmentId);
            if (!r.IsSuccess())
            {
                return r;
            }

            r.Extension = _mapper.Map<LimitInfoResponse>(li);

            return r;
        }

        public async Task<PagingResponseMessage<LimitInfoResponse>> Search(UserInfo user, LimitSearchRequest request)
        {
            PagingResponseMessage<LimitInfoResponse> r = new PagingResponseMessage<LimitInfoResponse>();

            var orgList = await _permissionExpansion.GetOrganizationOfPermission(user.Id, PERMISSION_BXXE);

            var query = _Store.SimpleQuery;
            query = query.Where(c => c.IsDeleted == false);

            if (request != null)
            {
                if (!String.IsNullOrEmpty(request.DepartmentId))
                {
                    var orgChildren = await _permissionExpansion.GetLowerDepartments(request.DepartmentId);
                    orgList = orgList.Where(x => orgChildren.Contains(x)).ToList();
                }
            }

            query = query.Where(c => orgList.Contains(c.UserInfo.DepartmentId));

            if (request != null)
            {
                if (!String.IsNullOrWhiteSpace(request.Keyword))
                {
                    query = query.Where(l => ( l.UserInfo.UserID.Contains(request.Keyword) || l.UserInfo.Name.Contains(request.Keyword) || l.Memo.Contains(request.Keyword) ));
                }
                if(request.PageSize>0 && request.PageIndex > 0)
                {
                    r.TotalCount = await query.CountAsync();
                    r.PageIndex = request.PageIndex;
                    r.PageSize = request.PageSize;
                    query = query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);
                }
            }

            var list = await query.ToListAsync();
            r.Extension = _mapper.Map<List<LimitInfoResponse>>(list);
            if(r.TotalCount == 0)
            {
                r.TotalCount = r.Extension.Count;
            }

            return r;
        }

        private async Task checkPermission(ResponseMessage r, string userId, string permissionId, string branchId)
        {
            bool hasPermission = false;
            if (!String.IsNullOrEmpty(branchId))
            {
                hasPermission = await _permissionExpansion.HavePermission(userId, permissionId, branchId);
            }
            else
            {
                hasPermission = await _permissionExpansion.HavePermission(userId, permissionId);
            }
            if (!hasPermission)
            {
                r.Code = "403";
                r.Message = "没有此项操作的权限";
            }
        }

    }
}
