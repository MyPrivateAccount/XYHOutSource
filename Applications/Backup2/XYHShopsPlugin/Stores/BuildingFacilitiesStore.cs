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
    public class BuildingFacilitiesStore : IBuildingFacilitiesStore
    {
        public BuildingFacilitiesStore(ShopsDbContext baseDataDbContext)
        {
            Context = baseDataDbContext;
            BuildingFacilities = Context.BuildingFacilities;
        }

        protected ShopsDbContext Context { get; }

        public IQueryable<BuildingFacilities> BuildingFacilities { get; set; }


        public async Task<BuildingFacilities> CreateAsync(BuildingFacilities buildingFacilities, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingFacilities == null)
            {
                throw new ArgumentNullException(nameof(buildingFacilities));
            }
            Context.Add(buildingFacilities);
            await Context.SaveChangesAsync(cancellationToken);
            return buildingFacilities;
        }


        public async Task DeleteAsync(BuildingFacilities buildingFacilities, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingFacilities == null)
            {
                throw new ArgumentNullException(nameof(buildingFacilities));
            }
            Context.Remove(buildingFacilities);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<BuildingFacilities> buildingFacilitiesList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingFacilitiesList == null)
            {
                throw new ArgumentNullException(nameof(buildingFacilitiesList));
            }
            Context.RemoveRange(buildingFacilitiesList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingFacilities>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BuildingFacilities).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BuildingFacilities>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BuildingFacilities).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(BuildingFacilities buildingFacilities, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingFacilities == null)
            {
                throw new ArgumentNullException(nameof(buildingFacilities));
            }
            Context.Attach(buildingFacilities);
            Context.Update(buildingFacilities);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task UpdateListAsync(List<BuildingFacilities> buildingFacilitiesList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingFacilitiesList == null)
            {
                throw new ArgumentNullException(nameof(buildingFacilitiesList));
            }
            Context.AttachRange(buildingFacilitiesList);
            Context.UpdateRange(buildingFacilitiesList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }


        public async Task SaveAsync(SimpleUser user, BuildingFacilities buildingFacilities, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (buildingFacilities == null)
            {
                throw new ArgumentNullException(nameof(buildingFacilities));
            }

            //查看楼盘是否存在
            if (!Context.Buildings.Any(x => x.Id == buildingFacilities.Id))
            {
                Buildings buildings = new Buildings()
                {
                    Id = buildingFacilities.Id,
                    CreateUser = user.Id,
                    ResidentUser1 = user.Id,
                    CreateTime = DateTime.Now,
                    OrganizationId = user.OrganizationId,
                    ExamineStatus = 0
                };

                Context.Add(buildings);
            }
            //基本信息
            if (!Context.BuildingFacilities.Any(x => x.Id == buildingFacilities.Id))
            {
                Context.Add(buildingFacilities);
            }
            else
            {
                Context.Attach(buildingFacilities);
                Context.Update(buildingFacilities);
            }
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }


    }
}
