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
    public class UpdateRecordFileScopeStore : IUpdateRecordFileScopeStore
    {
        public UpdateRecordFileScopeStore(ShopsDbContext shopsDbContext)
        {
            Context = shopsDbContext;
            UpdateRecordFileScopes = Context.UpdateRecordFileScopes;
        }
        protected ShopsDbContext Context { get; }
        public IQueryable<UpdateRecordFileScope> UpdateRecordFileScopes { get; set; }

        public async Task<UpdateRecordFileScope> CreateAsync(UpdateRecordFileScope updateRecordFileScope, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (updateRecordFileScope == null)
            {
                throw new ArgumentNullException(nameof(updateRecordFileScope));
            }
            Context.Add(updateRecordFileScope);
            await Context.SaveChangesAsync(cancellationToken);
            return updateRecordFileScope;
        }


        public async Task DeleteAsync(UpdateRecordFileScope updateRecordFileScope, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (updateRecordFileScope == null)
            {
                throw new ArgumentNullException(nameof(updateRecordFileScope));
            }
            Context.Remove(updateRecordFileScope);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<UpdateRecordFileScope> updateRecordFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (updateRecordFileScopeList == null)
            {
                throw new ArgumentNullException(nameof(updateRecordFileScopeList));
            }
            Context.RemoveRange(updateRecordFileScopeList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<UpdateRecordFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.UpdateRecordFileScopes).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<UpdateRecordFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.UpdateRecordFileScopes).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(UpdateRecordFileScope updateRecordFileScope, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (updateRecordFileScope == null)
            {
                throw new ArgumentNullException(nameof(updateRecordFileScope));
            }
            Context.Attach(updateRecordFileScope);
            Context.Update(updateRecordFileScope);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task UpdateListAsync(List<UpdateRecordFileScope> updateRecordFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (updateRecordFileScopeList == null)
            {
                throw new ArgumentNullException(nameof(updateRecordFileScopeList));
            }
            Context.AttachRange(updateRecordFileScopeList);
            Context.UpdateRange(updateRecordFileScopeList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public async Task SaveAsync(SimpleUser user, string buildingId, List<UpdateRecordFileScope> updateRecordFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (updateRecordFileScopeList == null || updateRecordFileScopeList.Count == 0)
                return;

            foreach (UpdateRecordFileScope file in updateRecordFileScopeList)
            {
                if (String.IsNullOrEmpty(file.FileGuid))
                {
                    file.FileGuid = Guid.NewGuid().ToString("N").ToLower();
                }
                //基本信息
                if (!Context.UpdateRecordFileScopes.Any(x => x.FileGuid == file.FileGuid))
                {
                    file.CreateTime = DateTime.Now;
                    file.CreateUser = user.Id;
                    Context.Add(file);
                }
                else
                {
                    Context.Attach(file);
                    Context.Update(file);
                }
            }
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }



    }
}
