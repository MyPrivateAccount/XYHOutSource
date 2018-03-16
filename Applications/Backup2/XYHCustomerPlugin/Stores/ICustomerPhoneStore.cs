using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public interface ICustomerPhoneStore
    {
        IQueryable<CustomerPhone> CustomerPhones { get; set; }

        IQueryable<CustomerPhone> CustomerPhoneAll();

        Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerPhone>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerPhone>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<CustomerPhone> CreateAsync(CustomerPhone customerphone, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(SimpleUser user, CustomerPhone customerphone, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<CustomerPhone> customerphoneList, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(CustomerPhone customerphone, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<CustomerPhone> customerphones, CancellationToken cancellationToken = default(CancellationToken));
    }
}
