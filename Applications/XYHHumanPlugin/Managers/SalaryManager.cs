using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Managers
{
    public class SalaryManager
    {
        public SalaryManager(IHumanManageStore stor, IMapper mapper)
        {
            _Store = stor;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IHumanManageStore _Store { get; }
        protected IMapper _mapper { get; }
        
        public virtual async Task<HumanSearchResponse<SalaryInfoResponse>> SearchSalaryInfo(UserInfo user, HumanSearchRequest condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            var Response = new HumanSearchResponse<SalaryInfoResponse>();
            var sql = @"SELECT a.* from XYH_HU_SALARY as a where";

            string connectstr = " ";
            if (!string.IsNullOrEmpty(condition?.KeyWord))
            {
                sql += connectstr + @"LOCATE('" + condition.KeyWord + "', a.`Position`)>0";
                connectstr = " and ";
            }
            else if (condition?.KeyWord != null)
            {
                sql += connectstr + @"a.`ID`!=''";
                connectstr = " and ";
            }

            if (condition?.OrderRule == 0 || condition?.OrderRule == null)
            {
                sql += @" ORDER BY a.`Position`";
            }

            try
            {
                var query = _Store.DapperSelect<SalaryInfo>(sql).ToList();
                Response.ValidityContractCount = query.Count;
                Response.TotalCount = query.Count;

                List<SalaryInfo> result = new List<SalaryInfo>();
                var begin = (condition.pageIndex) * condition.pageSize;
                var end = (begin + condition.pageSize) > query.Count ? query.Count : (begin + condition.pageSize);

                for (; begin < end; begin++)
                {
                    result.Add(query.ElementAt(begin));
                }

                Response.PageIndex = condition.pageIndex;
                Response.PageSize = condition.pageSize;
                Response.Extension = _mapper.Map<List<SalaryInfoResponse>>(result);
            }
            catch (Exception e)
            {

                throw;
            }
            
            return Response;
        }
    }
}
