using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Stores
{
    public interface IBuildingNoticeStore
    {

        IQueryable<BuildingNotice> BuildingNotices { get; set; }

        Task<BuildingNotice> CreateAsync(BuildingNotice buildingNotice, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(BuildingNotice buildingNotice, CancellationToken cancellationToken = default(CancellationToken));


        Task<TResult> GetAsync<TResult>(Func<IQueryable<BuildingNotice>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        IQueryable<BuildingNotice> GetQuery();

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BuildingNotice>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));


    }
}
