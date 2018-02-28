using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHBaseDataPlugin.Models;

namespace XYHBaseDataPlugin.Stores
{
    public class AreaDefineStore : IAreaDefineStore
    {
        public AreaDefineStore(BaseDataDbContext baseDataDbContext)
        {
            Context = baseDataDbContext;
            AreaDefines = Context.AreaDefines;
        }

        protected BaseDataDbContext Context { get; }

        public IQueryable<AreaDefine> AreaDefines { get; set; }


        public async Task<AreaDefine> CreateAsync(AreaDefine areaDefine, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (areaDefine == null)
            {
                throw new ArgumentNullException(nameof(areaDefine));
            }
            try
            {
                Context.Add(areaDefine);
                await Context.SaveChangesAsync(cancellationToken);
                return areaDefine;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public async Task DeleteAsync(AreaDefine areaDefine, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (areaDefine == null)
            {
                throw new ArgumentNullException(nameof(areaDefine));
            }
            Context.Remove(areaDefine);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<AreaDefine> areaDefineList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (areaDefineList == null)
            {
                throw new ArgumentNullException(nameof(areaDefineList));
            }
            Context.RemoveRange(areaDefineList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<AreaDefine>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.AreaDefines.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<AreaDefine>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.AreaDefines.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(AreaDefine areaDefine, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (areaDefine == null)
            {
                throw new ArgumentNullException(nameof(areaDefine));
            }
          //  var entry = Context.Entry(areaDefine);
         //   if (entry == null)
         //   {
                Context.AreaDefines.Attach(areaDefine);
       //     }
         //   else
          //  {
             //   entry.CurrentValues.SetValues(areaDefine);
                Context.AreaDefines.Update(areaDefine);
          //  }
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { throw; }
        }

        public async Task UpdateListAsync(List<AreaDefine> areaDefineList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (areaDefineList == null)
            {
                throw new ArgumentNullException(nameof(areaDefineList));
            }
            Context.AttachRange(areaDefineList);
            Context.UpdateRange(areaDefineList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }
    }
}
