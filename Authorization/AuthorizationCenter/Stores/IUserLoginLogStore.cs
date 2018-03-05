using AuthorizationCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public interface IUserLoginLogStore
    {
        IQueryable<UserLoginLog> UserLoginLogs { get; set; }

        Task<UserLoginLog> CreateAsync(UserLoginLog organization, CancellationToken cancellationToken);

        Task<TResult> GetAsync<TResult>(Func<IQueryable<UserLoginLog>, IQueryable<TResult>> query, CancellationToken cancellationToken);

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<UserLoginLog>, IQueryable<TResult>> query, CancellationToken cancellationToken);

    }
}
