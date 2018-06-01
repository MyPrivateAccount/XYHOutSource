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

        
    }
    
}
