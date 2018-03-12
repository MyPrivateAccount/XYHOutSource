using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public interface IFileInfoStore
    {
        IQueryable<CustomerDealFileInfo> FileInfos { get; set; }

        Task<CustomerDealFileInfo> CreateAsync(CustomerDealFileInfo fileInfo, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<CustomerDealFileInfo>> CreateListAsync(List<CustomerDealFileInfo> fileInfoList, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(CustomerDealFileInfo fileInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<CustomerDealFileInfo> fileInfoList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerDealFileInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerDealFileInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(CustomerDealFileInfo fileInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<CustomerDealFileInfo> fileInfoList, CancellationToken cancellationToken = default(CancellationToken));
    }
}
