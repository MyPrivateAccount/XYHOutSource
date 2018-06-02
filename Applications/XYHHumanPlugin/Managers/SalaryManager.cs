using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        //public virtual async Task<List<PositionInfoResponse>> GetStationListByDepartment(string departmentid, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    return _mapper.Map<List<PositionInfoResponse>>(await _Store.GetStationListAsync(a => a.Where(b => b.ParentID == departmentid)));
        //}

        public virtual async Task SetStation(SalaryInfoRequest salary, CancellationToken cancellationToken = default(CancellationToken))
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

        public virtual async Task DeleteStation(SalaryInfoRequest salary, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(salary.ID))
            {
                throw new ArgumentNullException(nameof(salary));
            }
            await _Store.DeleteSalaryAsync(_mapper.Map<SalaryInfo>(salary), cancellationToken);
            //await _Store.DeleteStationAsync(_mapper.Map<SalaryInfo>(departement), cancellationToken);
        }
    }
}
