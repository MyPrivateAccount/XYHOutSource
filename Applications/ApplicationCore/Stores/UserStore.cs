using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationCore.Stores
{
    public class UserStore<TContext> : IUserStore where TContext : CoreDbContext
    {
        public UserStore(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }
        protected virtual TContext Context { get; }

        public virtual async Task<TResult> GetAsync<TResult>(Func<IQueryable<Users>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return await query.Invoke(Context.Users).FirstOrDefaultAsync(cancellationToken);
        }

        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<Users>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.Users).ToListAsync(cancellationToken);
        }

    }
}
