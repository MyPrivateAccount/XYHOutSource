using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHBaseDataPlugin.Models;

namespace XYHBaseDataPlugin.Stores
{
    public interface IDictionaryDefineStore
    {
        Task<DictionaryDefine> CreateAsync(DictionaryDefine dictionaryDefine, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(DictionaryDefine dictionaryDefine, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<DictionaryDefine> dictionaryDefineList, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetAsync<TResult>(Func<IQueryable<DictionaryDefine>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<DictionaryDefine>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(DictionaryDefine dictionaryDefine, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<DictionaryDefine> dictionaryDefineList, CancellationToken cancellationToken = default(CancellationToken));

    }
}
