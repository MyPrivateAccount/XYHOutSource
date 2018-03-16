using ApplicationCore.Models;
using ApplicationCore.Stores;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using ApplicationCore.Dto;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Managers
{
    public class UserManager
    {
        private readonly CoreDbContext _dbContext = null;
        private readonly IDistributedCache _cache = null;
        private readonly string CACHE_PREFIX = "APP_USER_";

        public UserManager(CoreDbContext dbContext, IDistributedCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }



        public async Task<UserInfo> GetUserAsync(string id)
        {
            string key = $"{CACHE_PREFIX}{id}";
            var ui =  _cache.Get<UserInfo>(key);
            if(ui == null)
            {
               var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if(user !=null)
                {
                    ui = new UserInfo()
                    {
                        Id = user.Id,
                        OrganizationId = user.OrganizationId,
                        UserName = user.TrueName
                    };
                    await _cache.SetAsync<UserInfo>(key, ui, new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
                }
            }

            return ui;
        }


        //public UserManager(IUserStore userStore)
        //{
        //    Store = userStore ?? throw new ArgumentNullException(nameof(userStore));
        //}

        //protected IUserStore Store { get; }

        //public IdentityOptions Options { get; set; } = new IdentityOptions();

        //public virtual Task<Users> GetUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    if (principal == null)
        //    {
        //        throw new ArgumentNullException(nameof(principal));
        //    }
        //    var id = GetUserId(principal);
        //    if (id == null)
        //    {
        //        return Task.FromResult<Users>(null);
        //    }
        //    else
        //    {
        //        return Store.GetAsync<Users>(a => a.Where(b => b.Id == id), cancellationToken);
        //    }
        //}


        //public virtual string GetUserId(ClaimsPrincipal principal)
        //{
        //    if (principal == null)
        //    {
        //        throw new ArgumentNullException(nameof(principal));
        //    }
        //    //return principal.FindFirstValue(Options.ClaimsIdentity.UserIdClaimType);
        //    var item = principal.Claims.SingleOrDefault(a => a.ValueType == "http://www.w3.org/2001/XMLSchema#string");
        //    //http://www.w3.org/2001/XMLSchema#string
        //    return item.Value;
        //}
    }
}
