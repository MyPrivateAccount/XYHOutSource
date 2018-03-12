using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public interface ICustomerReportStore
    {
        IQueryable<CustomerReport> CustomerReportAll();

        Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerReport>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerReport>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<CustomerReport> CreateAsync(CustomerReport customerreport, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(SimpleUser user, CustomerReport customerreport, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<CustomerReport> customerreportList, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(CustomerReport customerreport, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<CustomerReport> customerreports, CancellationToken cancellationToken = default(CancellationToken));
    }
}
