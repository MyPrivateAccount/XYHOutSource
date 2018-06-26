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
    public class BorrowingManager
    {
        private readonly List<string> orgTypes = new List<string>() { "Filiale", "Subsidiary", "Bloc" };

        private const string PERMISSION_YJKGL = "FY_YJKGL";
        private const string PERMISSION_YJK = "FY_YJKSQ";
        private const string PERMISSION_YJKQR = "FY_YJKQR";
        private const string PERMISSION_YJKFK = "FY_YJKFK";

        public BorrowingManager(IBorrowingStore stor,
            IChargeInfoStore chargeStore,
            IOrganizationUtils orgUtils,
            ITransaction<ChargeDbContext> transaction,
            PermissionExpansionManager permissionExpansion,
            OrganizationExpansionManager organizationExpansion,
            IMapper mapper)
        {
            _Store = stor;
            _chargeStor = chargeStore;
            _transaction = transaction;
            _permissionExpansion = permissionExpansion;
            _organizationExpansion = organizationExpansion;
            _orgUtils = orgUtils;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IBorrowingStore _Store { get; }
        protected IChargeInfoStore _chargeStor { get; }
        protected IOrganizationUtils _orgUtils { get; }

        protected PermissionExpansionManager _permissionExpansion { get; }
        protected OrganizationExpansionManager _organizationExpansion { get; }
        protected ITransaction<ChargeDbContext> _transaction { get; }
        protected IMapper _mapper { get; }

        public async Task<ResponseMessage<ChargeInfoResponse>> Save(UserInfo user, ChargeInfoRequest request)
        {
            ResponseMessage<ChargeInfoResponse> r = new ResponseMessage<ChargeInfoResponse>();

            await checkInputPermission(r, user, request.ReimburseDepartment, request.ReimburseUser);
            if (!r.IsSuccess())
            {
                return r;
            }


            using (var t = await _transaction.BeginTransaction())
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(request.BranchId))
                    {
                        var org = await _orgUtils.GetNearParent(request.ReimburseDepartment, orgTypes);
                        if (org != null)
                        {
                            request.BranchId = org.Id;
                        }
                        else
                        {
                            request.BranchId = request.ReimburseDepartment;
                        }
                    }
                    if (String.IsNullOrEmpty(request.Department))
                    {
                        request.Department = user.OrganizationId;
                    }
                    if (String.IsNullOrWhiteSpace(request.ChargeNo))
                    {
                        string prefix = await _orgUtils.GetBranchPrefix(request.BranchId, "XYH");
                        request.BranchPrefix = prefix;
                        int seq = await _Store.GetChargeNo(request.BranchId, prefix, DateTime.Now, request.Type);
                        request.Seq = seq;

                        string bt = "JK";
                        if(request.Type == 3)
                        {
                            bt = "HK";
                        }
                        request.ChargeNo = String.Format("{0}{1}{2}{3:D5}", bt, prefix, DateTime.Now.ToString("yyyyMMdd"), seq);
                    }
                    var ci = _mapper.Map<ChargeInfo>(request);

                    //预借款冲减
                    if (request.Status == (int)ChargeStatus.Submit)
                    {
                        if (!String.IsNullOrEmpty(request.ChargeId) && request.Type == 3)
                        {
                            var li = await _Store.UpdateReimbursedAmount(_mapper.Map<SimpleUser>(user), ci);
                            decimal total = li.TotalAmount - li.UnSubmitAmount + (ci.ReimbursedAmount ?? 0);
                            if (li.LimitAmount < total)
                            {
                                r.Code = "410";
                                r.Message = $"预借款冲减超额：预借款总金额 {li.LimitAmount}元，已报销：{total}元，超出：{total - li.LimitAmount}元";

                                t.Rollback();
                                return r;
                            }
                        }
                    }

                    await _Store.Save(_mapper.Map<SimpleUser>(user), ci);

                    r.Extension = _mapper.Map<ChargeInfoResponse>(ci);

                    t.Commit();
                }
                catch (Exception e)
                {
                    t.Rollback();
                    throw;
                }
            }


            return r;
        }


        public async Task<PagingResponseMessage<ChargeInfoResponse>> Search(UserInfo user, ChargeSearchRequest request, string permissionId)
        {
            PagingResponseMessage<ChargeInfoResponse> r = new PagingResponseMessage<ChargeInfoResponse>();

            bool hasYjk = await _permissionExpansion.HavePermission(user.Id, PERMISSION_YJK);
            if (String.IsNullOrEmpty(permissionId))
            {
                permissionId = PERMISSION_YJKGL;
            }

            var orgList = await _permissionExpansion.GetOrganizationOfPermission(user.Id, permissionId);
            List<int> types = new List<int>() { 2, 3 };
            var query = _Store.SimpleQuery;
            query = query.Where(c => c.IsDeleted == false && types.Contains(c.Type));
            if (orgList.Count == 0)
            {
                if (!hasYjk)
                {
                    r.Extension = new List<ChargeInfoResponse>();
                    return r;
                }
                query = query.Where(x => x.ReimburseUser == user.Id);
            }
            else
            {
                if (request != null)
                {
                    if (!String.IsNullOrEmpty(request.ReimburseDepartment))
                    {
                        var orgChildren = await _permissionExpansion.GetLowerDepartments(request.ReimburseDepartment);
                        orgList = orgList.Where(x => orgChildren.Contains(x)).ToList();
                    }
                }

                query = query.Where(c => orgList.Contains(c.ReimburseDepartment));
            }


            if (request != null)
            {
                if (request.StartDate != null)
                {
                    var dt = request.StartDate.Value.Date;
                    query = query.Where(c => c.CreateTime >= dt);
                }
                if (request.EndDate != null)
                {
                    var dt = request.EndDate.Value.Date.AddDays(1);
                    query = query.Where(c => c.CreateTime < dt);
                }
                if (request.IsPayment != null)
                {
                    query = query.Where(c => c.IsPayment == request.IsPayment);
                }
               
                if (!String.IsNullOrWhiteSpace(request.Keyword))
                {
                    query = query.Where(c => (c.ReimburseUserInfo.Name.Contains(request.Keyword) || c.ReimburseUserInfo.UserID.Contains(request.Keyword) || c.Memo.Contains(request.Keyword) || c.ChargeNo.Contains(request.Keyword)));
                }
                if (request.Status != null && request.Status.Count > 0)
                {
                    query = query.Where(c => request.Status.Contains(c.Status));
                }
                

                if (request.PageIndex > 0 && request.PageSize > 0)
                {
                    r.TotalCount = await query.CountAsync();
                    r.PageSize = request.PageSize;
                    r.PageIndex = request.PageIndex;
                    query = query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);
                }
            }



            var list = await query.ToListAsync();

            r.Extension = _mapper.Map<List<ChargeInfoResponse>>(list);
            if (r.TotalCount == 0)
            {
                r.TotalCount = r.Extension.Count;
            }

            return r;
        }

        public async Task<ResponseMessage> DeleteBorrowing(UserInfo user, string id)
        {
            ResponseMessage r = new ResponseMessage();
            var cit = await _Store.Get(id);
            if (cit == null)
            {
                r.Code = "404";
                r.Message = "预借款单据不存在";
                return r;
            }

            await checkInputPermission(r, user, cit.ReimburseDepartment, cit.ReimburseUser);
            if (!r.IsSuccess())
            {
                return r;
            }
            

            await _Store.DeleteBorrowing(_mapper.Map<SimpleUser>(user), id);

            return r;
        }

        public async Task<ResponseMessage> Submit(UserInfo user, string id)
        {
            ResponseMessage r = new ResponseMessage();
            var cit = await _Store.Get(id);
            if (cit == null)
            {
                r.Code = "404";
                r.Message = "预借款不存在";
                return r;
            }

            await checkInputPermission(r, user, cit.ReimburseDepartment, cit.ReimburseUser);
            if (!r.IsSuccess())
            {
                return r;
            }

            //预借款冲减
           
            if (!String.IsNullOrEmpty(cit.ChargeId) && cit.Type == 3)
            {
                var li = await _Store.UpdateReimbursedAmount(_mapper.Map<SimpleUser>(user), cit);
                decimal total = li.TotalAmount - li.UnSubmitAmount + (cit.ReimbursedAmount ?? 0);
                if (li.LimitAmount < total)
                {
                    r.Code = "410";
                    r.Message = $"预借款冲减超额：预借款总金额 {li.LimitAmount}元，已报销：{total}元，超出：{total - li.LimitAmount}元";

                    return r;
                }
            }
            

            await _Store.UpdateStatus(_mapper.Map<SimpleUser>(user), id, (int)ChargeStatus.Submit, null, ModifyTypeEnum.Submit, ModifyTypeConstans.Submit);

            return r;
        }

        public async Task<ResponseMessage> Confirm(UserInfo user, ConfirmRequest request)
        {
            ResponseMessage r = new ResponseMessage();
            var ci = await _Store.Get(request.Id);
            if (ci == null)
            {
                r.Code = "404";
                r.Message = "预借款不存在";
                return r;
            }

            await checkPermission(r, user.Id, PERMISSION_YJKQR, ci.ReimburseDepartment);
            if (!r.IsSuccess())
            {
                return r;
            }

            if (request.Status == (int)ChargeStatus.Confirm)
            {
                await _Store.UpdateStatus(_mapper.Map<SimpleUser>(user), request.Id, (int)ChargeStatus.Confirm, request.Message, ModifyTypeEnum.Confirm, ModifyTypeConstans.Confirm);
            }
            else if (request.Status == (int)ChargeStatus.Reject)
            {
                await _Store.UpdateStatus(_mapper.Map<SimpleUser>(user), request.Id, (int)ChargeStatus.Reject, request.Message, ModifyTypeEnum.Reject, ModifyTypeConstans.Reject);
            }
            return r;
        }


        public async Task<ResponseMessage<PaymentInfoResponse>> Payment(UserInfo user, PaymentInfoRequest request)
        {

            ResponseMessage<PaymentInfoResponse> r = new ResponseMessage<PaymentInfoResponse>();
            var ci = await _Store.Get(request.ChargeId);
            if (ci == null)
            {
                r.Code = "404";
                r.Message = "预借款不存在";
                return r;
            }

            await checkPermission(r, user.Id, PERMISSION_YJKFK, ci.ReimburseDepartment);

            if (String.IsNullOrEmpty(request.Department))
            {
                request.Department = user.OrganizationId;
            }

            if (String.IsNullOrWhiteSpace(request.BranchId))
            {
                var org = await _orgUtils.GetNearParent(request.Department, orgTypes);
                if (org != null)
                {
                    request.BranchId = org.Id;
                }
                else
                {
                    request.BranchId = request.Department;
                }
            }

            if (String.IsNullOrWhiteSpace(request.PaymentNo))
            {
                string prefix = await _orgUtils.GetBranchPrefix(request.BranchId, "XYH");
                request.BranchPrefix = prefix;
                int seq = await _chargeStor.GetPaymentNo(request.BranchId, prefix, DateTime.Now);
                request.Seq = seq;

                request.PaymentNo = String.Format("FK{0}{1}{2:D5}", prefix, DateTime.Now.ToString("yyyyMMdd"), seq);
            }

            var pi = _mapper.Map<PaymentInfo>(request);

            var r2 = await _chargeStor.Payment(_mapper.Map<SimpleUser>(user), pi);
            r.Extension = _mapper.Map<PaymentInfoResponse>(r2);
            return r;

        }


        public async Task<ResponseMessage> RecordingConfirm(UserInfo user, ConfirmRequest request)
        {

            ResponseMessage r = new ResponseMessage();
            var ci = await _Store.Get(request.Id);
            if (ci == null)
            {
                r.Code = "404";
                r.Message = "还款单不存在";
                return r;
            }

            await checkPermission(r, user.Id, PERMISSION_YJKFK, ci.ReimburseDepartment);
            if (!r.IsSuccess())
            {
                return r;
            }
           
            
            await _Store.UpdateRecordingStatus(_mapper.Map<SimpleUser>(user), request.Id, request.Status, "", ModifyTypeEnum.RecordingConfirm, ModifyTypeConstans.RecordingConfirm);
          
            return r;

        }


        public async Task<ChargeInfoResponse> GetDetail(UserInfo user, string id)
        {
            var ci = await _Store.GetDetail(_mapper.Map<SimpleUser>(user), id);
            if (ci == null)
                return null;

            return _mapper.Map<ChargeInfoResponse>(ci);
        }


        private async Task checkInputPermission(ResponseMessage r, UserInfo ui,  string branchId, string userId)
        {
            bool hasPermission = false;
            bool hp = await _permissionExpansion.HavePermission(ui.Id, PERMISSION_YJK);
            if(hp && ui.OrganizationId == branchId && userId==ui.Id)
            {
                return;
            }

            if (!String.IsNullOrEmpty(branchId))
            {
                hasPermission = await _permissionExpansion.HavePermission(userId, PERMISSION_YJKGL, branchId);
            }
            else
            {
                hasPermission = await _permissionExpansion.HavePermission(userId, PERMISSION_YJKGL);
            }
            if (!hasPermission)
            {
                r.Code = "403";
                r.Message = "没有此项操作的权限";
            }
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
