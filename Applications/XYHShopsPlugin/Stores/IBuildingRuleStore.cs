using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Stores
{
    public interface IBuildingRuleStore
    {
        Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingRule>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<BuildingRule> SaveAsync(SimpleUser user, BuildingRule buildingRule, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveTemplateAsync(SimpleUser user, BuildingRule buildingRule, CancellationToken cancellationToken = default(CancellationToken));
    }
}
