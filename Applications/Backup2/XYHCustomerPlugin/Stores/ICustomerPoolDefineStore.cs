using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public interface ICustomerPoolDefineStore
    {
        IQueryable<CustomerPoolDefine> CustomerPoolDefines { get; set; }


        Task<CustomerPoolDefine> CreateAsync(CustomerPoolDefine customerPoolDefine, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(CustomerPoolDefine customerPoolDefine, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<CustomerPoolDefine> customerPoolDefineList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerPoolDefine>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerPoolDefine>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(CustomerPoolDefine customerPoolDefine, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<CustomerPoolDefine> customerPoolDefineList, CancellationToken cancellationToken = default(CancellationToken));
    }
}
