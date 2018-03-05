using AuthorizationCenter.Dto;
using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore;
using OpenIddict.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Managers
{
    public class ApplicationManager : OpenIddictApplicationManager<Applications>
    {
        public ApplicationManager(IOpenIddictApplicationStore<Applications> applicationStore,
            ILogger<OpenIddictApplicationManager<Applications>> logger,
            ApplicationStore store
            ) : base(applicationStore, logger)
        {
            Store = store;
        }
        /// <summary>
        /// Gets the store associated with the current manager.
        /// </summary>
        protected new ApplicationStore Store { get; }


        public async Task<List<Applications>> ListAsync(ApplicationSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            var q = Store.Applications;
            if (!string.IsNullOrEmpty(condition?.KeyWords))
            {
                q = q.Where(a => a.DisplayName.Contains(condition.KeyWords));
            }
            if (condition?.ApplicationTypes?.Count > 0)
            {
                q = q.Where(a => condition.ApplicationTypes.Contains(a.ApplicationType));
            }
            if (condition?.Ids?.Count > 0)
            {
                q = q.Where(a => condition.Ids.Contains(a.Id));
            }
            return await q.ToListAsync();
        }

        public async Task DeleteList(List<Applications> applicationsList)
        {
            await Store.DeleteListAsync(applicationsList);
        }

        public async Task DeleteListAsync(List<string> openIddictApplicationIds)
        {
            var openIddictApplications = await Store.ListAsync(a => a.Where(b => openIddictApplicationIds.Contains(b.Id)));
            if (openIddictApplications?.Count == 0)
            {
                return;
            }
            await Store.DeleteListAsync(openIddictApplications);
        }
    }
}
