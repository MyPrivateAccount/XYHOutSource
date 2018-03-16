using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XYHCustomerPlugin.Stores
{
    public interface IOrganizationExpansionStore
    {
        string GetFullName(string departmentid);

        string GetFullNameByUserId(string userid);

        //Task<TResult> GetAsync<TResult>(Func<IQueryable<OrganizationExpansion>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        //Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<OrganizationExpansion>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
    }
}
