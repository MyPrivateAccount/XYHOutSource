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

namespace XYHContractPlugin.Managers
{
    public class ContractListManager
    {
        public ContractListManager(IContractInfoStore icontractstore, IMapper mapper)
        {
            _icontractInfoStore = icontractstore;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            
        }

        protected IContractInfoStore _icontractInfoStore { get; }
        protected IMapper _mapper { get; }
        

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
                sql += connectstr + @"a.`Organizete`='" + condition.Organizate + "'";
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
