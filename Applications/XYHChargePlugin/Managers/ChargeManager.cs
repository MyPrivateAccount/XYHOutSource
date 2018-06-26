using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XYHChargePlugin.Dto;
using XYHChargePlugin.Models;
using XYHChargePlugin.Stores;
using System.Linq;
using ApplicationCore.Managers;
using Microsoft.EntityFrameworkCore;

namespace XYHChargePlugin.Managers
{
    public class ChargeManager
    {
        private readonly List<string> orgTypes = new List<string>() { "Filiale", "Subsidiary", "Bloc" };

        private const string PERMISSION_FYGL = "FY_GL";
        private const string PERMISSION_FYFK = "FY_FK";
        private const string PERMISSION_BXDQR = "FY_BXDQR";
        private const string PERMISSION_BXMD = "FY_BXMD";
        private const string PERMISSION_MXB = "FY_MXB";
        private const string PERMISSION_HZB = "FY_HZB";

        public ChargeManager(IChargeInfoStore stor,
            ILimitInfoStore limitStore,
            IBorrowingStore borrowingStore,
            IOrganizationUtils orgUtils,
            ITransaction<ChargeDbContext> transaction,
            PermissionExpansionManager permissionExpansion,
            OrganizationExpansionManager organizationExpansion,
            IMapper mapper)
        {
            _Store = stor;
            _limitStore = limitStore;
            _borrowingStore = borrowingStore;
            _transaction = transaction;
            _permissionExpansion = permissionExpansion;
            _organizationExpansion = organizationExpansion;
            _orgUtils = orgUtils;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }



        protected IChargeInfoStore _Store { get; }

        protected ILimitInfoStore _limitStore { get; }
        protected IBorrowingStore _borrowingStore { get; }
        protected IOrganizationUtils _orgUtils { get; }

        protected PermissionExpansionManager _permissionExpansion { get; }
        protected OrganizationExpansionManager _organizationExpansion { get; }
        protected ITransaction<ChargeDbContext> _transaction { get; }
        protected IMapper _mapper { get; }

        public async Task<ResponseMessage<ChargeInfoResponse>> Save(UserInfo user, ChargeInfoRequest request)
        {
            ResponseMessage<ChargeInfoResponse> r = new ResponseMessage<ChargeInfoResponse>();

            var hasPermission = await _permissionExpansion.HavePermission(user.Id, "FY_BXMD", request.ReimburseDepartment);
            if (!hasPermission)
            {
                r.Code = "403";
                r.Message = "没有权限";
                return r;
            }
            if (request.FeeList == null)
            {
                request.FeeList = new List<CostInfoRequest>();
            }
            if (request.BillList == null)
            {
                request.BillList = new List<ReceiptInfoRequest>();
            }
            request.FeeList.ForEach(item => item.ChargeId = request.ID);
            request.BillList.ForEach(item => item.ChargeId = request.ID);
            request.BillList.ForEach(item =>
            {
                if (item.FileScopes == null)
                {
                    item.FileScopes = new List<FileScopeRequest>();
                }
                item.FileScopes.ForEach(fs =>
                {
                    fs.ReceiptId = item.Id;
                    if (fs.FileList == null)
                    {
                        fs.FileList = new List<FileInfoRequest>();
                    }
                    fs.FileList.ForEach(f =>
                    {
                        if (String.IsNullOrEmpty(f.FileExt))
                        {
                            f.FileExt = System.IO.Path.GetExtension(f.Name ?? "");
                        }
                    });
                });
            });

            request.ChargeAmount = request.FeeList.Sum(x => x.Amount);
            request.BillAmount = request.BillList.Sum(x => x.ReceiptMoney);
            request.BillCount = request.BillList.Count;

            if(request.Status == (int)ChargeStatus.Submit)
            {
                DateTime dt = DateTime.Now;
                DateTime start = new DateTime(dt.Year, dt.Month, 1);
                DateTime end = start.AddMonths(1);

                LimitTipInfo ti = await _Store.GetLimitTip(request.ReimburseUser, start, end, request.ID);
                if (ti.LimitAmount > 0)
                {
                    //费用限制
                    var dl = await _Store.GetDictionaryDefine("CHARGE_COST_TYPE");
                    decimal totalLimit =  request.FeeList.Where(x => dl.Any(d => d.Value == x.Type.ToString() && d.Ext1 == "1")).Sum(x => x.Amount);
                   
                    decimal ybx = ti.TotalAmount - ti.UnSubmitAmount + totalLimit;
                    if (ybx > ti.LimitAmount)
                    {
                        r.Code = "410";
                        r.Message = $"费用超出限额：可报销费用总额 {ti.LimitAmount}元，已报销：{ybx}元，超出：{ybx - ti.LimitAmount}元";

                        return r;
                    }
                }

                
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

                        request.ChargeNo = String.Format("FY{0}{1}{2:D5}", prefix, DateTime.Now.ToString("yyyyMMdd"), seq);
                    }
                    var ci = _mapper.Map<ChargeInfo>(request);

                    //预借款冲减
                    if( request.Status == (int)ChargeStatus.Submit)
                    {
                        if (!String.IsNullOrEmpty(request.ChargeId))
                        {
                            var li = await _borrowingStore.UpdateReimbursedAmount(_mapper.Map<SimpleUser>(user), ci);
                            decimal total = li.TotalAmount - li.UnSubmitAmount + (ci.ReimbursedAmount??0);
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

            if (String.IsNullOrEmpty(permissionId))
            {
                permissionId = PERMISSION_FYGL;
            }

            var orgList = await _permissionExpansion.GetOrganizationOfPermission(user.Id, permissionId);

            var query = _Store.SimpleQuery;
            query = query.Where(c => c.IsDeleted == false);
            if (orgList.Count == 0)
            {
                query = query.Where(x => x.CreateUser == user.Id);
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
                if (request.IsBackup != null)
                {
                    query = query.Where(c => c.IsBackup == request.IsBackup);
                }
                if (!String.IsNullOrWhiteSpace(request.Keyword))
                {
                    query = query.Where(c => (c.ReimburseUserInfo.Name.Contains(request.Keyword) || c.ReimburseUserInfo.UserID.Contains(request.Keyword) || c.Memo.Contains(request.Keyword) || c.ChargeNo.Contains(request.Keyword)));
                }
                if (request.Status != null && request.Status.Count > 0)
                {
                    query = query.Where(c => request.Status.Contains(c.Status));
                }
                if (request.BillStatus != null && request.BillStatus.Count > 0)
                {
                    query = query.Where(c => request.BillStatus.Contains(c.BillStatus));
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

        public async Task<LimitTipResponse> GetLimitTip(UserInfo user, string userId, DateTime? date, string chargeId)
        {
            DateTime sd = date ?? DateTime.Now;
            sd = new DateTime(sd.Year, sd.Month, 1);
            DateTime ed = sd.AddMonths(1);

            var li = await _Store.GetLimitTip(userId, sd,ed, chargeId);

            return _mapper.Map<LimitTipResponse>(li);
        }
        public async Task<ChargeInfoResponse> GetDetail(UserInfo user, string id)
        {
            var ci = await _Store.GetDetail(_mapper.Map<SimpleUser>(user), id);
            if (ci == null)
                return null;

            return _mapper.Map<ChargeInfoResponse>(ci);
        }

        public async Task<ResponseMessage> DeleteCharge(UserInfo user, string id)
        {
            ResponseMessage r = new ResponseMessage();
            var cit = await _Store.Get(id);
            if (cit == null)
            {
                r.Code = "404";
                r.Message = "报销单不存在";
                return r;
            }
            bool hp = false;
            if (cit.CreateUser == user.Id)
            {
                hp = true;
            }
            else
            {
                hp = await _permissionExpansion.HavePermission(user.Id, PERMISSION_FYGL, cit.ReimburseDepartment);
            }
            if (!hp)
            {
                r.Code = "403";
                r.Message = "没有该报销单的操作权限";
                return r;
            }

            await _Store.DeleteCharge(_mapper.Map<SimpleUser>(user), id);

            return r;
        }

        public async Task<ResponseMessage> Submit(UserInfo user, string id)
        {
            ResponseMessage r = new ResponseMessage();
            var cit = await _Store.Get(id);
            if (cit == null)
            {
                r.Code = "404";
                r.Message = "报销单不存在";
                return r;
            }
            bool hp = false;
            if (cit.CreateUser == user.Id)
            {
                hp = true;
            }
            else
            {
                hp = await _permissionExpansion.HavePermission(user.Id, PERMISSION_FYGL, cit.ReimburseDepartment);
            }
            if (!hp)
            {
                r.Code = "403";
                r.Message = "没有该报销单的操作权限";
                return r;
            }

            DateTime dt = DateTime.Now;
            DateTime start = new DateTime(dt.Year, dt.Month, 1);
            DateTime end = start.AddMonths(1);

            LimitTipInfo ti = await _Store.GetLimitTip(cit.ReimburseUser, start, end, cit.ID);
            if(ti.LimitAmount > 0)
            {
                //费用限制
                var bl = await _Store.GetBillList(cit.ID);
                decimal totalLimit = bl.Where(x => x.TypeInfo.Ext1 == "1").Sum(x=>x.Amount);
                decimal ybx = ti.TotalAmount - ti.UnSubmitAmount + totalLimit;
                if(ybx > ti.LimitAmount)
                {
                    r.Code = "410";
                    r.Message = $"费用超出限额：可报销费用总额 {ti.LimitAmount}元，已报销：{ybx}元，超出：{ybx-ti.LimitAmount}元";
                    
                    return r;
                }
            }

            if (!String.IsNullOrEmpty(cit.ChargeId))
            {
                var li = await _borrowingStore.UpdateReimbursedAmount(_mapper.Map<SimpleUser>(user), cit);
                decimal total = li.TotalAmount - li.UnSubmitAmount + (cit.ReimbursedAmount??0);
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
                r.Message = "报销单不存在";
                return r;
            }
            bool hp = await _permissionExpansion.HavePermission(user.Id, PERMISSION_BXDQR, ci.ReimburseDepartment);

            if (!hp)
            {
                r.Code = "403";
                r.Message = "没有该报销单的操作权限";
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

        public async Task<ResponseMessage> ConfirmBill(UserInfo user, ConfirmRequest request)
        {
            ResponseMessage r = new ResponseMessage();
            var ci = await _Store.Get(request.Id);
            if (ci == null)
            {
                r.Code = "404";
                r.Message = "报销单不存在";
                return r;
            }
            bool hp = await _permissionExpansion.HavePermission(user.Id, PERMISSION_BXDQR, ci.ReimburseDepartment);

            if (!hp)
            {
                r.Code = "403";
                r.Message = "没有该报销单的操作权限";
                return r;
            }

            if (request.Status == (int)ChargeStatus.Confirm)
            {
                await _Store.UpdateBillStatus(_mapper.Map<SimpleUser>(user), request.Id, (int)BillStatusEnum.Confirm, request.Message, ModifyTypeEnum.ConfirmBill, ModifyTypeConstans.ConfirmBill);
            }
            else if (request.Status == (int)ChargeStatus.Reject)
            {
                await _Store.UpdateBillStatus(_mapper.Map<SimpleUser>(user), request.Id, (int)BillStatusEnum.Reject, request.Message, ModifyTypeEnum.RejectBill, ModifyTypeConstans.RejectBill);
            }

            return r;

        }

        public async Task<ResponseMessage<ChargeInfoResponse>> Backup(UserInfo user, ChargeInfoRequest request)
        {
            ResponseMessage<ChargeInfoResponse> r = new ResponseMessage<ChargeInfoResponse>();
            var cit = await _Store.Get(request.ID);
            if (cit == null)
            {
                r.Code = "404";
                r.Message = "报销单不存在";
                return r;
            }
            bool hp = false;
            if (cit.CreateUser == user.Id)
            {
                hp = true;
            }
            else
            {
                hp = await _permissionExpansion.HavePermission(user.Id, PERMISSION_FYGL, cit.ReimburseDepartment);
            }
            if (!hp)
            {
                r.Code = "403";
                r.Message = "没有该报销单的操作权限";
                return r;
            }

            var ci = _mapper.Map<ChargeInfo>(request);
            ci.BillList.ForEach(item =>
            {
                item.ChargeId = request.ID;
                if (item.FileScopes == null)
                {
                    item.FileScopes = new List<FileScopeInfo>();
                }
                item.FileScopes.ForEach(f =>
                {
                    f.ReceiptId = item.Id;
                });
            });
            ci.BillCount = ci.BillList.Count;
            ci.BillAmount = ci.BillList.Sum(x => x.ReceiptMoney);
            ci.Backuped = true;

            ci = await _Store.BackupBill(_mapper.Map<SimpleUser>(user), ci);

            r.Extension = _mapper.Map<ChargeInfoResponse>(ci);

            return r;
        }


        public async Task<ResponseMessage<PaymentInfoResponse>> Payment(UserInfo user, PaymentInfoRequest request)
        {

            ResponseMessage<PaymentInfoResponse> r = new ResponseMessage<PaymentInfoResponse>();
            var ci = await _Store.Get(request.ChargeId);
            if (ci == null)
            {
                r.Code = "404";
                r.Message = "报销单不存在";
                return r;
            }
            bool hp = await _permissionExpansion.HavePermission(user.Id, PERMISSION_FYFK, ci.ReimburseDepartment);

            if (!hp)
            {
                r.Code = "403";
                r.Message = "没有该报销单的操作权限";
                return r;
            }

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
                int seq = await _Store.GetPaymentNo(request.BranchId, prefix, DateTime.Now);
                request.Seq = seq;

                request.PaymentNo = String.Format("FK{0}{1}{2:D5}", prefix, DateTime.Now.ToString("yyyyMMdd"), seq);
            }

            var pi = _mapper.Map<PaymentInfo>(request);

            var r2 = await _Store.Payment(_mapper.Map<SimpleUser>(user), pi);
            r.Extension = _mapper.Map<PaymentInfoResponse>(r2);
            return r;

        }

        public async Task<ResponseMessage> SubmitBill(UserInfo user, string id)
        {
            ResponseMessage r = new ResponseMessage();
            var ci = await _Store.Get(id);
            if (ci == null)
            {
                r.Code = "404";
                r.Message = "报销单不存在";
                return r;
            }
            bool hp = false;
            if (ci.CreateUser == user.Id)
            {
                hp = true;
            }
            else
            {
                hp = await _permissionExpansion.HavePermission(user.Id, PERMISSION_FYGL, ci.ReimburseDepartment);
            }
            if (!hp)
            {
                r.Code = "403";
                r.Message = "没有该报销单的操作权限";
                return r;
            }

            await _Store.UpdateBillStatus(_mapper.Map<SimpleUser>(user), id, (int)BillStatusEnum.Submit, null, ModifyTypeEnum.SubmitBill, ModifyTypeConstans.SubmitBill);
            return r;
        }


        public async Task<PagingResponseMessage<CostInfoResponse>> SearchCost(UserInfo user, CostSearchRequest request)
        {
            PagingResponseMessage<CostInfoResponse> r = new PagingResponseMessage<CostInfoResponse>();

            var query = _Store.CostQuery;

            List<int> statusList = new List<int>() { (int)ChargeStatus.Confirm };
            var orgList = await _permissionExpansion.GetOrganizationOfPermission(user.Id, PERMISSION_MXB);

            query = query.Where(c => c.ChargeInfo.IsDeleted == false && statusList.Contains(c.ChargeInfo.Status));

            if (request != null)
            {
                if (!String.IsNullOrEmpty(request.ReimburseDepartment))
                {
                    var orgChildren = await _permissionExpansion.GetLowerDepartments(request.ReimburseDepartment);
                    orgList = orgList.Where(x => orgChildren.Contains(x)).ToList();
                }
            }

            query = query.Where(c => orgList.Contains(c.ChargeInfo.ReimburseDepartment));

            if(request != null)
            {
                if (request.StartDate != null)
                {
                    var dt = request.StartDate.Value.Date;
                    query = query.Where(c => c.ChargeInfo.CreateTime >= dt);
                }
                if (request.EndDate != null)
                {
                    var dt = request.EndDate.Value.Date.AddDays(1);
                    query = query.Where(c => c.ChargeInfo.CreateTime < dt);
                }
                if (request.IsPayment != null)
                {
                    query = query.Where(c => c.ChargeInfo.IsPayment == request.IsPayment);
                }
                if (request.IsBackup != null)
                {
                    query = query.Where(c => c.ChargeInfo.IsBackup == request.IsBackup);
                }
                if (!String.IsNullOrWhiteSpace(request.Keyword))
                {
                    query = query.Where(c => (c.ChargeInfo.ReimburseUserInfo.Name.Contains(request.Keyword) || c.ChargeInfo.ReimburseUserInfo.UserID.Contains(request.Keyword) || c.Memo.Contains(request.Keyword) || c.ChargeInfo.Memo.Contains(request.Keyword) || c.ChargeInfo.ChargeNo.Contains(request.Keyword)));
                }

                if (request.PageIndex>0 && request.PageSize > 0)
                {
                    r.TotalCount = await query.CountAsync();
                    r.PageSize = request.PageSize;
                    r.PageIndex = request.PageIndex;

                    query = query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);
                }
            }

            var list = await query.ToListAsync();
            r.Extension = _mapper.Map<List<CostInfoResponse>>(list);

            return r;

        }

    }
        
    
}