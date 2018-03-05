using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Stores
{
    public interface IUpdateRecordFileScopeStore
    {
        IQueryable<UpdateRecordFileScope> UpdateRecordFileScopes { get; set; }
        Task<UpdateRecordFileScope> CreateAsync(UpdateRecordFileScope updateRecordFileScope, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteAsync(UpdateRecordFileScope updateRecordFileScope, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteListAsync(List<UpdateRecordFileScope> updateRecordFileScopeList, CancellationToken cancellationToken = default(CancellationToken));
        Task<TResult> GetAsync<TResult>(Func<IQueryable<UpdateRecordFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<UpdateRecordFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task UpdateAsync(UpdateRecordFileScope updateRecordFileScope, CancellationToken cancellationToken = default(CancellationToken));
        Task UpdateListAsync(List<UpdateRecordFileScope> updateRecordFileScopeList, CancellationToken cancellationToken = default(CancellationToken));
        Task SaveAsync(SimpleUser user, string buildingId, List<UpdateRecordFileScope> updateRecordFileScopeList, CancellationToken cancellationToken = default(CancellationToken));

    }
}
