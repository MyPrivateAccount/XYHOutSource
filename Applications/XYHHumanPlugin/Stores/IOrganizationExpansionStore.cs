using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Stores
{
    public interface IOrganizationExpansionStore
    {
        string GetFullName(string departmentid);

        string GetFullNameByUserId(string userid);

        //Task<TResult> GetAsync<TResult>(Func<IQueryable<OrganizationExpansion>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        //Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<OrganizationExpansion>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
    }
}
