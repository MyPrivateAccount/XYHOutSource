using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHBaseDataPlugin.Models;

namespace XYHBaseDataPlugin.Stores
{
    public interface IDictionaryGroupStore
    {
        IQueryable<DictionaryGroup> DictionaryGroups { get; set; }

        Task<DictionaryGroup> CreateAsync(DictionaryGroup dictionaryGroup, CancellationToken cancellationToken = default(CancellationToken));


        Task DeleteAsync(DictionaryGroup dictionaryGroup, CancellationToken cancellationToken = default(CancellationToken));


        Task DeleteListAsync(List<DictionaryGroup> dictionaryGroupList, CancellationToken cancellationToken = default(CancellationToken));


        Task<TResult> GetAsync<TResult>(Func<IQueryable<DictionaryGroup>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));



        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<DictionaryGroup>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));


        Task UpdateAsync(DictionaryGroup dictionaryGroup, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<DictionaryGroup> dictionaryGroups, CancellationToken cancellationToken = default(CancellationToken));

    }
}
