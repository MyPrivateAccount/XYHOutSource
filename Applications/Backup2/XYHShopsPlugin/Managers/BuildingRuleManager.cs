using ApplicationCore.Dto;
using ApplicationCore.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Dto.Request;
using XYHShopsPlugin.Dto.Response;
using XYHShopsPlugin.Models;
using XYHShopsPlugin.Stores;

namespace XYHShopsPlugin.Managers
{
    public class BuildingRuleManager
    {
        public BuildingRuleManager(IBuildingRuleStore buildingRuleStore, IMapper mapper)
        {
            Store = buildingRuleStore ?? throw new ArgumentNullException(nameof(buildingRuleStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IBuildingRuleStore Store { get; }
        protected IMapper _mapper { get; }

        public virtual async Task<BuildingRuleInfoResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var facilities = await Store.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<BuildingRuleInfoResponse>(facilities);
        }

        public virtual async Task<BuildingRuleInfoResponse> SaveAsync(UserInfo user, BuildingRuleRequest buildingRuleRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (buildingRuleRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingRuleRequest));
            }


            return _mapper.Map<BuildingRuleInfoResponse>(await Store.SaveAsync(_mapper.Map<SimpleUser>(user), _mapper.Map<BuildingRule>(buildingRuleRequest), cancellationToken));
        }

        public virtual async Task SaveTemplateAsync(UserInfo user, BuildingRuleRequest buildingRuleRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (buildingRuleRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingRuleRequest));
            }


            await Store.SaveTemplateAsync(_mapper.Map<SimpleUser>(user), _mapper.Map<BuildingRule>(buildingRuleRequest), cancellationToken);
        }
    }
}
