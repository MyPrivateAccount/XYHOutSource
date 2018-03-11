using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Models;
using XYHContractPlugin.Models;
using XYHContractPlugin.Dto.Response;

namespace XYHContractPlugin.Stores
{
    public interface IContractInfoStore
    {
        IQueryable<ContractInfo> ContractInfos { get; set; }

        Task<ContractInfo> CreateAsync(SimpleUser userinfo, ContractInfo buildingBaseInfo, string modifyid, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(SimpleUser userinfo, string contractid, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<ContractInfo> buildingBaseList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<ContractInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<ContractInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetModifyAsync<TResult>(Func<IQueryable<ModifyInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TResult>> ListModifyAsync<TResult>(Func<IQueryable<ModifyInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(ContractInfo buildingBase, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<ContractInfo> buildingBaseList, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveAsync(SimpleUser user, ContractInfo buildingBaseInfo, CancellationToken cancellationToken = default(CancellationToken));
        Task UpdateExamineStatus(string modifyId, ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken));

    }
}
