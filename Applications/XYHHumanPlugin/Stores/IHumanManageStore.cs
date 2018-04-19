using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHHumanPlugin.Models;
using XYHHumanPlugin.Dto.Response;
using ApplicationCore.Models;

namespace XYHHumanPlugin.Stores
{
    public interface IHumanManageStore
    {
        IEnumerable<T> DapperSelect<T>(string sql);
        Task<HumanInfo> CreateAsync(SimpleUser userinfo, HumanInfo humaninfo, string modifyid, string checkaction, CancellationToken cancellationToken = default(CancellationToken));
        Task CreateAsync(AnnexInfo humaninfo, CancellationToken cancellationToken = default(CancellationToken));
        Task CreateAsync(FileInfo fileinfo, CancellationToken cancellationToken = default(CancellationToken));

        Task CreateListAsync(List<FileInfo> fileInfoList, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(HumanInfo userinfo, string contractid, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<HumanInfo> buildingBaseList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<HumanInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<HumanInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetModifyAsync<TResult>(Func<IQueryable<ModifyInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TResult>> ListModifyAsync<TResult>(Func<IQueryable<ModifyInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(HumanInfo buildingBase, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<HumanInfo> buildingBaseList, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveAsync(HumanInfo humaninfo, CancellationToken cancellationToken = default(CancellationToken));
        Task UpdateExamineStatus(string modifyId, ExamineStatusEnum status, int type, CancellationToken cancellationToken = default(CancellationToken));

    }
}
