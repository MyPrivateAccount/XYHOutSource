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
    public class DictionaryGroupStore : IDictionaryGroupStore
    {
        public DictionaryGroupStore(BaseDataDbContext baseDataDbContext)
        {
            Context = baseDataDbContext;
            DictionaryGroups = Context.DictionaryGroups;
        }

        protected BaseDataDbContext Context { get; }

        public IQueryable<DictionaryGroup> DictionaryGroups { get; set; }

        public async Task<DictionaryGroup> CreateAsync(DictionaryGroup dictionaryGroup, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dictionaryGroup == null)
            {
                throw new ArgumentNullException(nameof(dictionaryGroup));
            }
            Context.Add(dictionaryGroup);
            await Context.SaveChangesAsync(cancellationToken);
            return dictionaryGroup;
        }


        public async Task DeleteAsync(DictionaryGroup dictionaryGroup, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dictionaryGroup == null)
            {
                throw new ArgumentNullException(nameof(dictionaryGroup));
            }
            Context.Remove(dictionaryGroup);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<DictionaryGroup> dictionaryGroupList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dictionaryGroupList == null)
            {
                throw new ArgumentNullException(nameof(dictionaryGroupList));
            }
            Context.RemoveRange(dictionaryGroupList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<DictionaryGroup>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.DictionaryGroups).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<DictionaryGroup>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.DictionaryGroups).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(DictionaryGroup dictionaryGroup, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dictionaryGroup == null)
            {
                throw new ArgumentNullException(nameof(dictionaryGroup));
            }
            Context.Attach(dictionaryGroup);
            Context.Update(dictionaryGroup);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }
        public async Task UpdateListAsync(List<DictionaryGroup> dictionaryGroups, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dictionaryGroups == null)
            {
                throw new ArgumentNullException(nameof(dictionaryGroups));
            }
            Context.AttachRange(dictionaryGroups);
            Context.UpdateRange(dictionaryGroups);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

    }
}
