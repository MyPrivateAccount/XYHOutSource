using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public class FileInfoStore : IFileInfoStore
    {
        public FileInfoStore(CustomerDbContext custoemerDbContext)
        {
            Context = custoemerDbContext;
        }

        protected CustomerDbContext Context { get; }

        public IQueryable<CustomerDealFileInfo> FileInfos { get; set; }


        public async Task<CustomerDealFileInfo> CreateAsync(CustomerDealFileInfo fileInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }
            Context.Add(fileInfo);
            await Context.SaveChangesAsync(cancellationToken);
            return fileInfo;
        }

        public async Task<List<CustomerDealFileInfo>> CreateListAsync(List<CustomerDealFileInfo> fileInfoList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfoList == null)
            {
                throw new ArgumentNullException(nameof(fileInfoList));
            }
            Context.AddRange(fileInfoList);
            await Context.SaveChangesAsync(cancellationToken);
            return fileInfoList;
        }


        public async Task DeleteAsync(CustomerDealFileInfo fileInfo, CancellationToken cancellationToken = default(CancellationToken))
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

        public async Task DeleteListAsync(List<CustomerDealFileInfo> fileInfoList, CancellationToken cancellationToken = default(CancellationToken))
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

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerDealFileInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.FileInfos.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerDealFileInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.FileInfos.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(CustomerDealFileInfo fileInfo, CancellationToken cancellationToken = default(CancellationToken))
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

        public async Task UpdateListAsync(List<CustomerDealFileInfo> fileInfoList, CancellationToken cancellationToken = default(CancellationToken))
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


        public async Task SaveAsync(SimpleUser user, string buildingId, string shopId, List<CustomerDealFileInfo> fileInfoList, CancellationToken cancellationToken = default(CancellationToken))
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