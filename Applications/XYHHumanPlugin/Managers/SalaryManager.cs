using ApplicationCore.Dto;
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
using SalaryInfoRequest = XYHHumanPlugin.Dto.Response.SalaryInfoResponse;

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

        public virtual async Task SetSalary(SalaryInfoRequest salary, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (salary == null)
            {
                throw new ArgumentNullException(nameof(salary));
            }
            if (string.IsNullOrEmpty(salary.ID))
            {
                salary.ID = Guid.NewGuid().ToString();
            }

            await _Store.SetSalaryAsync(_mapper.Map<SalaryInfo>(salary), cancellationToken);
        }

        public virtual async Task<SalaryInfoResponse> GetSalaryItem(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return _mapper.Map<SalaryInfoResponse>(await _Store.GetSalaryAsync(a => a.Where(b => b.Position == id)));
        }

        public virtual async Task DeleteSalary(SalaryInfoRequest salary, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(salary.ID))
            {
                throw new ArgumentNullException(nameof(salary));
            }
            await _Store.DeleteSalaryAsync(_mapper.Map<SalaryInfo>(salary), cancellationToken);
        }
        
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
            else if (condition?.KeyWord == null)
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
