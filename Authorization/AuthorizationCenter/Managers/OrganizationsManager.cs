using AuthorizationCenter.Dto;
using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Managers
{
    public class OrganizationsManager
    {
        public OrganizationsManager(IOrganizationStore organizationStore)
        {
            Store = organizationStore ?? throw new ArgumentNullException(nameof(organizationStore));
        }

        protected IOrganizationStore Store { get; }


        public virtual async Task<Organization> CreateAsync(Organization organization, CancellationToken cancellationToken)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }
            return await Store.CreateAsync(organization, cancellationToken);
        }

        public virtual Task DeleteAsync(Organization organization, CancellationToken cancellationToken)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }
            return Store.DeleteAsync(organization, cancellationToken);
        }
        public virtual Task DeleteListAsync(List<Organization> organizationList, CancellationToken cancellationToken)
        {
            if (organizationList == null)
            {
                throw new ArgumentNullException(nameof(organizationList));
            }
            return Store.DeleteListAsync(organizationList, cancellationToken);
        }


        public virtual Task<Organization> FindByIdAsync(string id, CancellationToken cancellationToken)
        {
            return Store.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
        }

        public virtual Task<List<Organization>> FindByParentAsync(string organizationId, CancellationToken cancellationToken)
        {
            return Store.ListAsync(a => a.Where(b => b.ParentId == organizationId && !b.IsDeleted), cancellationToken);
        }

        public virtual Task<List<Organization>> GetListAsync(List<string> organizationIds, CancellationToken cancellationToken)
        {
            return Store.ListAsync(a => a.Where(b => organizationIds.Contains(b.Id) && !b.IsDeleted), cancellationToken);
        }

        public virtual async Task UpdateAsync(Organization organization, CancellationToken cancellationToken)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }
            await Store.UpdateAsync(organization, cancellationToken);
        }

        public virtual async Task<string> FindFilialeIdAsync(string organizationId)
        {
            return await FindFilialeId(organizationId);
        }

        public virtual async Task<List<Organization>> Search(OrganizationSearchCondition condition, CancellationToken cancellationToken)
        {
            var query = Store.Organizations.Where(a => !a.IsDeleted);
            if (condition?.idList?.Count > 0)
            {
                query = query.Where(a => condition.idList.Contains(a.Id));
            }
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<bool> OrganizationExists(string id, CancellationToken cancellationToken)
        {
            return await Store.OrganizationExists(id, cancellationToken);
        }


        private async Task<string> FindFilialeId(string organizationId)
        {
            var organization = await Store.GetAsync(a => a.Where(b => b.Id == organizationId && !b.IsDeleted), CancellationToken.None);
            if (organization != null)
            {
                if (organization.Type == OrganizationType.Filiale)
                {
                    return organizationId;
                }
                await FindFilialeId(organization.ParentId);
            }
            return null;
        }

    }
}
