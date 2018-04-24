using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using XYHContractPlugin.Stores;
using XYHContractPlugin.Dto.Response;
using XYHContractPlugin.Dto.Request;
using ApplicationCore.Dto;
using System.Threading.Tasks;
using System.Threading;
using XYHContractPlugin.Models;
using AutoMapper;

using ApplicationCore.Managers;
using Microsoft.EntityFrameworkCore;

namespace XYHContractPlugin.Managers
{
    public class ContractListManager
    {
        public ContractListManager(
            IContractInfoStore icontractstore,
            IOrganizationExpansionStore organizationExpansionStore,
            PermissionExpansionManager permissionExpansionManager,
            IMapper mapper)
        {
            _icontractInfoStore = icontractstore;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _iorganizationExpansionStore = organizationExpansionStore ?? throw new ArgumentNullException(nameof(organizationExpansionStore));
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
        }

        protected IContractInfoStore _icontractInfoStore { get; }
        protected IMapper _mapper { get; }
        protected IOrganizationExpansionStore _iorganizationExpansionStore { get; }

        protected PermissionExpansionManager _permissionExpansionManager { get; }
        public virtual async Task<ContractSearchResponse<ContractInfoResponse>> SearchContract2(UserInfo user, ContractSearchRequest condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            var pagingResponse = new ContractSearchResponse<ContractInfoResponse>();
            var query = _icontractInfoStore.ContractInfoAll().Where(a => !a.IsDelete);
            query = SearchConditionFiltration(condition, query);
            var organsPer = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "SEARCH_CONTRACT");
            //查询部门
            if (!string.IsNullOrEmpty(condition.Organizate))
            {
              
                if(organsPer.Contains(condition.Organizate))//包含在此权限的范围内
                {
                    var organs = await _permissionExpansionManager.GetLowerDepartments(condition.Organizate);
                    query = query.Where(x => organs.Contains(x.OrganizateID));
                }
                else
                {
                    query = query.Where(x => organsPer.Contains(x.OrganizateID));
                }
  
            }
            else
            {
                query = query.Where(x => organsPer.Contains(x.OrganizateID));
            }


            if (condition.CheckStatu > 0)
            {
                int nCheckState = 0;
                if ((condition.CheckStatu & 0x01) > 0)//1 2 4 8 未提交 审核中 通过 驳回
                {
                    nCheckState = (condition.CheckStatu & 0x01);
                }
                if ((condition.CheckStatu & 0x02) > 0)
                {
                    nCheckState = (condition.CheckStatu & 0x02);
                }
                if ((condition.CheckStatu & 0x04) > 0)
                {
                    nCheckState = (condition.CheckStatu & 0x04);
                }
                if ((condition.CheckStatu & 0x08) > 0)
                {
                    nCheckState = (condition.CheckStatu & 0x08);
                }
                query = query.Where(x => x.ExamineStatus == nCheckState);
            }
            
            if (condition.pageIndex == -1)
            {
                var qlist = await query.ToListAsync(cancellationToken);
                foreach (var item in qlist)
                {
                    item.Organizate = _iorganizationExpansionStore.GetFullName(item.OrganizateID).Replace("默认顶级-", "");
                    item.CreateDepartment = _iorganizationExpansionStore.GetFullName(item.OrganizateID).Replace("默认顶级-", "");
                    List<string> fullId = await _permissionExpansionManager.GetParentDepartments(item.OrganizateID);
                    fullId.Remove("0");
                    item.OrganizateFullId = string.Join("*", fullId.ToArray());
                }
                pagingResponse.TotalCount = qlist.Count;
                pagingResponse.ValidityContractCount = qlist.Count;
                pagingResponse.PageIndex = condition.pageIndex;
                pagingResponse.PageSize = condition.pageSize;
                pagingResponse.Extension = _mapper.Map<List<ContractInfoResponse>>(qlist);
            }
            else
            {
                pagingResponse.TotalCount = await query.CountAsync(cancellationToken);
                pagingResponse.ValidityContractCount = pagingResponse.TotalCount;
                //需要加上排序
                var qlist = await query.Skip(condition.pageIndex * condition.pageSize).Take(condition.pageSize).ToListAsync(cancellationToken);
                foreach(var item in qlist)
                {
                    item.Organizate = _iorganizationExpansionStore.GetFullName(item.OrganizateID).Replace("默认顶级-", "");
                    item.CreateDepartment = _iorganizationExpansionStore.GetFullName(item.OrganizateID).Replace("默认顶级-", "");
                    List<string> fullId = await _permissionExpansionManager.GetParentDepartments(item.OrganizateID);
                    fullId.Remove("0");
                    item.OrganizateFullId = string.Join("*", fullId.ToArray());
                }
                //                 qlist.Select(async x =>
                //                 {
                //                     x.Organizate = _iorganizationExpansionStore.GetFullName(x.OrganizateID);
                //                     x.CreateDepartment = _iorganizationExpansionStore.GetFullName(x.OrganizateID).Replace("默认顶级-", "");
                //                     List<string> fullId = await _permissionExpansionManager.GetParentDepartments(x.OrganizateID);
                //                     fullId.Remove("0");
                //                     x.OrganizateFullId = string.Join("*", fullId.ToArray());
                //                     return x;
                //                 });
                pagingResponse.PageIndex = condition.pageIndex;
                pagingResponse.PageSize = condition.pageSize;
                pagingResponse.Extension = _mapper.Map<List<ContractInfoResponse>>(qlist);
            }


            return pagingResponse;
        }

        public IQueryable<ContractInfo> SearchConditionFiltration(ContractSearchRequest condition, IQueryable<ContractInfo> query)
        {
            //查询主键信息
            if (!string.IsNullOrEmpty(condition.KeyWord))
            {
               query = query.Where(x => x.Name.Contains(condition.KeyWord) || x.Num.Contains(condition.KeyWord) );
            }

            if (condition.CheckStatu > 0)
            {
                //query = from 
            }

            if (condition.Discard == 1)
            {
                query = query.Where(x => !x.IsDelete);//这个后面考虑
            }

            if (condition.OverTime == 1)//过期和时间限定分开
            {
                query = query.Where(x => x.EndTime < DateTime.Now);
            }
            if (condition?.CreateDateStart != null)
            {
     
                query = query.Where(x => (x.CreateTime >= condition.CreateDateStart.Value ));
            }
            if(condition?.CreateDateEnd != null)
            {
                query = query.Where(x => ( x.EndTime >= x.CreateTime.Value));
            }
            if (condition.Follow == 1)
            {
                query = query.Where(x => x.IsFollow.Value);
            }

            if (condition?.OrderRule == 0 || condition?.OrderRule == null)
            {
                query = query.OrderByDescending(x => x.StartTime);
            }
            else
            {
                query = query.OrderBy(x => x.Num);
            }
            return query;
        }
        public virtual async Task<ContractSearchResponse<ContractInfoResponse>> SearchContract(UserInfo user, ContractSearchRequest condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            var Response = new ContractSearchResponse<ContractInfoResponse>();
            var sql = @"SELECT a.* from XYH_DT_CONTRACTINFO as a where";

            if (condition?.CheckStatu > 0)
            {
                sql = @"SELECT a.* from XYH_DT_CONTRACTINFO as a LEFT JOIN XYH_DT_MODIFY as b ON a.`CurrentModify`=b.`ID` where";
            }

            string connectstr=" ";
            if (!string.IsNullOrEmpty(condition?.KeyWord))
            {
                sql += connectstr + @"LOCATE('" + condition.KeyWord + "', a.`Name`)>0";
                connectstr = " and ";
            }
            else if (condition?.KeyWord != null)
            {
                sql += connectstr + @"a.`ID`!=''";
                connectstr = " and ";
            }

            if (condition?.Discard == 1)
            {
                sql += connectstr + @"a.IsDelete";
                connectstr = " and ";
            }

            if (condition?.OverTime == 1)//过期和时间限定分开
            {
                sql += connectstr + @"a.`EndTime`<=CURTIME()";
                connectstr = " and ";
            }
            else if (condition?.CreateDateStart != null && condition?.CreateDateEnd != null)
            {
                sql += connectstr + @"(a.`StartTime`<='" + condition.CreateDateStart.Value+"'";
                connectstr = " and ";
                sql += connectstr + @"a.`EndTime`>='" + condition.CreateDateEnd.Value+"')";
            }

            if (condition?.Follow == 1)
            {
                sql += connectstr + @"a.`Follow`!=''";
                connectstr = " and ";
            }

            if (condition?.CheckStatu > 0)
            {
                string head = "(", tail=")";
                if ((condition?.CheckStatu & 0x01) > 0)//1 2 4 8 未提交 审核中 通过 驳回
                {
                    sql += connectstr + head + @"b.`ExamineStatus`=0";
                    connectstr = " or ";
                    head = "";
                }
                if ((condition?.CheckStatu & 0x02) > 0)
                {
                    sql += connectstr + head + @"b.`ExamineStatus`=1";
                    connectstr = " or ";
                    head = "";
                }
                if ((condition?.CheckStatu & 0x04) > 0)
                {
                    sql += connectstr + head + @"b.`ExamineStatus`=8";
                    connectstr = " or ";
                    head = "";
                }
                if ((condition?.CheckStatu & 0x08) > 0)
                {
                    sql += connectstr + head + @"b.`ExamineStatus`=16";
                    connectstr = " or ";
                    head = "";
                }

                sql += tail;
                connectstr = " and ";
            }

            if (!string.IsNullOrEmpty(condition?.Organizate))
            {
                sql += connectstr + @"a.`OrganizateID`='" + condition.Organizate + "'";
                connectstr = " and ";
            }


            if (condition?.OrderRule == 0 || condition?.OrderRule == null)
            {
                sql += @" ORDER BY a.`StartTime`";
            }
            else if (condition?.OrderRule == 1)
            {
                sql += @" ORDER BY a.`ID`";
            }

            var query = _icontractInfoStore.DapperSelect<ContractInfo>(sql).ToList();
            Response.ValidityContractCount = query.Count;
            Response.TotalCount = query.Count;

            List<ContractInfo> result = new List<ContractInfo>();
            var begin = (condition.pageIndex) * condition.pageSize;
            var end = (begin + condition.pageSize)>query.Count ? query.Count: (begin+condition.pageSize);

            for (; begin < end; begin++)
            {
                result.Add(query.ElementAt(begin));
            }
            
            Response.PageIndex = condition.pageIndex;
            Response.PageSize = condition.pageSize;
            Response.Extension = _mapper.Map<List<ContractInfoResponse>>(result);

            foreach (var item in Response.Extension)
            {
                item.ExamineStatus = 0;
                if (item.CurrentModify != null)
                {
                    var moinf = await _icontractInfoStore.GetModifyAsync(a => a.Where(b => b.ID == item.CurrentModify));
                    if (moinf != null)
                    {
                        item.ExamineStatus = moinf.ExamineStatus;
                    }
                }
            }

            return Response;


            //sql += @" limit " + condition.pageIndex * condition.pageSize + "," + condition.pageSize + "";

            //var query = _icontractInfoStore.DapperSelect<ContractInfo>(sql).ToList();

            //Response.ValidityContractCount = query.Count;
            //Response.PageIndex = condition.pageIndex;
            //Response.PageSize = condition.pageSize;
            //Response.Extension = _mapper.Map<List<ContractInfoResponse>>(query);

            //foreach (var item in Response.Extension)
            //{
            //    item.ExamineStatus = 0;
            //    if (item.CurrentModify != null)
            //    {
            //        var moinf = await _icontractInfoStore.GetModifyAsync(a => a.Where(b => b.ID == item.CurrentModify));
            //        if (moinf != null)
            //        {
            //            item.ExamineStatus = moinf.ExamineStatus;
            //        }

            //    }
            //}

            //return Response;
        }
    }
}
