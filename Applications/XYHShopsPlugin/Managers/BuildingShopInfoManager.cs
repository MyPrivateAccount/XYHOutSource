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
    public class BuildingShopInfoManager
    {
        public BuildingShopInfoManager(IBuildingShopInfoStore buildingShopInfoStore, IMapper mapper)
        {
            Store = buildingShopInfoStore ?? throw new ArgumentNullException(nameof(buildingShopInfoStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IBuildingShopInfoStore Store { get; }
        protected IMapper _mapper { get; }

        public virtual async Task<BuildingShopInfoResponse> CreateAsync(BuildingShopInfoRequest buildingBaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }
            var baseinfo = await Store.CreateAsync(_mapper.Map<BuildingShopInfo>(buildingBaseInfoRequest), cancellationToken);
            return _mapper.Map<BuildingShopInfoResponse>(baseinfo);
        }

        public virtual async Task<BuildingShopInfoResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var baseinfo = await Store.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<BuildingShopInfoResponse>(baseinfo);
        }

        public virtual async Task UpdateAsync(BuildingShopInfoRequest buildingBaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }
            await Store.UpdateAsync(_mapper.Map<BuildingShopInfo>(buildingBaseInfoRequest), cancellationToken);
        }


        public virtual async Task SaveAsync(UserInfo user, BuildingShopInfoRequest buildingBaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (buildingBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }
            await Store.SaveAsync(_mapper.Map<SimpleUser>(user), _mapper.Map<BuildingShopInfo>(buildingBaseInfoRequest), cancellationToken);
        }
    }
}
