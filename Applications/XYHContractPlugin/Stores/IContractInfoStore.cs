using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Models;
using XYHContractPlugin.Models;

namespace XYHContractPlugin.Stores
{
    public interface IContractInfoStore
    {
        IQueryable<ContractInfo> ContractInfos { get; set; }

        Task<ContractInfo> CreateAsync(ContractInfo buildingBaseInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(ContractInfo buildingBaseInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<ContractInfo> buildingBaseList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<ContractInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<ContractInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(ContractInfo buildingBase, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<ContractInfo> buildingBaseList, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveAsync(SimpleUser user, ContractInfo buildingBaseInfo, CancellationToken cancellationToken = default(CancellationToken));

    }
}
