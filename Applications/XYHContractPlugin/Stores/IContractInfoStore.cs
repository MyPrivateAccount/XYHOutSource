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
        IEnumerable<T> DapperSelect<T>(string sql);

        //IQueryable<ContractFileScope> GetDetailQuery();

        Task<ContractInfo> CreateAsync(SimpleUser userinfo, ContractInfo buildingBaseInfo, string modifyid, string checkaction, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> CreateAsync(SimpleUser userinfo, List<AnnexInfo> annexinfo, CancellationToken cancle = default(CancellationToken));
        Task<bool> CreateAsync(SimpleUser userinfo, List<ComplementInfo> compleinfo, CancellationToken cancle = default(CancellationToken));
        Task<bool> AutoCreateAsync(SimpleUser userinfo, List<ComplementInfo> compleinfo, CancellationToken cancle = default(CancellationToken));
        Task CreateModifyAsync(SimpleUser userinfo, string contractid, string modifyid, int ntype, string checkaction, ExamineStatusEnum exa = ExamineStatusEnum.UnSubmit, bool updatetocontract = true, string ext1 = null, string ext2 = null, string ext3 = null, string ext4 = null, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(SimpleUser userinfo, string contractid, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<ContractInfo> buildingBaseList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<ContractInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<ContractInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetModifyAsync<TResult>(Func<IQueryable<ModifyInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TResult>> GetListModifyAsync<TResult>(Func<IQueryable<ModifyInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> GetListAnnexAsync<TResult>(Func<IQueryable<AnnexInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TResult>> GetListComplementAsync<TResult>(Func<IQueryable<ComplementInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(ContractInfo buildingBase, CancellationToken cancellationToken = default(CancellationToken));
        Task UpdateListAsync(List<AnnexInfo> buildingBase, CancellationToken cancellationToken = default(CancellationToken));
        Task UpdateListAsync(List<ComplementInfo> buildingBase, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<ContractInfo> buildingBaseList, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveAsync(SimpleUser user, ContractInfo buildingBaseInfo, CancellationToken cancellationToken = default(CancellationToken));
        Task UpdateExamineStatus(string modifyId, ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken));

        IQueryable<ContractInfo> ContractInfoAll();
  
    }
}
