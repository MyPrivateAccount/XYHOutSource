using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHChargePlugin.Models;
using XYHChargePlugin.Dto.Response;
using ApplicationCore.Models;

namespace XYHChargePlugin.Stores
{
    public interface IChargeManageStore
    {
        IEnumerable<T> DapperSelect<T>(string sql);
        Task<ChargeInfo> CreateChargeAsync(SimpleUser userinfo, ChargeInfo chargeinfo, string modifyid, string checkaction, CancellationToken cancellationToken = default(CancellationToken));
        Task CreateCostListAsync(List<CostInfo> costinfo, CancellationToken cancellationToken = default(CancellationToken));
        Task CreateReceiptListAsync(List<ReceiptInfo> costinfo, CancellationToken cancellationToken = default(CancellationToken));

        Task CreateFileListAsync(List<FileInfo> costinfo, CancellationToken cancellationToken = default(CancellationToken));
        Task CreateFileScopeAsync(string strid, List<FileScopeInfo> scope, CancellationToken cancellationToken = default(CancellationToken));
        //Task DeleteAsync(ChargeInfo userinfo, string contractid, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetChargeAsync<TResult>(Func<IQueryable<ChargeInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateExamineStatus(string modifyId, ExamineStatusEnum status, int type, CancellationToken cancellationToken = default(CancellationToken));

    }
}
