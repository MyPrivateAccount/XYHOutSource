using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Stores
{
    public interface IFileInfoStore
    {
        IQueryable<FileInfo> FileInfos { get; set; }

        Task<FileInfo> CreateAsync(FileInfo fileInfo, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<FileInfo>> CreateListAsync(List<FileInfo> fileInfoList, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(FileInfo fileInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<FileInfo> fileInfoList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<FileInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<FileInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(FileInfo fileInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<FileInfo> fileInfoList, CancellationToken cancellationToken = default(CancellationToken));

    }
}
