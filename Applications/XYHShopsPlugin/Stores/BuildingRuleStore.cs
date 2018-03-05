using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Stores
{
    public class BuildingRuleStore : IBuildingRuleStore
    {
        public BuildingRuleStore(ShopsDbContext baseDataDbContext)
        {
            Context = baseDataDbContext;
        }

        protected ShopsDbContext Context { get; }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingRule>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BuildingRules).SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<BuildingRule> SaveAsync(SimpleUser user, BuildingRule buildingRule, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (buildingRule == null)
            {
                throw new ArgumentNullException(nameof(buildingRule));
            }

            //查看楼盘是否存在
            if (!Context.Buildings.Any(x => x.Id == buildingRule.Id))
            {
                Buildings buildings = new Buildings()
                {
                    Id = buildingRule.Id,
                    CreateUser = user.Id,
                    ResidentUser1 = user.Id,
                    CreateTime = DateTime.Now,
                    OrganizationId = user.OrganizationId,
                    ExamineStatus = 0
                };

                Context.Add(buildings);
            }
            //基本信息
            if (!Context.BuildingRules.Any(x => x.Id == buildingRule.Id))
            {
                Context.Add(buildingRule);
            }
            else
            {
                Context.Attach(buildingRule);
                Context.Update(buildingRule);
            }
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }

            return buildingRule;
        }


        public async Task SaveTemplateAsync(SimpleUser user, BuildingRule buildingRule, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (buildingRule == null)
            {
                throw new ArgumentNullException(nameof(buildingRule));
            }

            //查看楼盘是否存在
            if (!Context.Buildings.Any(x => x.Id == buildingRule.Id))
            {
                Buildings buildings = new Buildings()
                {
                    Id = buildingRule.Id,
                    CreateUser = user.Id,
                    ResidentUser1 = user.Id,
                    CreateTime = DateTime.Now,
                    OrganizationId = user.OrganizationId,
                    ExamineStatus = 0
                };

                Context.Add(buildings);
            }
            //基本信息
            if (!Context.BuildingRules.Any(x => x.Id == buildingRule.Id))
            {
                Context.Add(buildingRule);
            }
            else
            {
                Context.Attach(buildingRule);
                var entry = Context.Entry(buildingRule);
                entry.Property(x => x.ReportedTemplate).IsModified = true;
            }
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }
    }
}
