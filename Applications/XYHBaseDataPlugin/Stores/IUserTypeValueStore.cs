using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHBaseDataPlugin.Models;

namespace XYHBaseDataPlugin.Stores
{
    public interface IUserTypeValueStore
    {
        IQueryable<UserTypeValue> UserTypeValueAll();

        Task<TResult> GetAsync<TResult>(Func<IQueryable<UserTypeValue>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<UserTypeValue>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<UserTypeValue> CreateAsync(UserTypeValue usertypevalue, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<UserTypeValue> usertypevalueList, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(UserTypeValue usertypevalue, CancellationToken cancellationToken = default(CancellationToken));
    }
}
