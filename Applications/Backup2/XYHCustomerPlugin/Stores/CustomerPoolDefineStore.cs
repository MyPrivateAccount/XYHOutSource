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
    public class CustomerPoolDefineStore : ICustomerPoolDefineStore
    {
        public CustomerPoolDefineStore(CustomerDbContext customerDbContext)
        {
            Context = customerDbContext;
            CustomerPoolDefines = Context.CustomerPoolDefines;
        }

        protected CustomerDbContext Context { get; }

        public IQueryable<CustomerPoolDefine> CustomerPoolDefines { get; set; }


        public async Task<CustomerPoolDefine> CreateAsync(CustomerPoolDefine customerPoolDefine, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerPoolDefine == null)
            {
                throw new ArgumentNullException(nameof(customerPoolDefine));
            }
            Context.Add(customerPoolDefine);
            await Context.SaveChangesAsync(cancellationToken);
            return customerPoolDefine;
        }


        public async Task DeleteAsync(CustomerPoolDefine customerPoolDefine, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerPoolDefine == null)
            {
                throw new ArgumentNullException(nameof(customerPoolDefine));
            }
            Context.Remove(customerPoolDefine);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<CustomerPoolDefine> customerPoolDefineList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerPoolDefineList == null)
            {
                throw new ArgumentNullException(nameof(customerPoolDefineList));
            }
            Context.RemoveRange(customerPoolDefineList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerPoolDefine>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerPoolDefines.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerPoolDefine>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerPoolDefines.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(CustomerPoolDefine customerPoolDefine, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerPoolDefine == null)
            {
                throw new ArgumentNullException(nameof(customerPoolDefine));
            }
            Context.Attach(customerPoolDefine);
            Context.Update(customerPoolDefine);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task UpdateListAsync(List<CustomerPoolDefine> customerPoolDefineList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerPoolDefineList == null)
            {
                throw new ArgumentNullException(nameof(customerPoolDefineList));
            }
            Context.AttachRange(customerPoolDefineList);
            Context.UpdateRange(customerPoolDefineList);
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
