using ApplicationCore.Dto;
using ApplicationCore.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Models;
using XYHShopsPlugin.Stores;

namespace XYHShopsPlugin.Managers
{
    public class BuildingFacilitiesManager
    {
        public BuildingFacilitiesManager(IBuildingFacilitiesStore buildingFacilitiesStore, IMapper mapper)
        {
            Store = buildingFacilitiesStore ?? throw new ArgumentNullException(nameof(buildingFacilitiesStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IBuildingFacilitiesStore Store { get; }
        protected IMapper _mapper { get; }

        public virtual async Task<BuildingFacilitiesInfoResponse> CreateAsync(BuildingFacilitiesRequest buildingFacilitiesRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingFacilitiesRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingFacilitiesRequest));
            }
            var facilities = await Store.CreateAsync(_mapper.Map<BuildingFacilities>(buildingFacilitiesRequest), cancellationToken);
            return _mapper.Map<BuildingFacilitiesInfoResponse>(facilities);
        }

        public virtual async Task<BuildingFacilitiesInfoResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var facilities = await Store.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<BuildingFacilitiesInfoResponse>(facilities);
        }

        public virtual async Task UpdateAsync(BuildingFacilitiesRequest buildingFacilitiesRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingFacilitiesRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingFacilitiesRequest));
            }
            await Store.UpdateAsync(_mapper.Map<BuildingFacilities>(buildingFacilitiesRequest), cancellationToken);
        }

        public virtual async Task SaveAsync(UserInfo user, BuildingFacilitiesRequest buildingFacilitiesRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (buildingFacilitiesRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingFacilitiesRequest));
            }
            await Store.SaveAsync(_mapper.Map<SimpleUser>(user), _mapper.Map<BuildingFacilities>(buildingFacilitiesRequest), cancellationToken);
        }
    }
}
