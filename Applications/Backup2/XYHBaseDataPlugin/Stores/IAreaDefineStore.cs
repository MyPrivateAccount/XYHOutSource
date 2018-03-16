using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHBaseDataPlugin.Models;

namespace XYHBaseDataPlugin.Stores
{
    public interface IAreaDefineStore
    {
        IQueryable<AreaDefine> AreaDefines { get; set; }

        Task<AreaDefine> CreateAsync(AreaDefine areaDefine, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(AreaDefine areaDefine, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<AreaDefine> areaDefineList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<AreaDefine>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<AreaDefine>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(AreaDefine areaDefine, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<AreaDefine> areaDefineList, CancellationToken cancellationToken = default(CancellationToken));
    }
}
