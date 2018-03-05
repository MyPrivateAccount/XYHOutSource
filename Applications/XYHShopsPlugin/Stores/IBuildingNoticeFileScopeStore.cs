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
    public interface IBuildingNoticeFileScopeStore
    {
         IQueryable<BuildingNoticeFileScope> BuildingNoticeFileScopes { get; set; }


        Task<BuildingNoticeFileScope> CreateAsync(BuildingNoticeFileScope buildingNoticeFileScope, CancellationToken cancellationToken = default(CancellationToken));



        Task DeleteAsync(BuildingNoticeFileScope buildingNoticeFileScope, CancellationToken cancellationToken = default(CancellationToken));


        Task DeleteListAsync(List<BuildingNoticeFileScope> buildingNoticeFileScopeList, CancellationToken cancellationToken = default(CancellationToken));


        Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingNoticeFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));



        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BuildingNoticeFileScope>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));


        Task UpdateAsync(BuildingNoticeFileScope buildingNoticeFileScope, CancellationToken cancellationToken = default(CancellationToken));


        Task UpdateListAsync(List<BuildingNoticeFileScope> buildingNoticeFileScopeList, CancellationToken cancellationToken = default(CancellationToken));



        Task SaveAsync(SimpleUser user, string buildingId, List<BuildingNoticeFileScope> buildingNoticeFileScopeList, CancellationToken cancellationToken = default(CancellationToken));
 

        
    }
}
