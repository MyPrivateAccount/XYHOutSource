using AuthorizationCenter.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public class UserLoginLogStore<TContext> : IUserLoginLogStore where TContext : ApplicationDbContext
    {
        public UserLoginLogStore(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            UserLoginLogs = Context.UserLoginLogs;
        }

        protected virtual TContext Context { get; }
        public IQueryable<UserLoginLog> UserLoginLogs { get; set; }


        public async Task<UserLoginLog> CreateAsync(UserLoginLog organization, CancellationToken cancellationToken)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }
            Context.Add(organization);
            await Context.SaveChangesAsync(cancellationToken);
            return organization;
        }


        public Task<TResult> GetAsync<TResult>(Func<IQueryable<UserLoginLog>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.UserLoginLogs).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<UserLoginLog>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.UserLoginLogs).ToListAsync(cancellationToken);
        }


    }
}
