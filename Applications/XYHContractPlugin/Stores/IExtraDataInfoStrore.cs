using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using XYHContractPlugin.Models;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Dto;

namespace XYHContractPlugin.Stores
{
    public interface IExtraDataInfoStrore
    {
        IQueryable<CompanyAInfo> CompanyAInfo { get; set; }
        Task<CompanyAInfo> CreateAsync(CompanyAInfo companyAInfo, CancellationToken cancellationToken = default(CancellationToken));
        //Task<List<FileInfo>> CreateListAsync(List<CompanyAInfo> companyAInfoList, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteAsync(UserInfo userInfo, CompanyAInfo companyAInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<CompanyAInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CompanyAInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(CompanyAInfo fileInfo, CancellationToken cancellationToken = default(CancellationToken));

        IQueryable<CompanyAInfo> CompanyAInfoAll();
    }
}
