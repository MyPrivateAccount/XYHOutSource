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
    public class CustomerFilescopeStore : ICustomerFilescopeStore
    {
        public CustomerFilescopeStore(CustomerDbContext baseDataDbContext)
        {
            Context = baseDataDbContext;
            CustomerFileScopes = Context.CustomerFileScopes;
        }

        protected CustomerDbContext Context { get; }

        public IQueryable<CustomerFileScope> CustomerFileScopes { get; set; }


        public async Task<CustomerFileScope> CreateAsync(CustomerFileScope customerFileScope, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerFileScope == null)
            {
                throw new ArgumentNullException(nameof(customerFileScope));
            }
            Context.Add(customerFileScope);
            await Context.SaveChangesAsync(cancellationToken);
            return customerFileScope;
        }


        public async Task DeleteAsync(CustomerFileScope customerFileScope, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerFileScope == null)
            {
                throw new ArgumentNullException(nameof(customerFileScope));
            }
            Context.Remove(customerFileScope);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<CustomerFileScope> customerFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerFileScopeList == null)
            {
                throw new ArgumentNullException(nameof(customerFileScopeList));
            }
            Context.RemoveRange(customerFileScopeList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerFileScopes).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerFileScopes).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(CustomerFileScope customerFileScope, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerFileScope == null)
            {
                throw new ArgumentNullException(nameof(customerFileScope));
            }
            Context.Attach(customerFileScope);
            Context.Update(customerFileScope);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task UpdateListAsync(List<CustomerFileScope> customerFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerFileScopeList == null)
            {
                throw new ArgumentNullException(nameof(customerFileScopeList));
            }
            Context.AttachRange(customerFileScopeList);
            Context.UpdateRange(customerFileScopeList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public async Task SaveAsync(SimpleUser user, string buildingId, List<CustomerFileScope> customerFileScopeList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (customerFileScopeList == null || customerFileScopeList.Count == 0)
                return;

            foreach (CustomerFileScope file in customerFileScopeList)
            {
                if (String.IsNullOrEmpty(file.FileGuid))
                {
                    file.FileGuid = Guid.NewGuid().ToString("N").ToLower();
                }
                //基本信息
                if (!Context.CustomerFileScopes.Any(x => x.FileGuid == file.FileGuid))
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
