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

            if ((condition?.SearchType & 0x10) > 0)
            {
                sql = @"SELECT a.* from XYH_DT_CONTRACTINFO as a LEFT JOIN XYH_DT_MODIFY as b ON a.`CurrentModify`=b.`ID` where";
            }

            string connectstr=" ";
            if ((condition?.SearchType & 0x01) >0)
            {
                if (!string.IsNullOrEmpty(condition.KeyWord))
                {
                    sql += connectstr + @"LOCATE('" + condition.KeyWord + "', a.`Name`)>0";
                    connectstr = " and ";
                }
            }

            if ((condition?.SearchType & 0x02) > 0)
            {
                sql += connectstr + @"a.IsDelete";
                connectstr = " and ";
            }

            if ((condition?.SearchType & 0x04) > 0)//过期和时间限定分开
            {
                sql += connectstr + @"a.`EndTime`<=CURTIME()";
                connectstr = " and ";
            }
            else if ((condition?.SearchType & 0x20) > 0)
            {
                sql += connectstr + @"(a.`StartTime`<='" + condition.CreateDateStart.Value+"'";
                connectstr = " and ";
                sql += connectstr + @"a.`EndTime`>='" + condition.CreateDateEnd.Value+"')";
            }

            if ((condition?.SearchType & 0x08) > 0)
            {
                sql += connectstr + @"a.`Follow`!=''";
                connectstr = " and ";
            }

            if ((condition?.SearchType & 0x10) > 0)
            {
                sql += connectstr + @"b.`ExamineStatus`="+condition.CheckStatu;
                connectstr = " and ";
            }

            if ((condition?.SearchType & 0x40) > 0)
            {
                sql += connectstr + @"a.`CreateDepartment`='" + condition.Organizate + "'";
                connectstr = " and ";
            }


            if (condition?.OrderRule == 0)
            {
                sql += @" ORDER BY a.`StartTime`";
            }
            else if (condition?.OrderRule == 1)
            {
                sql += @" ORDER BY a.`ID`";
            }

            sql += @" limit " + condition.pageIndex * condition.pageSize + "," + condition.pageSize + "";

            var query = _icontractInfoStore.DapperSelect<ContractInfo>(sql).ToList();

            Response.ValidityContractCount = query.Count;
            Response.PageIndex = condition.pageIndex;
            Response.PageSize = condition.pageSize;
            Response.Extension = _mapper.Map<List<ContractInfoResponse>>(query);
            return Response;
        }
    }
}
