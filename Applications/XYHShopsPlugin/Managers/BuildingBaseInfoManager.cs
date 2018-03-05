using ApplicationCore.Dto;
using ApplicationCore.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Models;
using XYHShopsPlugin.Stores;

namespace XYHShopsPlugin.Managers
{
    public class BuildingBaseInfoManager
    {

        public BuildingBaseInfoManager(IBuildingBaseInfoStore buildingBaseInfoStore, IMapper mapper)
        {
            Store = buildingBaseInfoStore ?? throw new ArgumentNullException(nameof(buildingBaseInfoStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IBuildingBaseInfoStore Store { get; }
        protected IMapper _mapper { get; }

        public virtual async Task<BuildingBaseInfoResponse> CreateAsync(BuildingBaseInfoRequest buildingBaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }
            var baseinfo = await Store.CreateAsync(_mapper.Map<BuildingBaseInfo>(buildingBaseInfoRequest), cancellationToken);
            return _mapper.Map<BuildingBaseInfoResponse>(baseinfo);
        }

        public virtual async Task<BuildingBaseInfoResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var baseinfo = await Store.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<BuildingBaseInfoResponse>(baseinfo);
        }

        public virtual async Task<BuildingBaseInfoResponse> FindByNameAsync(string buildingId, string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            var baseinfo = await Store.GetAsync(a => a.Where(b => b.Id != buildingId && b.Name == name), cancellationToken);
            return _mapper.Map<BuildingBaseInfoResponse>(baseinfo);
        }

        public virtual async Task<bool> CheckDuplicateBuilding(string buildingId, string name, string city, string district, string area, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Store.CheckDuplicateBuilding(buildingId, name, city, district, area, cancellationToken);
        }


        public virtual async Task UpdateAsync(BuildingBaseInfoRequest buildingBaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }
            await Store.UpdateAsync(_mapper.Map<BuildingBaseInfo>(buildingBaseInfoRequest), cancellationToken);
        }


        public virtual async Task SaveAsync(UserInfo user, BuildingBaseInfoRequest buildingBaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (buildingBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }


            await Store.SaveAsync(_mapper.Map<SimpleUser>(user), _mapper.Map<BuildingBaseInfo>(buildingBaseInfoRequest), cancellationToken);
        }
    }
}
