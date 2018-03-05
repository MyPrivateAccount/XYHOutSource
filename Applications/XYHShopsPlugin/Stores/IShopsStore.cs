using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Stores
{
    public interface IShopsStore
    {
        IQueryable<Shops> Shops { get; set; }


        IQueryable<Shops> GetSimpleQuery();

        IQueryable<Shops> GetDetailQuery();

        Task<Shops> CreateAsync(Shops shops, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(SimpleUser user, Shops shops, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<Shops> shopsList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<Shops>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<Shops>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(Shops shops, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<Shops> shopsList, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveSummaryAsync(SimpleUser user, string buildingId, string shopId, string summary, string source, string sourceId, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateExamineStatus(string shopId, ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken));

        Task<CustomerNotShops> CreateCustomerNotShopAsync(CustomerNotShops customerNotShops, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListNotShopsAsync<TResult>(Func<IQueryable<CustomerNotShops>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
    }
}
