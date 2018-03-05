using AuthorizationCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public class ExtendUserStore<TContext> : UserStore<Users>, IExtendUserStore where TContext : ApplicationDbContext
    {
        public ExtendUserStore(TContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }


        protected new TContext Context { get; private set; }


        public Task<TResult> GetAsync<TResult>(Func<IQueryable<Users>, IQueryable<TResult>> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return query.Invoke(Context.Users).SingleOrDefaultAsync();
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<Users>, IQueryable<TResult>> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.Users).ToListAsync();
        }


        public async Task DeleteListAsync(List<Users> userList, CancellationToken cancellationToken)
        {
            if (userList == null)
            {
                throw new ArgumentNullException(nameof(userList));
            }
            Context.RemoveRange(userList);
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
