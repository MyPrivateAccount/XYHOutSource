using AuthorizationCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public interface IExtendUserStore : IUserStore<Users>
    {

        Task<TResult> GetAsync<TResult>(Func<IQueryable<Users>, IQueryable<TResult>> query);

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<Users>, IQueryable<TResult>> query);
    }
}
