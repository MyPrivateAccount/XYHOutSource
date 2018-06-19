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

        public ChargeManager(IChargeInfoStore stor, 
            IOrganizationUtils orgUtils, 
            ITransaction<ChargeDbContext> transaction,
            PermissionExpansionManager permissionExpansion,
            OrganizationExpansionManager organizationExpansion,
            IMapper mapper)
        {
            _Store = stor;
            _transaction = transaction;
            _permissionExpansion = permissionExpansion;
            _organizationExpansion = organizationExpansion;
            _orgUtils = orgUtils;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

       

        protected IChargeInfoStore _Store { get; }
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
                        int seq = await _Store.GetChargeNo(request.BranchId, DateTime.Now, request.Type);
                        request.Seq = seq;
                        request.ChargeNo = String.Format("FY{0}{1}{2:D5}", prefix, DateTime.Now.ToString("yyyyMMdd"), seq);
                    }
                    var ci = _mapper.Map<ChargeInfo>(request);
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


        public async Task<PagingResponseMessage<ChargeInfoResponse>> Search(UserInfo user, ChargeSearchRequest request)
        {
            PagingResponseMessage<ChargeInfoResponse> r = new PagingResponseMessage<ChargeInfoResponse>();

            var orgList =await _permissionExpansion.GetOrganizationOfPermission(user.Id, "FY_BXMD");

            var query = _Store.SimpleQuery;
            if (request != null)
            {
                if (!String.IsNullOrEmpty(request.ReimburseDepartment))
                {
                    var orgChildren = await _permissionExpansion.GetLowerDepartments(request.ReimburseDepartment);
                    orgList = orgList.Where(x => orgChildren.Contains(x)).ToList();
                }
            }
            query = query.Where(c => orgList.Contains(c.ReimburseDepartment));

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
                    query = query.Where(c => (c.ReimburseUserInfo.Name.Contains(request.Keyword) || c.ReimburseUserInfo.UserID.Contains(request.Keyword) || c.Memo.Contains(request.Keyword) || c.ChargeNo.Contains(request.Keyword) ));
                }

                if(request.PageIndex>0 && request.PageSize > 0)
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
    }
}


//using ApplicationCore.Dto;
//using ApplicationCore.Models;
//using AutoMapper;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using XYHChargePlugin.Dto.Request;
//using XYHChargePlugin.Dto.Response;
//using XYHChargePlugin.Models;
//using XYHChargePlugin.Stores;

//namespace XYHChargePlugin.Managers
//{
//    public class ChargeManager
//    {
//        public ChargeManager(IChargeManageStore stor, IMapper mapper)
//        {
//            _Store = stor;
//            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
//        }

//        public static int UnCheckCharge = 0;
//        public static int CheckedCharge = 1;

//        protected IChargeManageStore _Store { get; }
//        protected IMapper _mapper { get; }

//        public virtual async Task AddCharge(UserInfo User, ContentRequest req, string modifyid, string check, CancellationToken cancellationToken = default(CancellationToken))
//        {
//            await _Store.CreateChargeAsync(_mapper.Map<SimpleUser>(User), 
//                _mapper.Map<ChargeInfo>(req.ChargeInfo),
//                modifyid, check, cancellationToken);

//            await _Store.CreateCostListAsync(_mapper.Map<List<CostInfo>>(req.CostInfos), cancellationToken);
//            await _Store.CreateReceiptListAsync(_mapper.Map<SimpleUser>(User), _mapper.Map<List<ReceiptInfo>>(req.ReceiptInfos), cancellationToken);
//        }

//        public virtual async Task CreateFilelistAsync(string userid, List<FileInfoCallbackRequest> fileInfoCallbackRequestList, CancellationToken cancellationToken = default(CancellationToken))
//        {
//            if (fileInfoCallbackRequestList == null)
//            {
//                throw new ArgumentNullException(nameof(fileInfoCallbackRequestList));
//            }

//            var fileInfos = _mapper.Map<List<FileInfo>>(fileInfoCallbackRequestList);
//            for (int i = 0; i < fileInfos.Count; i++)
//            {
//                fileInfos[i].IsDeleted = false;
//                fileInfos[i].CreateTime = DateTime.Now;
//                fileInfos[i].CreateUser = userid;
//                fileInfos[i].Uri = fileInfoCallbackRequestList[i].FilePath;
//            }
//            await _Store.CreateFileListAsync(fileInfos, cancellationToken);
//        }

//        public virtual async Task CreateFileScopeAsync(string userid, string receiptid, FileInfoRequest filescop, CancellationToken cancellationToken = default(CancellationToken))
//        {
//            if (filescop == null)
//            {
//                throw new ArgumentNullException(nameof(filescop));
//            }

//           await _Store.CreateFileScopeAsync(receiptid, _mapper.Map<FileScopeInfo>(filescop), cancellationToken);
//        }

//        public virtual async Task SubmitAsync(string modifyid, ExamineStatusEnum ext, CancellationToken cancellationToken = default(CancellationToken))
//        {
//            if (modifyid == null)
//            {
//                throw new ArgumentNullException(nameof(modifyid));
//            }

//            if (ext == ExamineStatusEnum.Approved)
//            {
//                await _Store.UpdateExamineStatus(modifyid, ext, CheckedCharge, cancellationToken);
//            }
//            else if (ext == ExamineStatusEnum.Reject)
//            {
//                await _Store.UpdateExamineStatus(modifyid, ext, UnCheckCharge, cancellationToken);
//            }

//        }

//        public virtual async Task<List<ReceiptInfoResponse>> GetRecieptbyID(UserInfo user, string chargeid)
//        {
//            if (user == null)
//            {
//                throw new ArgumentNullException(nameof(user));
//            }

//            if (!string.IsNullOrEmpty(chargeid))
//            {
//                var tt = await _Store.GetRecieptListAsync(a=> a.Where(b => b.ChargeID == chargeid));
//                var retun =_mapper.Map<List<ReceiptInfoResponse>>(tt);

//                foreach (var item in retun)
//                {
//                    var scope = await _Store.GetScopeInfo(a => a.Where(b => b.ReceiptID == item.ID));
//                    foreach (var itm in scope)
//                    {
//                        var file = await _Store.GetFileInfo(a => a.Where(b => b.FileGuid == itm.FileGuid && b.Type == "ORIGINAL"));
//                        if (file != null)
//                        {
//                            if (item.FileList == null) {item.FileList = new List<SimpleList>();}
//                            item.FileList.Add(new SimpleList { uid = item.FileList.Count, name="", status="done", url=ConvertToSimpleFile(file.Uri)});
//                        }

//                    }
//                }

//                return retun;
//            }
//            return null;
//        }

//        public virtual async Task<ChargeDetailInfoResponse> GetChargeDetailInfo(string chargeid)
//        {
//            var info = new ChargeDetailInfoResponse();
//            if (!string.IsNullOrEmpty(chargeid))
//            {
//                info.ChargeInfo = _mapper.Map<ChargeInfoResponse>(await _Store.GetChargeAsync(a => a.Where(b => b.ID == chargeid)));
//                info.CostInfos = _mapper.Map<List<CostInfoResponseEx>>(await _Store.GetCostListAsync(a => a.Where(b => b.ChargeID == chargeid)));
//                foreach (var item in info.CostInfos)
//                {
//                    item.ReceiptList = _mapper.Map<List<ReceiptInfoResponse>>(await _Store.GetRecieptListAsync(a => a.Where(b => b.CostID == item.ID)));
//                    foreach (var it in item.ReceiptList)
//                    {
//                        var scope = await _Store.GetScopeInfo(a => a.Where(b => b.ReceiptID == it.ID));
//                        foreach (var itm in scope)
//                        {
//                            var file = await _Store.GetFileInfo(a => a.Where(b => b.FileGuid == itm.FileGuid && b.Type == "ORIGINAL"));
//                            if (file != null)
//                            {
//                                if (it.FileList == null) { it.FileList = new List<SimpleList>(); }
//                                it.FileList.Add(new SimpleList { uid = it.FileList.Count, name = "", status = "done", url = ConvertToSimpleFile(file.Uri) });
//                            }

//                        }
//                    }
//                }
//            }

//            return info;
//        }

//        private string ConvertToSimpleFile(string file)
//        {
//            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
//            fr = (fr ?? "").TrimEnd('/');
//            var tf = fr + "/" + file.TrimStart('/');
//            return tf;
//        }

//        private FileItemResponse ConvertToFileItem(string fileGuid, List<FileInfo> fl)
//        {
//            FileItemResponse fi = new FileItemResponse();
//            fi.FileGuid = fileGuid;
//            fi.Group = fl.FirstOrDefault()?.Group;
//            fi.Icon = fl.FirstOrDefault(x => x.Type == "ICON")?.Uri;
//            fi.Original = fl.FirstOrDefault(x => x.Type == "ORIGINAL")?.Uri;
//            fi.Medium = fl.FirstOrDefault(x => x.Type == "MEDIUM")?.Uri;
//            fi.Small = fl.FirstOrDefault(x => x.Type == "SMALL")?.Uri;

//            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
//            fr = (fr ?? "").TrimEnd('/');
//            if (!String.IsNullOrEmpty(fi.Icon))
//            {
//                fi.Icon = fr + "/" + fi.Icon.TrimStart('/');
//            }
//            if (!String.IsNullOrEmpty(fi.Original))
//            {
//                fi.Original = fr + "/" + fi.Original.TrimStart('/');
//            }
//            if (!String.IsNullOrEmpty(fi.Medium))
//            {
//                fi.Medium = fr + "/" + fi.Medium.TrimStart('/');
//            }
//            if (!String.IsNullOrEmpty(fi.Small))
//            {
//                fi.Small = fr + "/" + fi.Small.TrimStart('/');
//            }
//            return fi;
//        }

//        public virtual async Task UpdateChargePostTime(string chargeid, string department, CancellationToken cancellationToken = default(CancellationToken))
//        {
//            await _Store.UpdatePostTime(chargeid, department, cancellationToken);
//        }

//        public virtual async Task UpdateRecieptList(List<ReceiptInfoRequest> lstreciept, CancellationToken cancellationToken = default(CancellationToken))
//        {
//            foreach (var item in lstreciept)//给发票找个cost这样才能指定
//            {
//                if (string.IsNullOrEmpty(item.CostID))
//                {
//                    var tf = await _Store.GetCostListAsync(a => a.Where(b => b.ChargeID == item.ChargeID && b.Type == item.Type), cancellationToken);
//                    if (tf != null && tf.Count >0)
//                    {
//                        item.CostID = tf[0].ID;
//                    }
//                }
//            }
//            await _Store.UpdateRecieptList(_mapper.Map<List<ReceiptInfo>>(lstreciept), cancellationToken);
//        }

//        public virtual async Task UpdateLimit(string userid, int cost, CancellationToken cancellationToken = default(CancellationToken))
//        {

//            if (string.IsNullOrEmpty(userid))
//            {
//                throw new ArgumentNullException(nameof(userid));
//            }

//            await _Store.SetLimit(userid, cost, cancellationToken);
//        }

//        public virtual async Task<LimitInfoResponse> GetLimitInfo(string userid, CancellationToken cancellationToken = default(CancellationToken))
//        {
//            if (string.IsNullOrEmpty(userid))
//            {
//                throw new ArgumentNullException(nameof(userid));
//            }

//            return _mapper.Map< LimitInfoResponse >( await _Store.GetLimitAsync(a => a.Where(b => b.UserID == userid)));
//        }

//        public virtual async Task<ChargeSearchResponse<ChargeInfoResponse>> SearchChargeInfo(UserInfo user, ChargeSearchRequest condition, bool Isself, CancellationToken cancellationToken = default(CancellationToken))
//        {
//            if (condition == null)
//            {
//                throw new ArgumentNullException(nameof(condition));
//            }

//            var Response = new ChargeSearchResponse<ChargeInfoResponse>();

//            var sql = @"SELECT a.* from XYH_CH_CHARGEMANAGE as a where";

//            string connectstr = " ";

//            if (Isself)//只能检索自己的费用
//            {
//                sql = @"SELECT a.* from XYH_CH_CHARGEMANAGE as a where a.`CreateUser`='"+user.Id+"'";

//                if (condition?.CheckStatu > 0)
//                {
//                    sql = @"SELECT a.* from XYH_CH_CHARGEMANAGE as a LEFT JOIN XYH_CH_MODIFY as b ON a.`CurrentModify`=b.`ID` where a.`CreateUser`='"+user.Id+ "'";
//                }

//                connectstr = " and ";

//                if (!string.IsNullOrEmpty(condition?.KeyWord))
//                {
//                    sql += connectstr + @"LOCATE('" + condition.KeyWord + "', a.`ID`)>0";
//                    connectstr = " and ";
//                }
//                else if (condition?.KeyWord != null)
//                {
//                    sql += connectstr + @"a.`ID`!=''";
//                    connectstr = " and ";
//                }
//            }
//            else
//            {
//                sql = @"SELECT a.* from XYH_CH_CHARGEMANAGE as a where a.`ID`!=''";

//                if (condition?.CheckStatu > 0)
//                {
//                    sql = @"SELECT a.* from XYH_CH_CHARGEMANAGE as a LEFT JOIN XYH_CH_MODIFY as b ON a.`CurrentModify`=b.`ID` where a.`ID`!=''";
//                }

//                connectstr = " and ";
//            }

//            if (condition?.SearchTimeType > 0)
//            {
//                switch (condition?.SearchTimeType)
//                {
//                    case 1:
//                        {
//                            sql += connectstr + @"(a.`CreateTime`<='" + condition.CreateDateStart.Value + "'";
//                            connectstr = " and ";
//                            sql += connectstr + @"a.`CreateTime`>='" + condition.CreateDateEnd.Value + "')";
//                        }
//                        break;
//                    case 2:
//                        {
//                            sql += connectstr + @"(a.`PostTime`<='" + condition.CreateDateStart.Value + "'";
//                            connectstr = " and ";
//                            sql += connectstr + @"a.`PostTime`>='" + condition.CreateDateEnd.Value + "')";
//                        }
//                        break;
//                    default:
//                        break;
//                }

//            }

//            if (condition?.ChargePrice > 0)
//            {
//                sql += connectstr + @"a.`TotalCost`>"+condition.ChargePrice;
//                connectstr = " and ";
//            }

//            if (condition?.CheckStatu > 0)
//            {
//                string head = "(", tail = ")";
//                if (condition?.CheckStatu == 1)//1 2 4 8 未提交 审核中 通过 驳回
//                {
//                    sql += connectstr + head + @"b.`ExamineStatus`=0 or b.`ExamineStatus`=1";
//                    connectstr = " or ";
//                    head = "";
//                }
//                if (condition?.CheckStatu == 2)
//                {
//                    sql += connectstr + head + @"b.`ExamineStatus`=8";
//                    connectstr = " or ";
//                    head = "";
//                }
//                if (condition?.CheckStatu == 3)
//                {
//                    sql += connectstr + head + @"b.`ExamineStatus`=16";
//                    connectstr = " or ";
//                    head = "";
//                }

//                sql += tail;
//                connectstr = " and ";
//            }


//            if (condition?.OrderRule == 0 || condition?.OrderRule == null)
//            {
//                sql += @" ORDER BY a.`CreateTime`";
//            }
//            else if (condition?.OrderRule == 1)
//            {
//                sql += @" ORDER BY a.`ID`";
//            }

//            var query = _Store.DapperSelect<ChargeInfo>(sql).ToList();
//            Response.ValidityContractCount = query.Count;
//            Response.TotalCount = query.Count;

//            List<ChargeInfo> result = new List<ChargeInfo>();
//            var begin = (condition.pageIndex) * condition.pageSize;
//            var end = (begin + condition.pageSize) > query.Count ? query.Count : (begin + condition.pageSize);

//            for (; begin < end; begin++)
//            {
//                result.Add(query.ElementAt(begin));
//            }

//            Response.PageIndex = condition.pageIndex;
//            Response.PageSize = condition.pageSize;
//            Response.Extension = _mapper.Map<List<ChargeInfoResponse>>(result);

//            foreach (var item in Response.Extension)
//            {
//                //sql = @"SELECT FORMAT(SUM(Cost),2) FROM xyh_ch_cost WHERE ChargeID='" + item.ID+"'";
//                if (!string.IsNullOrEmpty(item.CurrentModify))
//                {
//                    var t = await _Store.GetModifyAsync(a => a.Where(b => b.ID == item.CurrentModify));
//                    item.CheckStatus = t.ExamineStatus.GetValueOrDefault();
//                }
//            }

//            return Response;

//        }
//    }
//}
