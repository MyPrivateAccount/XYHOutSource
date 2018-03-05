using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Managers
{
    public class OrganizationExpansionManager
    {
        public OrganizationExpansionManager(IOrganizationExpansionStore organizationExpansionStore,
            IOrganizationStore organizationStore,
            ExtendUserManager<Users> extendUserManager
            )
        {
            Store = organizationExpansionStore ?? throw new ArgumentNullException(nameof(organizationExpansionStore));
            _organizationStore = organizationStore ?? throw new ArgumentNullException(nameof(organizationStore));
            _extendUserManager = extendUserManager ?? throw new ArgumentNullException(nameof(extendUserManager));
        }

        protected IOrganizationExpansionStore Store { get; }
        protected IOrganizationStore _organizationStore { get; }

        protected ExtendUserManager<Users> _extendUserManager { get; }
        public virtual async Task Expansion()
        {
            var oldData = await Store.ListAsync(a => a.Where(b => true));
            if (oldData?.Count > 0)
            {
                Store.DeleteListAsync(oldData).Wait();
            }
            List<OrganizationExpansion> list = new List<OrganizationExpansion>();
            var result = await GetAllSonExpansion(list, "0", "默认顶级", "", "");
            await Store.CreateListAsync(result);
        }

        public virtual async Task CreateAsync(Organization organization)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }
            List<OrganizationExpansion> list = new List<OrganizationExpansion>();
            var olist = await GetAllParent(new List<Organization>(), organization.ParentId);
            olist.Add(new Organization { Id = "0", OrganizationName = "默认顶级" });
            foreach (var item in olist)
            {
                list.Add(new OrganizationExpansion()
                {
                    OrganizationId = item.Id,
                    OrganizationName = item.OrganizationName,
                    SonId = organization.Id,
                    SonName = organization.OrganizationName,
                    City = item.City,
                    IsImmediate = organization.ParentId == item.Id,
                    FullName = item.FullName + "-" + organization.OrganizationName,
                    Type = item.Type
                });
            }
            await Store.CreateListAsync(list);
            return;
        }


        public virtual async Task DeleteAsync(Organization organization)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }
            var o = await Store.GetAsync(a => a.Where(b => b.OrganizationId == organization.Id));
            if (o == null)
            {
                var organizationExpansions = await Store.ListAsync(a => a.Where(b => b.OrganizationId == organization.Id || b.SonId == organization.Id));
                await Store.DeleteListAsync(organizationExpansions);
            }
        }

        public virtual async Task<List<Organization>> FindByParentOrMyOrganAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var organizationExpansions = await Store.ListAsync(a => a.Where(b => b.OrganizationId == id), cancellationToken);
            return await _organizationStore.ListAsync(a => a.Where(b => organizationExpansions.Select(x => x.SonId).Contains(b.Id) || b.Id == id), cancellationToken);
        }

        public virtual async Task<List<Organization>> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var organizationExpansions = await Store.ListAsync(a => a.Where(b => b.OrganizationId == id), cancellationToken);
            return await _organizationStore.ListAsync(a => a.Where(b => organizationExpansions.Select(x => x.SonId).Contains(b.Id)), cancellationToken);
        }

        public virtual async Task<IEnumerable<string>> GetAllSonIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var organizationExpansions = await Store.ListAsync(a => a.Where(b => b.OrganizationId == id), cancellationToken);
            return organizationExpansions.Select(a => a.SonId);
        }

        public virtual async Task<IEnumerable<string>> GetAllParentIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var organizationExpansions = await Store.ListAsync(a => a.Where(b => b.SonId == id), cancellationToken);
            return organizationExpansions.Select(a => a.OrganizationId);
        }


        public virtual async Task UpdateAsync(Organization organization)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }
            var organizationExpansions = await Store.ListAsync(a => a.Where(b => b.OrganizationId == organization.Id || b.SonId == organization.Id));
            for (int i = 0; i < organizationExpansions.Count; i++)
            {
                if (organizationExpansions[i].OrganizationId == organization.Id)
                {
                    organizationExpansions[i].OrganizationName = organization.OrganizationName;
                    organizationExpansions[i].City = organization.City;
                    organizationExpansions[i].FullName = organizationExpansions[i].FullName.Replace(organizationExpansions[i].FullName.Split('-')[0], organization.OrganizationName);
                    organizationExpansions[i].Type = organization.Type;
                }
                else if (organizationExpansions[i].SonId == organization.Id)
                {
                    organizationExpansions[i].SonName = organization.OrganizationName;
                }
            }
            //foreach (var item in organizationExpansions)
            //{
            //    if (item.OrganizationId == organization.Id)
            //    {
            //        item.OrganizationName = organization.OrganizationName;
            //    }
            //    else if (item.SonId == organization.Id)
            //    {
            //        item.SonName = organization.OrganizationName;
            //    }
            //}
            await Store.UpdateListAsync(organizationExpansions);
        }


        private async Task<List<OrganizationExpansion>> GetAllSonExpansion(List<OrganizationExpansion> list, string id, string name, string city, string type)
        {
            var organizations = await _organizationStore.ListAsync(a => a.Where(b => b.ParentId == id), CancellationToken.None);
            if (organizations?.Count > 0)
            {
                foreach (var item in organizations)
                {
                    list.Add(new OrganizationExpansion()
                    {
                        OrganizationId = id,
                        OrganizationName = name,
                        SonId = item.Id,
                        SonName = item.OrganizationName,
                        City = city,
                        IsImmediate = true,
                        Type = type,
                        FullName = name + "-" + item.OrganizationName
                    });
                    List<Organization> sonlist = new List<Organization>();
                    sonlist = await GetSonExpansion(sonlist, item.Id, item.OrganizationName, item.City, item.Type);
                    foreach (var l in sonlist)
                    {
                        list.Add(new OrganizationExpansion()
                        {
                            OrganizationId = id,
                            OrganizationName = name,
                            SonId = l.Id,
                            SonName = l.OrganizationName,
                            City = city,
                            IsImmediate = false,
                            Type = type,
                            FullName = name + "-" + l.FullName
                        });
                    }
                    await GetAllSonExpansion(list, item.Id, item.OrganizationName, item.City, item.Type);
                }
            }
            return list;
        }


        private async Task<List<Organization>> GetSonExpansion(List<Organization> list, string id, string fullname, string city, string type)
        {
            var organizations = await _organizationStore.ListAsync(a => a.Where(b => b.ParentId == id), CancellationToken.None);
            if (organizations?.Count > 0)
            {
                for (int i = 0; i < organizations.Count; i++)
                {
                    organizations[i].FullName = fullname + "-" + organizations[i].OrganizationName;
                    list.Add(organizations[i]);
                    await GetSonExpansion(list, organizations[i].Id, organizations[i].FullName, organizations[i].City, organizations[i].Type);
                }
                //foreach (var item in organizations)
                //{
                //    list.Add(item);
                //    await GetSonExpansion(list, item.Id, item.OrganizationName, item.City, item.Type);
                //}
            }
            return list;
        }




        private async Task<List<Organization>> GetAllParent(List<Organization> list, string id)
        {
            var organization = await _organizationStore.GetAsync(a => a.Where(b => b.Id == id), CancellationToken.None);
            if (organization != null)
            {
                list.Add(organization);
                await GetAllParent(list, organization.ParentId);
            }
            return list;
        }
    }
}
