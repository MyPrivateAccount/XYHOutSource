using ApplicationCore.Dto;
using ApplicationCore.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHHumanPlugin.Dto.Response;
using XYHHumanPlugin.Models;
using XYHHumanPlugin.Stores;
using PositionInfoRequest = XYHHumanPlugin.Dto.Response.PositionInfoResponse;

namespace XYHHumanPlugin.Managers
{
    public class StationManager
    {
        public StationManager(IHumanManageStore stor, IMapper mapper)
        {
            _Store = stor;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IHumanManageStore _Store { get; }
        protected IMapper _mapper { get; }

        public virtual async Task<List<PositionInfoResponse>> GetStationListByDepartment(string departmentid, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _mapper.Map<List<PositionInfoResponse>>(await _Store.GetStationListAsync(a => a.Where(b => b.ParentID == departmentid)));
        }

        public virtual async Task SetStation(PositionInfoRequest departmentid, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (departmentid == null)
            {
                throw new ArgumentNullException(nameof(departmentid));
            }
            if (string.IsNullOrEmpty(departmentid.ID))
            {
                departmentid.ID = Guid.NewGuid().ToString();
            }

            await _Store.SetStationAsync(_mapper.Map<PositionInfo>(departmentid), cancellationToken);
        }

        public virtual async Task DeleteStation(PositionInfoRequest departement, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(departement.ID))
            {
                throw new ArgumentNullException(nameof(departement));
            }
            await _Store.DeleteStationAsync(_mapper.Map<PositionInfo>(departement), cancellationToken);
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
