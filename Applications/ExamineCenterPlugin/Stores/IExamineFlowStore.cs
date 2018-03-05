using ExamineCenterPlugin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamineCenterPlugin.Stores
{
    public interface IExamineFlowStore
    {
        IQueryable<ExamineFlow> ExamineFlows { get; set; }
        IQueryable<ExamineRecord> ExamineRecords { get; set; }

        Task<ExamineFlow> CreateExamineFlowAsync(ExamineFlow examineFlow, CancellationToken cancellationToken = default(CancellationToken));

        Task<ExamineRecord> CreateExamineRecordAsync(ExamineRecord examineRecord, CancellationToken cancellationToken = default(CancellationToken));

        Task StepCallBackUpdateExamineFlowAsync(string examineUserId, ExamineFlow examineFlow, CancellationToken cancellationToken = default(CancellationToken));

        Task FlowCallBackUpdateExamineFlowAsync(ExamineFlow examineFlow, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateExamineRecordAsync(ExamineRecord examineRecord, CancellationToken cancellationToken = default(CancellationToken));
        Task UpdateExamineRejectAsync(ExamineRecord examineRecord, CancellationToken cancellationToken = default(CancellationToken));

        IQueryable<ExamineRecord> GetRecordQuery();
        IQueryable<ExamineFlow> GetFlowQuery();

        Task<TResult> ExamineFlowGetAsync<TResult>(Func<IQueryable<ExamineFlow>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ExamineFlowListAsync<TResult>(Func<IQueryable<ExamineFlow>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> ExamineRecordGetAsync<TResult>(Func<IQueryable<ExamineRecord>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ExamineRecordListAsync<TResult>(Func<IQueryable<ExamineRecord>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

    }
}
