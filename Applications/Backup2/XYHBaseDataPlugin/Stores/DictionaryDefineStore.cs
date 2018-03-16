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
    public class DictionaryDefineStore : IDictionaryDefineStore
    {
        public DictionaryDefineStore(BaseDataDbContext baseDataDbContext)
        {
            Context = baseDataDbContext;
            DictionaryDefines = Context.DictionaryDefines;

        }

        protected BaseDataDbContext Context { get; }

        public IQueryable<DictionaryDefine> DictionaryDefines { get; set; }


        public async Task<DictionaryDefine> CreateAsync(DictionaryDefine dictionaryDefine, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dictionaryDefine == null)
            {
                throw new ArgumentNullException(nameof(dictionaryDefine));
            }
            Context.Add(dictionaryDefine);
            await Context.SaveChangesAsync(cancellationToken);
            return dictionaryDefine;
        }


        public async Task DeleteAsync(DictionaryDefine dictionaryDefine, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dictionaryDefine == null)
            {
                throw new ArgumentNullException(nameof(dictionaryDefine));
            }
            Context.Remove(dictionaryDefine);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<DictionaryDefine> dictionaryDefineList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dictionaryDefineList == null)
            {
                throw new ArgumentNullException(nameof(dictionaryDefineList));
            }
            Context.RemoveRange(dictionaryDefineList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<DictionaryDefine>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.DictionaryDefines.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<DictionaryDefine>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.DictionaryDefines.OrderBy(a => a.Order)).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(DictionaryDefine dictionaryDefine, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dictionaryDefine == null)
            {
                throw new ArgumentNullException(nameof(dictionaryDefine));
            }
            Context.Attach(dictionaryDefine);
            Context.Update(dictionaryDefine);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task UpdateListAsync(List<DictionaryDefine> dictionaryDefineList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dictionaryDefineList == null)
            {
                throw new ArgumentNullException(nameof(dictionaryDefineList));
            }
            Context.AttachRange(dictionaryDefineList);
            Context.UpdateRange(dictionaryDefineList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }
    }
}
