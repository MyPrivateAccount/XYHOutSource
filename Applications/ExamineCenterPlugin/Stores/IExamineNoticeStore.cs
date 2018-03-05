using ExamineCenterPlugin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamineCenterPlugin.Stores
{
    public interface IExamineNoticeStore
    {
        IQueryable<ExamineNotice> ExamineNotices { get; set; }
        Task NoticeCallbackAsync(List<string> userIds, ExamineFlow examineFlow, CancellationToken cancellationToken = default(CancellationToken));
        Task CreateExamineNotice(List<ExamineNotice> examineNotice, CancellationToken cancellationToken = default(CancellationToken));
        Task UpdateExamineNotice(List<ExamineNotice> examineNotice, CancellationToken cancellationToken = default(CancellationToken));
        IQueryable<ExamineNotice> GetQuery();
        Task SetReadingStatus(string noticeId);

        Task<TResult> GetAsync<TResult>(Func<IQueryable<ExamineNotice>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<ExamineNotice>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
    }
}
