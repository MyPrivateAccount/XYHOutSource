using ApplicationCore.Models;
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
    public interface IBuildingsStore
    {
        IQueryable<Buildings> Buildings { get; set; }

        IQueryable<Buildings> GetSimpleSerchQuery();

        IQueryable<Buildings> GetDetailQuery();

        Task<Buildings> CreateAsync(Buildings buildings, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(SimpleUser user, Buildings buildings, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<Buildings> buildingsList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<Buildings>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<Buildings>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(Buildings buildings, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<Buildings> buildingsList, CancellationToken cancellationToken = default(CancellationToken));


        Task SaveSummaryAsync(SimpleUser user, string buildingId, string summary,string source, string sourceId, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveCommissionAsync(SimpleUser user, string buildingId, string commission, string source, string sourceId, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateExamineStatus(string buildingId, Models.ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken));

    }
}
