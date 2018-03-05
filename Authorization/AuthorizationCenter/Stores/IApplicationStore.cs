using AuthorizationCenter.Models;
using OpenIddict.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public interface IApplicationStore
    {
        IQueryable<Applications> Applications { get; set; }

        Task<TResult> GetAsync<TResult>(Func<IQueryable<Applications>, IQueryable<TResult>> query);




    }
}
