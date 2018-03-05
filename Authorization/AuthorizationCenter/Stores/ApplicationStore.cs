using AuthorizationCenter.Models;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore;
using OpenIddict.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AuthorizationCenter.Stores
{

    public class ApplicationStore : OpenIddictApplicationStore<Applications, Authorization, Token, ApplicationDbContext, string>, IApplicationStore
    {
        public ApplicationStore(ApplicationDbContext context) : base(context)
        {
            Context = context;
            Applications = Context.Applications;
        }
        protected new ApplicationDbContext Context { get; }

        public new IQueryable<Applications> Applications { get; set; }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<Applications>, IQueryable<TResult>> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return query.Invoke(Context.Applications).SingleOrDefaultAsync();
        }


        public new Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<Applications>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.Applications).ToListAsync(cancellationToken);
        }


        public async Task DeleteListAsync(List<Applications> openIddictApplications)
        {
            if (openIddictApplications == null)
            {
                throw new ArgumentNullException(nameof(openIddictApplications));
            }
            Context.RemoveRange(openIddictApplications);
            try
            {
                await Context.SaveChangesAsync();
                //Context.RemoveRange(openIddictApplications);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

    }
}
