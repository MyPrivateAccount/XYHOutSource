
using ApplicationCore.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationCore.Managers
{
    public class OrganizationExpansionManager
    {
        public OrganizationExpansionManager(IOrganizationExpansionStore organizationExpansionStore)
        {
            Store = organizationExpansionStore ?? throw new ArgumentNullException(nameof(organizationExpansionStore));
        }

        protected IOrganizationExpansionStore Store { get; }

        public virtual async Task<List<string>> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var organizationExpansions = await Store.ListAsync(a => a.Where(b => b.OrganizationId == id), cancellationToken);
            return organizationExpansions.Select(x => x.SonId).ToList();
        }



    }
}
