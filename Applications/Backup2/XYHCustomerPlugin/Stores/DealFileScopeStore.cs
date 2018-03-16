using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public class DealFileScopeStore: IDealFileScopeStore
    {
        public DealFileScopeStore(CustomerDbContext customerDbContext)
        {
            Context = customerDbContext;
            DealFileScopes = Context.DealFileScopes;
        }
        protected CustomerDbContext Context { get; }
        public IQueryable<DealFileScope> DealFileScopes { get; set; }

        public async Task<DealFileScope> CreateAsync(DealFileScope dealFileScope, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dealFileScope == null)
            {
                throw new ArgumentNullException(nameof(dealFileScope));
            }
            Context.Add(dealFileScope);
            await Context.SaveChangesAsync(cancellationToken);
            return dealFileScope;
        }


        public async Task DeleteAsync(DealFileScope dealFileScope, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dealFileScope == null)
            {
                throw new ArgumentNullException(nameof(dealFileScope));
            }
            Context.Remove(dealFileScope);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<DealFileScope> dealFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dealFileScopeList == null)
            {
                throw new ArgumentNullException(nameof(dealFileScopeList));
            }
            Context.RemoveRange(dealFileScopeList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<DealFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.DealFileScopes).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<DealFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.DealFileScopes).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(DealFileScope shopsFileScope, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopsFileScope == null)
            {
                throw new ArgumentNullException(nameof(shopsFileScope));
            }
            Context.Attach(shopsFileScope);
            Context.Update(shopsFileScope);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task UpdateListAsync(List<DealFileScope> dealFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (dealFileScopeList == null)
            {
                throw new ArgumentNullException(nameof(dealFileScopeList));
            }
            Context.AttachRange(dealFileScopeList);
            Context.UpdateRange(dealFileScopeList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public async Task SaveAsync(SimpleUser user, string dealId, List<DealFileScope> shopsFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (shopsFileScopeList == null || shopsFileScopeList.Count == 0)
                return;

            foreach (DealFileScope file in shopsFileScopeList)
            {
                if (String.IsNullOrEmpty(file.FileGuid))
                {
                    file.FileGuid = Guid.NewGuid().ToString("N").ToLower();
                }
                //基本信息
                if (!Context.DealFileScopes.Any(x => x.FileGuid == file.FileGuid))
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