using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using XYHContractPlugin.Models;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Dto;

namespace XYHContractPlugin.Stores
{
    public class ExtraDataInfoStore : IExtraDataInfoStrore
    {
        public ExtraDataInfoStore(ContractDbContext dbContext)
        {
            Context = dbContext;
            CompanyAInfo = Context.CompanyAInfo;
        }
        protected ContractDbContext Context;
        public IQueryable<CompanyAInfo> CompanyAInfo { get; set; }
        public async Task<CompanyAInfo> CreateAsync(CompanyAInfo companyAInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(companyAInfo == null)
            {
                throw new ArgumentNullException(nameof(CompanyAInfo));
            }

            Context.CompanyAInfo.Add(companyAInfo);
            await Context.SaveChangesAsync(cancellationToken);
            return companyAInfo;
        }
        //Task<List<FileInfo>> CreateListAsync(List<CompanyAInfo> companyAInfoList, CancellationToken cancellationToken = default(CancellationToken));
        public async Task DeleteAsync(UserInfo userInfo, CompanyAInfo companyAInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(companyAInfo == null)
            {
                throw new ArgumentException(nameof(CompanyAInfo));

            }
            companyAInfo.DeleteTime = DateTime.Now;
            companyAInfo.DeleteUser = userInfo.Id;
            companyAInfo.IsDelete = true;
            Context.CompanyAInfo.Update(companyAInfo);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch(DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(UserInfo userInfo, List<CompanyAInfo> Infos, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (Infos == null || Infos.Count == 0)
            {
                throw new ArgumentException(nameof(Infos));

            }
       
            Context.CompanyAInfo.AttachRange(Infos);
            Context.CompanyAInfo.UpdateRange(Infos);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<CompanyAInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CompanyAInfo.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CompanyAInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CompanyAInfo.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(CompanyAInfo companyAInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (companyAInfo == null)
            {
                throw new ArgumentNullException(nameof(companyAInfo));
            }
            Context.CompanyAInfo.Attach(companyAInfo);
            Context.CompanyAInfo.Update(companyAInfo);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { throw; }
        }
        public IQueryable<CompanyAInfo> CompanyAInfoAll()
        {
            var query = from a in Context.CompanyAInfo.AsNoTracking()
                        select a;
            return query;
        }
    }
}
