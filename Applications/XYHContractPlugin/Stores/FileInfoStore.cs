using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHContractPlugin.Models;


namespace XYHContractPlugin.Stores
{
    public class FileInfoStore : IFileInfoStore
    {
        public FileInfoStore(ContractDbContext DbContext)
        {
            Context = DbContext;
            FileInfos = Context.FileInfos;
        }

        protected ContractDbContext Context { get; }

        public IQueryable<FileInfo> FileInfos { get; set; }


        public async Task<FileInfo> CreateAsync(FileInfo fileInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }
            Context.Add(fileInfo);
            await Context.SaveChangesAsync(cancellationToken);
            return fileInfo;
        }

        public async Task<List<FileInfo>> CreateListAsync(List<FileInfo> fileInfoList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfoList == null)
            {
                throw new ArgumentNullException(nameof(fileInfoList));
            }
            Context.AddRange(fileInfoList);
            await Context.SaveChangesAsync(cancellationToken);
            return fileInfoList;
        }


        public async Task DeleteAsync(FileInfo fileInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }
            Context.Remove(fileInfo);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<FileInfo> fileInfoList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfoList == null)
            {
                throw new ArgumentNullException(nameof(fileInfoList));
            }
            Context.RemoveRange(fileInfoList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<FileInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.FileInfos.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<FileInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.FileInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(FileInfo fileInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }
            Context.Attach(fileInfo);
            Context.Update(fileInfo);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task UpdateListAsync(List<FileInfo> fileInfoList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfoList == null)
            {
                throw new ArgumentNullException(nameof(fileInfoList));
            }
            Context.AttachRange(fileInfoList);
            Context.UpdateRange(fileInfoList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }


        public async Task SaveAsync(SimpleUser user, string buildingId, string shopId, List<FileInfo> fileInfoList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfoList == null)
            {
                throw new ArgumentNullException(nameof(fileInfoList));
            }
            Context.AttachRange(fileInfoList);
            Context.UpdateRange(fileInfoList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

    }
}
