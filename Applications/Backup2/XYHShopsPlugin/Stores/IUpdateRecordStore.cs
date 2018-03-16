using ApplicationCore.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Stores
{
    public interface IUpdateRecordStore
    {
        IQueryable<UpdateRecord> UpdateRecords { get; set; }

        IQueryable<UpdateRecord> GetDetail();

        Task<UpdateRecord> CreateUpdateRecordAsync(UpdateRecord updateRecord, CancellationToken cancellationToken = default(CancellationToken));


        Task UpdateUpdateRecordAsync(UpdateRecord updateRecord, CancellationToken cancellationToken = default(CancellationToken));

        IQueryable<UpdateRecord> GetQuery();
        IQueryable<UpdateRecord> GetFollowDetail(string userId);

        Task<TResult> GetAsync<TResult>(Func<IQueryable<UpdateRecord>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));


        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<UpdateRecord>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task ShopsHotCallbackAsync(ExamineResponse examineResponse);

        Task ShopsAddCallbackAsync(ExamineResponse examineResponse);

        Task ReportRuleCallbackAsync(ExamineResponse examineResponse);

        Task CommissionTypeCallbackAsync(ExamineResponse examineResponse);

        Task BuildingNoCallbackAsync(ExamineResponse examineResponse);

        Task DiscountPolicyCallbackAsync(ExamineResponse examineResponse);

        Task PriceCallbackAsync(ExamineResponse examineResponse);

    }
}
